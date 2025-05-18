using UserBoard.Contracts;
using UserBoard.Models;
using UserBoard.Services;
using EasyNetQ;
using BoardUser.Services;

namespace UserBoard.Controllers;

public class UserBoardRequestRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly IServiceProvider serviceProvider;
  readonly List<IDisposable> disposables = [];

  public UserBoardRequestRMQController(IBus _bus, IServiceProvider _serviceProvider)
  {
    bus = _bus;
    serviceProvider = _serviceProvider;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks =
    [
      bus.Rpc.RespondAsync<UserBoardRequestGetContract, UserBoardRequestModel?>(Get),
      bus.Rpc.RespondAsync<UserBoardRequestCreateContract, UserBoardRequestModel?>(Create),
      bus.Rpc.RespondAsync<UserBoardRequestDeleteContract, UserBoardRequestModel?>(Delete),
      bus.Rpc.RespondAsync<UserBoardRequestsGetByUserContract, UserBoardRequestModel[]?>(GetUserBoardRequestsByUser),
      bus.Rpc.RespondAsync<UserBoardRequestsGetByBoardContract, UserBoardRequestModel[]?>(GetUserBoardRequestsByBoard)
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<UserBoardRequestModel[]?> GetUserBoardRequestsByBoard(UserBoardRequestsGetByBoardContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardRequestsService>();
    return await userBoardService.GetUserBoardRequestsByBoard(contract);
  }

  public async Task<UserBoardRequestModel[]?> GetUserBoardRequestsByUser(UserBoardRequestsGetByUserContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardRequestsService>();
    return await userBoardService.GetUserBoardRequestsByUser(contract);
  }

  public async Task<UserBoardRequestModel?> Get(UserBoardRequestGetContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardRequestsService>();
    return await userBoardService.Get(contract);
  }

  public async Task<UserBoardRequestModel?> Create(UserBoardRequestCreateContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardRequestsService>();
    return await userBoardService.Create(contract);
  }

  public async Task<UserBoardRequestModel?> Delete(UserBoardRequestDeleteContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardRequestsService>();
    return await userBoardService.Delete(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}