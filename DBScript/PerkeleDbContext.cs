using System;
using System.Collections.Generic;
using Board.Models;
using Microsoft.EntityFrameworkCore;
using UserBoard.Models;
using Users.Models;

namespace DBScript;

public partial class PerkeleDbContext : DbContext
{
  public PerkeleDbContext(DbContextOptions options) : base(options)
  {
    Database.EnsureDeleted();
    Database.EnsureCreated();
  }
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
