using Microsoft.EntityFrameworkCore;
using Board.Models;
using UserBoard.Models;
using Users.Models;

namespace Board;

public class BoardDBContext(DbContextOptions<BoardDBContext> options) : DbContext(options)
{
  public DbSet<BoardModel> Boards { get; set; } = null!;
  public DbSet<TicketModel> Tickets { get; set; } = null!;
  public DbSet<TicketNodeModel> TicketNodes { get; set; } = null!;
  public DbSet<UserModel> Users { get; set; } = null!;
  public DbSet<UserBoardModel> UserBoards { get; set; } = null!;
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TicketNodeModel>().HasKey(el => new { el.Key, el.TicketId });
    modelBuilder.Entity<UserModel>().HasAlternateKey(el => el.Email);
    modelBuilder.Entity<UserBoardModel>().HasKey(e => new { e.UserId, e.BoardId });
  }
}