using Microsoft.EntityFrameworkCore;
using Users.Models;

namespace Users;

public class UsersDBContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<UserModel> Users { get; set; } = null!;
}