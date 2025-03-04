using EasyNetQ;
using UserBoard.Models;
using UserBoard.Contracts;
using Microsoft.EntityFrameworkCore;
using Auth;

namespace UserBoard.Services;

public class UserBoardService(UserBoardDBContext context, IBus _bus)
{
  private readonly UserBoardDBContext DB = context;
  private readonly IBus bus = _bus;

  public async Task<UserBoardModel?> AddAsync(UserBoardCreateContract contract)
  {
    var el_entry = await DB.UserBoards.AddAsync(new () {
      UserId = contract.UserId,
      BoardId = contract.BoardId,
      Role = contract.Role
    });
    await DB.SaveChangesAsync();

    return el_entry.Entity;
  }

  public async Task<UserBoardModel?> GetAsync(UserBoardGetContract contract)
  {
    var el = await DB.UserBoards.FirstAsync(x => x.BoardId == contract.BoardId && x.UserId == contract.UserId);
    return el;
  }

  public async Task<UserBoardModel?> UpdateAsync(UserBoardUpdateContract contract)
  {
    var el = await GetAsync(new (contract.BoardId, contract.UserId));
    if (el is null)
      return null;

    el.Role = contract.Role ?? el.Role;

    DB.UserBoards.Update(el);
    await DB.SaveChangesAsync();

    return el;
  }

  public async Task<UserBoardModel?> DeleteAsync(UserBoardDeleteContract contract)
  {
    var el = await GetAsync(new (contract.BoardId, contract.UserId));
    if (el is null)
      return null;

    DB.UserBoards.Remove(el);

    await DB.SaveChangesAsync();
    return el;
  }
}