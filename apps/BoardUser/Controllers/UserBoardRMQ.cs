using UserBoard.Contracts;
using UserBoard.Models;
using UserBoard.Services;
using EasyNetQ;

namespace UserBoard.Controllers;

public class UserBoardRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly UserBoardService userBoardService;
  readonly List<IDisposable> disposables = [];

  public UserBoardRMQController(IBus _bus, UserBoardService _userBoardService)
  {
    bus = _bus;
    userBoardService = _userBoardService;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks =
    [
      bus.Rpc.RespondAsync<UserBoardGetContract, UserBoardModel?>(GetUserBoard),
      bus.Rpc.RespondAsync<UserBoardCreateContract, UserBoardModel?>(CreateUserBoard),
      bus.Rpc.RespondAsync<UserBoardUpdateContract, UserBoardModel?>(UpdateUserBoard),
      bus.Rpc.RespondAsync<UserBoardDeleteContract, UserBoardModel?>(DeleteUserBoard),
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<UserBoardModel?> GetUserBoard(UserBoardGetContract contract)
  {
    return await userBoardService.GetAsync(contract);
  }

  public async Task<UserBoardModel?> CreateUserBoard(UserBoardCreateContract contract)
  {
    return await userBoardService.AddAsync(contract);
  }

  public async Task<UserBoardModel?> UpdateUserBoard(UserBoardUpdateContract contract)
  {
    return await userBoardService.UpdateAsync(contract);
  }

  public async Task<UserBoardModel?> DeleteUserBoard(UserBoardDeleteContract contract)
  {
    return await userBoardService.DeleteAsync(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}