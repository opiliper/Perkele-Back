namespace Board.DTOs;

public class TicketAddDTO
{
  public uint Board_id { get; set; }
  public List<TicketNodeAddDTO>? Nodes { get; set; }
}