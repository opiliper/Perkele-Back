using System.Text.Json;
using System.Text.Json.Serialization;
using Auth.DTOs;
using Auth.Results;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Users.DTOs;

namespace Gateway.Controllers;

[ApiController]
[Route("api/v1_0/Auth")]
public class AuthController(AuthService _authService) : ControllerBase
{
  private readonly AuthService authService = _authService;
  private readonly JsonSerializerOptions json_options = new() { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<string>> Login([FromBody] LoginDTO dTO)
  {
    var result = await authService.Login(new(Email: dTO.Email, Password: dTO.Password));

    return result switch
    {
      LoginResults.USER_NOT_FOUND => NotFound(LoginResults.USER_NOT_FOUND),
      LoginResults.WRONG_PASSWORD => BadRequest(LoginResults.WRONG_PASSWORD),
      _ => Ok(result),
    };
  }

  [HttpPost]
  [Route("register")]
  public async Task<ActionResult<RegisterResult>> Register([FromBody] CreateUserDTO dTO)
  {
    var result = await authService.Register(new(dTO));
    if (result is null)
      return BadRequest();

    return result;
  }
}
