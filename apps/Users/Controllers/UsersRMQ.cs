using EasyNetQ;
using Users.Contracts;
using Users.Models;
using Users.Services;

namespace Users.Controllers;

public class UsersRMQController : IDisposable
{
  private readonly IBus bus;
  private readonly UsersService usersService;
  readonly List<IDisposable> disposables = [];

  public UsersRMQController(IBus _bus, UsersService _usersService)
  {
    bus = _bus;
    usersService = _usersService;

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
    return await usersService.GetUserByIdAsync(contract);
  }

  public async Task<UserModel?> CreateUser(CreateUserContract contract)
  {
    return await usersService.CreateUserAsync(contract);
  }

  public async Task<UserModel?> UpdateUser(UpdateUserContract contract)
  {
    return await usersService.UpdateUserAsync(contract);
  }

  public async Task<UserModel?> DeleteUser(DeleteUserContract contract)
  {
    return await usersService.DeleteUserAsync(contract);
  }

  public void Dispose()
  {
    disposables.ForEach(disposable => disposable.Dispose());
    GC.SuppressFinalize(this);
  }
}