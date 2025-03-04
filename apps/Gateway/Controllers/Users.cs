using System.Text.Json;
using System.Text.Json.Serialization;
using Users.Contracts;
using Users.DTOs;
using Users.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Gateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1_0/Users")]
public class UsersController(IBus _bus) : ControllerBase
{
  private readonly IBus bus = _bus;
  private readonly JsonSerializerOptions json_options = new () { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpGet("{id}")]
  public async Task<ActionResult<UserModel?>> GetUser(uint id)
  {
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: id));
    if (user == null) {
      return NotFound();
    }
    return Ok(new JsonResult(user, json_options).Value);
  }

  [HttpPost]
  public async Task<ActionResult<UserModel?>> CreateUser([FromBody] CreateUserDTO createUserDTO)
  {
    var user = await bus.Rpc.RequestAsync<CreateUserContract, UserModel?>(new(createUserDTO));
    if (user == null) {
      return BadRequest();
    }
    return Ok(new JsonResult(user, json_options).Value);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<UserModel?>> UpdateUser(uint id, [FromBody] UpdateUserDTO updateUserDTO)
  {
    var user = await bus.Rpc.RequestAsync<UpdateUserContract, UserModel?>(new(id, updateUserDTO));
    if (user == null) {
      return NotFound();
    }
    return Ok(new JsonResult(user, json_options).Value);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<UserModel?>> DeleteUser(uint id)
  {
    var user = await bus.Rpc.RequestAsync<DeleteUserContract, UserModel?>(new(id));
    if (user == null) {
      return NotFound();
    }
    return Ok(new JsonResult(user, json_options).Value);
  }
}