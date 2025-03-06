using Board.Contracts;
using Board.Models;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using UserBoard.Contracts;
using UserBoard.Models;
using Users.Contracts;
using Users.Models;

namespace BoardUser.Services;

public class UserBoardRequestsService(UserBoardDBContext context, IBus _bus)
{
  private readonly UserBoardDBContext DB = context;
  private readonly IBus bus = _bus;

  public async Task<UserBoardRequestModel?> Get(UserBoardRequestGetContract contract)
  {
    var target_user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (target_user == null) 
      return null;

    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(contract.BoardId));
    if (board == null) {
      return null;
    }

    var el = await DB.UserBoardRequests.FirstOrDefaultAsync(el => el.BoardId == contract.BoardId || el.UserId == contract.UserId);
    return el;
  }
  public async Task<UserBoardRequestModel?> Create(UserBoardRequestCreateContract contract)
  {
    var target_user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (target_user == null)
      return null;

    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(contract.BoardId));
    if (board == null) {
      return null;
    }

    var el = (await DB.UserBoardRequests.AddAsync(new () {
      UserId = contract.UserId,
      BoardId = contract.BoardId,
    })).Entity;
    DB.SaveChanges();
    return el;
  }
  public async Task<UserBoardRequestModel?> Delete(UserBoardRequestDeleteContract contract)
  {
    var target_user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (target_user == null) 
      return null;

    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(contract.BoardId));
    if (board == null) {
      return null;
    }

    var el = await Get(new (contract.BoardId, contract.UserId));
    if (el == null)
      return null;

    DB.UserBoardRequests.Remove(el);
    DB.SaveChanges();
    return el;
  }
}