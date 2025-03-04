using Microsoft.EntityFrameworkCore;
using Board.Models;

namespace Board;

public class BoardDBContext(DbContextOptions<BoardDBContext> options) : DbContext(options)
{
  public DbSet<BoardModel> Boards { get; set; } = null!;
  public DbSet<TicketModel> Tickets { get; set; } = null!;
  public DbSet<TicketNodeModel> TicketNodes { get; set; } = null!;
}