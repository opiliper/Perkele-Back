using Board.Models;
using Microsoft.EntityFrameworkCore;
using UserBoard.Models;
using Users.Models;
using Ticket.Models;

public class PerkeleDBContext(DbContextOptions options) : DbContext(options)
{
  public DbSet<BoardModel> Boards { get; set; } = null!;
  public DbSet<TicketModel> Tickets { get; set; } = null!;
  public DbSet<TicketNodeModel> TicketNodes { get; set; } = null!;
  public DbSet<UserModel> Users { get; set; } = null!;
  public DbSet<UserBoardModel> UserBoards { get; set; } = null!;
  public DbSet<UserBoardRequestModel> UserBoardRequests { get; set; } = null!;
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    modelBuilder.Entity<TicketNodeModel>().HasKey(el => new { el.Key, el.TicketId });
    modelBuilder.Entity<UserModel>().HasIndex(el => el.Email).IsUnique();
    modelBuilder.Entity<UserBoardModel>().HasKey(e => new { e.UserId, e.BoardId });
    modelBuilder.Entity<UserBoardRequestModel>().HasKey(el => new { el.UserId, el.BoardId });
  }
}