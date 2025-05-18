using EasyNetQ;
using UserBoard.Models;
using UserBoard.Contracts;
using Microsoft.EntityFrameworkCore;
using BoardUser;
using Users.Models;
using Users.Contracts;

namespace UserBoard.Services;

public class UserBoardService(UserBoardDBContext context, IBus _bus)
{
  private readonly UserBoardDBContext DB = context;
  private readonly IBus bus = _bus;

  public async Task<UserBoardModel?> AddAsync(UserBoardCreateContract contract)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (user == null)
      return null;

    UserBoardModel? el = (await DB.UserBoards.AddAsync(new()
    {
      UserId = contract.UserId,
      BoardId = contract.BoardId,
      Role = contract.Role
    })).Entity;
    await DB.SaveChangesAsync();

    return el;
  }

  public async Task<UserBoardModel?> GetAsync(UserBoardGetContract contract)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (user == null)
      return null;

    var el = await DB.UserBoards.FirstOrDefaultAsync(x => x.BoardId == contract.BoardId && x.UserId == contract.UserId);
    return el;
  }

  public async Task<UserBoardModel?> UpdateAsync(UserBoardUpdateContract contract)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (user == null)
      return null;

    var el = await GetAsync(new(contract.BoardId, contract.UserId));
    if (el is null)
      return null;

    el.Role = contract.Role ?? el.Role;

    DB.UserBoards.Update(el);
    await DB.SaveChangesAsync();

    return el;
  }

  public async Task<UserBoardModel?> DeleteAsync(UserBoardDeleteContract contract)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (user == null)
      return null;

    var el = await GetAsync(new(contract.BoardId, contract.UserId));
    if (el is null)
      return null;

    DB.UserBoards.Remove(el);

    await DB.SaveChangesAsync();
    return el;
  }

  public async Task<UserBoardModel[]?> GetUserBoardModelsByUser(UserBoardsGetByUserContract contract)
  {
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: contract.UserId));
    if (user == null)
      return null;

    UserBoardModel[] userBoardModels = [.. DB.UserBoards.Where(x => x.UserId == contract.UserId)];
    return userBoardModels;
  }
}