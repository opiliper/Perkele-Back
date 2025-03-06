using Board.Contracts;
using Board.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserBoard.Contracts;
using UserBoard.DTOs;
using UserBoard.Enums;
using UserBoard.Models;
using Users.Contracts;
using Users.Models;

namespace Gateway.Controllers;

[Authorize]
[ApiController]
[Route("api/v1_0/UserBoard")]
public class UserBoardController(IBus _bus) : ControllerBase
{
  private readonly IBus bus = _bus;
  
  [HttpPost("change_access")]
  public async Task<ActionResult<UserBoardModel?>> GiveUserAccessToBoard(UserBoardDTO dTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var source_user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (source_user == null) 
      return Forbid();

    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(dTO.BoardId));
    if (board == null) {
      return NotFound();
    }

    var target_user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: dTO.UserId));
    if (target_user == null) 
      return NotFound();

    var userBoardModel = await bus.Rpc.RequestAsync<UserBoardGetContract, UserBoardModel?>(new(dTO.BoardId, ctx_user_id));
    if (userBoardModel == null || userBoardModel.Role < UserBoardRoleEnum.Full) {
      return Forbid();
    }

    if (dTO.UserBoardRole == null) {
      return Ok(await bus.Rpc.RequestAsync<UserBoardDeleteContract, UserBoardModel?>(new (dTO.BoardId, dTO.UserId)));
    }
    
    var userBoardRequest = await bus.Rpc.RequestAsync<UserBoardRequestDeleteContract, UserBoardRequestModel?>(new (dTO.BoardId, dTO.UserId));
    if (userBoardRequest == null)
      return NotFound();

    return Ok(await bus.Rpc.RequestAsync<UserBoardCreateContract, UserBoardModel?>(new (dTO.BoardId, dTO.UserId, (UserBoardRoleEnum)dTO.UserBoardRole)));
  }
  [HttpPost("access_request")]
  public async Task<ActionResult<UserBoardRequestModel?>> CreateUserBoardRequest(UserBoardRequestDTO dTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var source_user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (source_user == null) 
      return Forbid();

    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(dTO.BoardId));
    if (board == null) {
      return NotFound();
    }

    var userBoardRequest = await bus.Rpc.RequestAsync<UserBoardRequestCreateContract, UserBoardRequestModel?>(new (dTO.BoardId, dTO.UserId));
    return Ok(userBoardRequest);
  }
}