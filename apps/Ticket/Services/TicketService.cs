using Ticket.Contracts;
using Ticket.Models;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Board.Contracts;
using Board.Models;

namespace Ticket.Services;

public class TicketService(TicketDBContext context, IBus bus)
{
  private readonly TicketDBContext DB = context;
  private readonly IBus Bus = bus;

  public async Task<TicketModel?> AddAsync(TicketCreateContract contract)
  {
    var _board = await Bus.Rpc.RequestAsync<BoardGetContract, BoardModel>(new (contract.DTO.Board_id));
    if (_board is null)
      return null;

    var el_entry = await DB.Tickets.AddAsync(new TicketModel()
    {
      BoardId = _board.Id,
      Title = contract.DTO.Title,
      Value = contract.DTO.Value,
      Column = contract.DTO.Column,
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
    return el_entry.Entity;
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

    el.Title = contract.DTO.Title ?? el.Title;
    el.Value = contract.DTO.Value ?? el.Value;
    el.Column = contract.DTO.Column ?? el.Column;
    if (contract.DTO.Nodes is not null)
    {
      foreach (var element in contract.DTO.Nodes)
      {
        var _node = await DB.TicketNodes.FindAsync(element.Key, contract.Id);

        if (_node is null)
          continue;

        _node.Value = element.Value;

        DB.TicketNodes.Update(_node);
      }
    }
    DB.Tickets.Update(el);

    await DB.SaveChangesAsync();
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