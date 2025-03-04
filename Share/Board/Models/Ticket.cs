namespace Board.Models;

public class TicketModel
{
  public uint Id { get; set; }
  public BoardModel Board { get; set; }
  public uint BoardId { get; set; }
  public List<TicketNodeModel> Nodes { get; set; }
}