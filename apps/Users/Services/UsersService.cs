using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Users.Contracts;
using Users.Models;

namespace Users.Services;

public class UsersService(UsersDBContext usersDB)
{
  private readonly UsersDBContext DB = usersDB;
  public async Task<UserModel> CreateUserAsync(CreateUserContract contract)
  {
    PasswordHasher<UserLightModel> hasher = new();
    var user = DB.Users.Add(new()
    {
      Name = contract.DTO.Name ?? contract.DTO.Email,
      Email = contract.DTO.Email,
      Image = contract.DTO.Image,
      Created_At = DateTime.Now.ToUniversalTime(),
      Updated_At = DateTime.Now.ToUniversalTime(),
    });

    await DB.SaveChangesAsync();

    user.Entity.Password_Hash = hasher.HashPassword((UserLightModel)user.Entity, contract.DTO.Password);

    DB.Users.Update(user.Entity);
    await DB.SaveChangesAsync();
    return user.Entity;
  }

  public async Task<UserModel?> GetUserByIdAsync(GetUserContract contract)
  {
    var res = await DB.Users.FirstOrDefaultAsync(el => el.Id == contract.Id || el.Email == contract.Email);
    if (res is null || res.Password_Hash == null)
      return null;
    return res;
  }

  public async Task<UserModel?> UpdateUserAsync(UpdateUserContract contract)
  {
    var user = await DB.Users.FindAsync(contract.Id);
    if (user == null)
    {
      return null;
    }

    PasswordHasher<UserLightModel> passwordHasher = new();
    user.Name = contract.DTO.Name ?? user.Name;
    user.Email = contract.DTO.Email ?? user.Email;
    user.Updated_At = DateTime.Now.ToUniversalTime();
    if (contract.DTO.Password != null)
      user.Password_Hash = passwordHasher.HashPassword((UserLightModel)user, contract.DTO.Password);
    user.Image = contract.DTO.Image ?? user.Image;

    DB.Users.Update(user);
    await DB.SaveChangesAsync();
    return user;
  }

  public async Task<UserModel?> DeleteUserAsync(DeleteUserContract contract)
  {
    var user = await DB.Users.FindAsync(contract.Id);
    if (user == null) {
      return null;
    }

    DB.Users.Remove(user);
    await DB.SaveChangesAsync();
    return user;
  }
}