using Microsoft.EntityFrameworkCore;
using UserBoard.Models;

namespace BoardUser;

public class UserBoardDBContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<UserBoardModel> UserBoards = null!;
}