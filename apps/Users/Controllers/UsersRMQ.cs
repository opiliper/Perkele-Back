using EasyNetQ;
using Users.Contracts;
using Users.Models;
using Users.Services;

namespace Users.Controllers;

public class UsersRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly IServiceProvider serviceProvider;
  readonly List<IDisposable> disposables = [];

  public UsersRMQController(IBus _bus, IServiceProvider _serviceProvider)
  {
    bus = _bus;
    serviceProvider = _serviceProvider;

    List<EasyNetQ.Internals.AwaitableDisposable<IDisposable>> tasks =
    [
      bus.Rpc.RespondAsync<GetUserContract, UserModel?>(GetUser),
      bus.Rpc.RespondAsync<CreateUserContract, UserModel?>(CreateUser),
      bus.Rpc.RespondAsync<UpdateUserContract, UserModel?>(UpdateUser),
      bus.Rpc.RespondAsync<DeleteUserContract, UserModel?>(DeleteUser),
    ];
    tasks.ForEach(async task => disposables.Add(await task));
  }

  public async Task<UserModel?> GetUser(GetUserContract contract)
  {
    var usersService = serviceProvider.GetRequiredService<UsersService>();
    return await usersService.GetUserByIdAsync(contract);
  }

  public async Task<UserModel?> CreateUser(CreateUserContract contract)
  {
    var usersService = serviceProvider.GetRequiredService<UsersService>();
    return await usersService.CreateUserAsync(contract);
  }

  public async Task<UserModel?> UpdateUser(UpdateUserContract contract)
  {
    var usersService = serviceProvider.GetRequiredService<UsersService>();
    return await usersService.UpdateUserAsync(contract);
  }

  public async Task<UserModel?> DeleteUser(DeleteUserContract contract)
  {
    var usersService = serviceProvider.GetRequiredService<UsersService>();
    return await usersService.DeleteUserAsync(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}