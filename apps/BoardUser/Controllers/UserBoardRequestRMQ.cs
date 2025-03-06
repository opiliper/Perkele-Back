using UserBoard.Contracts;
using UserBoard.Models;
using UserBoard.Services;
using EasyNetQ;
using BoardUser.Services;

namespace UserBoard.Controllers;

public class UserBoardRequestRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly UserBoardRequestsService userBoardService;
  readonly List<IDisposable> disposables = [];

  public UserBoardRequestRMQController(IBus _bus, UserBoardRequestsService _userBoardService)
  {
    bus = _bus;
    userBoardService = _userBoardService;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks =
    [
      bus.Rpc.RespondAsync<UserBoardRequestGetContract, UserBoardRequestModel?>(Get),
      bus.Rpc.RespondAsync<UserBoardRequestCreateContract, UserBoardRequestModel?>(Create),
      bus.Rpc.RespondAsync<UserBoardRequestDeleteContract, UserBoardRequestModel?>(Delete),
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<UserBoardRequestModel?> Get(UserBoardRequestGetContract contract)
  {
    return await userBoardService.Get(contract);
  }

  public async Task<UserBoardRequestModel?> Create(UserBoardRequestCreateContract contract)
  {
    return await userBoardService.Create(contract);
  }

  public async Task<UserBoardRequestModel?> Delete(UserBoardRequestDeleteContract contract)
  {
    return await userBoardService.Delete(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}