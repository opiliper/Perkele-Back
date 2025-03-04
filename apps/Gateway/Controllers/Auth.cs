using System.Text.Json;
using System.Text.Json.Serialization;
using Auth.DTOs;
using EasyNetQ;
using Gateway.Services;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Route("api/v1_0/Auth")]
public class AuthController(IBus _bus, AuthService _authService) : ControllerBase
{
  private readonly IBus bus = _bus;
  private readonly AuthService authService = _authService;
  private readonly JsonSerializerOptions json_options = new() { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpPost]
  [Route("login")]
  public async Task<ActionResult<string>> Login([FromBody] LoginDTO dTO)
  {
    var result = await authService.Login(dTO);

    return result switch
    {
      LoginResults.USER_NOT_FOUND => NotFound(LoginResults.USER_NOT_FOUND),
      LoginResults.WRONG_PASSWORD => BadRequest(LoginResults.WRONG_PASSWORD),
      _ => Ok(result),
    };
  }
}
