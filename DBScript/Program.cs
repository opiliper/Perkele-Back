using Board.Models;
using Users.Models;
using Microsoft.EntityFrameworkCore;
using UserBoard.Models;

namespace DBScript;

public class Program
{
  public static void Main()
  {
    var options = new DbContextOptionsBuilder<CoolDBContext>()
      .UseNpgsql("Server=localhost;Port=5432;User Id=opiliper;Password=123;Database=Perkele-DB;")
      .Options;
    using var db = new CoolDBContext(options);
  }
}

public class CoolDBContext : DbContext
{
  public DbSet<BoardModel> Boards { get; set; } = null!;
  public DbSet<TicketModel> Tickets { get; set; } = null!;
  public DbSet<TicketNodeModel> TicketNodes { get; set; } = null!;
  public DbSet<UserModel> Users { get; set; } = null!;
  public DbSet<UserBoardModel> UserBoardModels = null!;
  public CoolDBContext(DbContextOptions<CoolDBContext> options) : base(options)
  {
    Database.EnsureDeleted();
    Database.EnsureCreated();
  }
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TicketNodeModel>().HasKey(el => new { el.Key, el.TicketId });
    modelBuilder.Entity<UserBoardModel>().HasKey(obj => new { obj.UserId, obj.BoardId } );
    modelBuilder.Entity<UserModel>().HasAlternateKey(el => el.Email);
  }
}