namespace Board.Models;

public class BoardModel
{
  public uint Id { get; set; }
  public string Name { get; set; }
  public List<TicketModel> Tickets { get; set; }
}