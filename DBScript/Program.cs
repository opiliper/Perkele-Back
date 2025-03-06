using Microsoft.EntityFrameworkCore;

namespace DBScript;

public class Program
{
  public static void Main()
  {
    var options = new DbContextOptionsBuilder<PerkeleDBContext>()
      .UseNpgsql("Server=localhost;Port=5432;User Id=opiliper;Password=123;Database=Perkele-DB;")
      .Options;
    using var db = new PerkeleDBContext(options);
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
  }
}