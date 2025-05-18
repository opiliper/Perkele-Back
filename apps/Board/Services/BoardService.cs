using Board.Contracts;
using Board.Models;
using Microsoft.EntityFrameworkCore;

namespace Board.Services;

public class BoardService(BoardDBContext context)
{
  private readonly BoardDBContext DB = context;
  public async Task<BoardModel> AddAsync(BoardCreateContract contract)
  {
    var el_entry = await DB.Boards.AddAsync(new BoardModel() {
      Name = string.IsNullOrEmpty(contract.DTO.Name)? "Some Name" : contract.DTO.Name,
    });
    await DB.SaveChangesAsync();

    return (await DB.Boards
                  .Include(u => u.Tickets)
                  .FirstOrDefaultAsync(u => u.Id == el_entry.Entity.Id))!;
  }

  public async Task<BoardModel?> GetAsync(BoardGetContract contract)
  {
    var el = await DB.Boards
                  .Include(u => u.Tickets)
                    .ThenInclude(u => u.Nodes)
                  .FirstOrDefaultAsync(u => u.Id == contract.Id);
    return el;
  }

  public async Task<BoardModel?> UpdateAsync(BoardUpdateContract contract)
  {
    var el = await GetAsync(new (contract.Id));
    if (el is null)
      return null;

    el.Name = contract.DTO.Name ?? el.Name;

    DB.Boards.Update(el);
    await DB.SaveChangesAsync();

    return el;
  }

  public async Task<BoardModel?> DeleteAsync(BoardDeleteContract contract)
  {
    var el = await GetAsync(new(contract.Id));
    if (el is null)
      return null;

    DB.Boards.Remove(el);

    await DB.SaveChangesAsync();
    return el;
  }
}