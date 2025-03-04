using Board.Contracts;
using Board.Models;
using Board.Services;
using EasyNetQ;

namespace Board.Controllers;

public class TicketRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly TicketService ticketService;
  readonly List<IDisposable> disposables = new();

  public TicketRMQController(IBus _bus, TicketService _ticketService)
  {
    bus = _bus;
    ticketService = _ticketService;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks =
    [
      bus.Rpc.RespondAsync<TicketGetContract, TicketModel?>(GetTicket),
      bus.Rpc.RespondAsync<TicketCreateContract, TicketModel?>(CreateTicket),
      bus.Rpc.RespondAsync<TicketUpdateContract, TicketModel?>(UpdateTicket),
      bus.Rpc.RespondAsync<TicketDeleteContract, TicketModel?>(DeleteTicket),
      bus.Rpc.RespondAsync<TicketNodeCreateContract, TicketNodeModel?>(AddNode),
      bus.Rpc.RespondAsync<TicketNodeDeleteContract, TicketNodeModel?>(DeleteNode),
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<TicketModel?> GetTicket(TicketGetContract contract)
  {
    return await ticketService.GetAsync(contract);
  }

  public async Task<TicketModel?> CreateTicket(TicketCreateContract contract)
  {
    return await ticketService.AddAsync(contract);
  }

  public async Task<TicketModel?> UpdateTicket(TicketUpdateContract contract)
  {
    return await ticketService.UpdateAsync(contract);
  }

  public async Task<TicketModel?> DeleteTicket(TicketDeleteContract contract)
  {
    return await ticketService.DeleteAsync(contract);
  }

  public async Task<TicketNodeModel?> AddNode(TicketNodeCreateContract contract)
  {
    return await ticketService.AddNodeAsync(contract);
  }

  public async Task<TicketNodeModel?> DeleteNode(TicketNodeDeleteContract contract)
  {
    return await ticketService.DeleteNodeAsync(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}
