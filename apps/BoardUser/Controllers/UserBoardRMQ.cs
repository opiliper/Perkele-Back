using UserBoard.Contracts;
using UserBoard.Models;
using UserBoard.Services;
using EasyNetQ;

namespace UserBoard.Controllers;

public class UserBoardRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly IServiceProvider serviceProvider;
  readonly List<IDisposable> disposables = [];

  public UserBoardRMQController(IBus _bus, IServiceProvider _serviceProvider)
  {
    bus = _bus;
    serviceProvider = _serviceProvider;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks =
    [
      bus.Rpc.RespondAsync<UserBoardGetContract, UserBoardModel?>(GetUserBoard),
      bus.Rpc.RespondAsync<UserBoardCreateContract, UserBoardModel?>(CreateUserBoard),
      bus.Rpc.RespondAsync<UserBoardUpdateContract, UserBoardModel?>(UpdateUserBoard),
      bus.Rpc.RespondAsync<UserBoardDeleteContract, UserBoardModel?>(DeleteUserBoard),
      bus.Rpc.RespondAsync<UserBoardsGetByUserContract, UserBoardModel[]?>(GetUserBoardsByUser)
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<UserBoardModel[]?> GetUserBoardsByUser(UserBoardsGetByUserContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardService>();
    return await userBoardService.GetUserBoardModelsByUser(contract);
  }

  public async Task<UserBoardModel?> GetUserBoard(UserBoardGetContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardService>();
    return await userBoardService.GetAsync(contract);
  }

  public async Task<UserBoardModel?> CreateUserBoard(UserBoardCreateContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardService>();
    return await userBoardService.AddAsync(contract);
  }

  public async Task<UserBoardModel?> UpdateUserBoard(UserBoardUpdateContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardService>();
    return await userBoardService.UpdateAsync(contract);
  }

  public async Task<UserBoardModel?> DeleteUserBoard(UserBoardDeleteContract contract)
  {
    var userBoardService = serviceProvider.GetRequiredService<UserBoardService>();
    return await userBoardService.DeleteAsync(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}