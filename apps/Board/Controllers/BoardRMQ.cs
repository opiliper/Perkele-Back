using Board.Contracts;
using Board.DTOs;
using Board.Models;
using Board.Services;
using EasyNetQ;

namespace Board.Controllers;

public class BoardRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly BoardService boardService;
  readonly List<IDisposable> disposables = [];
  public BoardRMQController(IBus _bus, BoardService _boardService, ILogger<BoardRMQController> logger)
  {
    bus = _bus;
    boardService = _boardService;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks = [
      bus.Rpc.RespondAsync<BoardGetContract, BoardModel?>(GetBoard),
      bus.Rpc.RespondAsync<BoardCreateContract, BoardModel?>(CreateBoard),
      bus.Rpc.RespondAsync<BoardUpdateContract, BoardModel?>(UpdateBoard),
      bus.Rpc.RespondAsync<BoardDeleteContract, BoardModel?>(DeleteBoard),
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<BoardModel?> GetBoard(BoardGetContract contract)
  {
    return await boardService.GetAsync(contract);
  }

  public async Task<BoardModel?> CreateBoard(BoardCreateContract contract)
  {
    return await boardService.AddAsync(contract);
  }

  public async Task<BoardModel?> UpdateBoard(BoardUpdateContract contract)
  {
    return await boardService.UpdateAsync(contract);
  }

  public async Task<BoardModel?> DeleteBoard(BoardDeleteContract contract)
  {
    return await boardService.DeleteAsync(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}