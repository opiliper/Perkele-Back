using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.DTOs;
using EasyNetQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Users.Contracts;
using Users.Models;

namespace Gateway.Services;

public class AuthService(IBus _bus)
{
  private readonly IBus bus = _bus;
  public async Task<string> Login(LoginDTO dTO)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Email: dTO.Email));
    if (user == null)
      return LoginResults.USER_NOT_FOUND;

    if (!IsPasswordRight(user, dTO.Password))
      return LoginResults.WRONG_PASSWORD;

    return CreateJWT(user);
  }
  public bool IsPasswordRight(UserModel user, string password)
  {
    bool result = false;

    PasswordHasher<UserLightModel> hasher = new();
    var hash_check = hasher.VerifyHashedPassword((UserLightModel)user, user.Password_Hash!, password);
    if (hash_check != PasswordVerificationResult.Failed)
    {
      result = true;
    }
    return result;
  }

  public string CreateJWT(UserModel user)
  {
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

public static class LoginResults
{
  public const string USER_NOT_FOUND = "No Such User Idiot";
  public const string WRONG_PASSWORD = "Wrong Password Bruh";
}