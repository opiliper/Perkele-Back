using System.Text.Json;
using System.Text.Json.Serialization;
using Board.Contracts;
using Board.DTOs;
using Board.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBoard.Contracts;
using UserBoard.Enums;
using UserBoard.Models;
using Users.Contracts;
using Users.Models;

namespace Gateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1_0/Board")]
public class BoardController(IBus _bus) : ControllerBase
{
  private readonly IBus bus = _bus;
  private readonly JsonSerializerOptions json_options = new() { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpGet("{id}")]
  public async Task<ActionResult<BoardModel?>> GetBoard(uint id)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return NotFound();

    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(id));
    if (board == null)
    {
      return NotFound();
    }

    var user_id = HttpContext.User.FindFirst("Id")!.Value;
    var userBoardModel = await bus.Rpc.RequestAsync<UserBoardGetContract, UserBoardModel?>(new(id, Convert.ToUInt32(user_id)));
    if (userBoardModel == null || userBoardModel.Role < UserBoardRoleEnum.ReadOnly)
      return Forbid();

    return Ok(new JsonResult(board, json_options).Value);
  }

  [HttpPost]
  public async Task<ActionResult<BoardModel?>> CreateBoard([FromBody] BoardAddDTO boardAddDTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return NotFound();

    var board = await bus.Rpc.RequestAsync<BoardCreateContract, BoardModel?>(new(boardAddDTO));
    if (board == null) {
      return BadRequest();
    }

    var user_id = HttpContext.User.FindFirst("Id")!.Value;
    
    await bus.Rpc.RequestAsync<UserBoardCreateContract, UserBoardModel?>(new (board.Id, Convert.ToUInt32(user_id), UserBoardRoleEnum.Full));

    return Ok(new JsonResult(board, json_options).Value);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<BoardModel?>> UpdateBoard(uint id, [FromBody] BoardUpdateDTO boardUpdateDTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return NotFound();

    var board = await bus.Rpc.RequestAsync<BoardUpdateContract, BoardModel?>(new(id, boardUpdateDTO));
    if (board == null)
    {
      return NotFound();
    }

    var user_id = HttpContext.User.FindFirst("Id")!.Value;
    var userBoardModel = await bus.Rpc.RequestAsync<UserBoardGetContract, UserBoardModel?>(new(id, Convert.ToUInt32(user_id)));
    if (userBoardModel == null || userBoardModel.Role < UserBoardRoleEnum.Full) {
      return Forbid();
    }

    return Ok(new JsonResult(board, json_options).Value);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<BoardModel?>> DeleteBoard(uint id)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return NotFound();

    var board = await bus.Rpc.RequestAsync<BoardDeleteContract, BoardModel?>(new(id));
    if (board == null) {
      return NotFound();
    }

    var user_id = HttpContext.User.FindFirst("Id")!.Value;
    var userBoardModel = await bus.Rpc.RequestAsync<UserBoardGetContract, UserBoardModel?>(new(id, Convert.ToUInt32(user_id)));
    if (userBoardModel == null || userBoardModel.Role < UserBoardRoleEnum.Full) {
      return Forbid();
    }

    return Ok(new JsonResult(board, json_options).Value);
  }
}