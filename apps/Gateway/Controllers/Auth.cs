using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using Auth.DTOs;
using EasyNetQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Users.Contracts;
using Users.Models;

namespace Gateway.Controllers;

[ApiController]
[Route("api/v1_0/Auth")]
public class AuthController(IBus _bus) : ControllerBase
{
  private readonly IBus bus = _bus;
  private readonly JsonSerializerOptions json_options = new() { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<string>> Login([FromBody] LoginDTO dTO)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new (Email: dTO.Email));
    if (user == null)
      return NotFound();
    PasswordHasher<UserLightModel> hasher = new();
    
    var hash_check = hasher.VerifyHashedPassword((UserLightModel)user, user.Password_Hash!, dTO.Password);
    if (hash_check != (PasswordVerificationResult.Success | PasswordVerificationResult.SuccessRehashNeeded)) {
      return BadRequest("Bruh Wrong Password");
    }

    var claims = new List<Claim> { new(ClaimTypes.Email, user.Email) };
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(jwt);
  }
}
