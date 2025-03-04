using Board.Contracts;
using Board.DTOs;
using Board.Models;
using Microsoft.EntityFrameworkCore;

namespace Board.Services;

public class TicketService(BoardDBContext context, BoardService _boardService)
{
  private readonly BoardDBContext DB = context;
  private readonly BoardService boardService = _boardService;

  public async Task<TicketModel?> AddAsync(TicketCreateContract contract)
  {
    var _board = await boardService.GetAsync(new(contract.DTO.Board_id));
    if (_board is null)
      return null;

    var el_entry = await DB.Tickets.AddAsync(new TicketModel()
    {
      BoardId = _board.Id,
      Nodes = []
    });
    await DB.SaveChangesAsync();

    if (contract.DTO.Nodes is not null)
    {
      await DB.TicketNodes.AddRangeAsync(
        contract.DTO.Nodes.ConvertAll(u =>
        {
          return new TicketNodeModel()
          {
            TicketId = el_entry.Entity.Id,
            Key = u.Key,
            Value = u.Value
          };
        })
      );
    }

    await DB.SaveChangesAsync();

    var el = await GetAsync(new(el_entry.Entity.Id));
    return el;
  }

  public async Task<TicketModel?> GetAsync(TicketGetContract contract)
  {
    var el = await DB.Tickets
            .Include(t => t.Nodes)
            .Include(t => t.Board)
            .FirstOrDefaultAsync(e => e.Id == contract.Id);

    if (el is null)
      return null;
    return el;
  }

  public async Task<TicketModel?> UpdateAsync(TicketUpdateContract contract)
  {
    var el = await GetAsync(new(contract.Id));
    if (el is null)
      return null;

    if (contract.DTO.Nodes is not null)
    {
      foreach (var element in contract.DTO.Nodes)
      {
        var _node = await DB.TicketNodes.FindAsync(element.Key, contract.Id);

        if (_node is null)
          continue;

        _node.Value = element.Value;

        DB.TicketNodes.Update(_node);
        await DB.SaveChangesAsync();
      }
    }

    return el;
  }

  public async Task<TicketModel?> DeleteAsync(TicketDeleteContract contract)
  {
    var el = await GetAsync(new(contract.Id));
    if (el is null)
      return null;

    DB.Tickets.Remove(el);
    await DB.SaveChangesAsync();

    return el;
  }

  public async Task<TicketNodeModel?> AddNodeAsync(TicketNodeCreateContract contract)
  {
    var ticket = await GetAsync(new(contract.TicketId));
    if (ticket is null)
      return null;

    await DB.TicketNodes.AddAsync(new TicketNodeModel()
    {
      TicketId = contract.TicketId,
      Key = contract.DTO.Key,
      Value = contract.DTO.Value
    });
    await DB.SaveChangesAsync();

    var el = await DB.TicketNodes.FindAsync(contract.DTO.Key, contract.TicketId);
    return el;
  }

  public async Task<TicketNodeModel?> DeleteNodeAsync(TicketNodeDeleteContract contract)
  {
    var el = await DB.TicketNodes.FindAsync(contract.Key, contract.TicketId);

    if (el is null)
      return null;

    DB.TicketNodes.Remove(el);
    await DB.SaveChangesAsync();

    return el;
  }
}