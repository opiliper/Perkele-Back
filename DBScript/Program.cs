using Board.Models;
using Users.Models;
using Microsoft.EntityFrameworkCore;
using UserBoard.Models;

namespace DBScript;

public class Program
{
  public static void Main()
  {
    var options = new DbContextOptionsBuilder<PerkeleDbContext>()
      .UseNpgsql("Server=localhost;Port=5432;User Id=opiliper;Password=123;Database=Perkele-DB;")
      .Options;
    using var db = new PerkeleDbContext(options);
  }
}