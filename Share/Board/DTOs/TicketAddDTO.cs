namespace Board.DTOs;

public class TicketAddDTO
{
  public uint Board_id { get; set; }
  public string? Title { get; set; }
  public string Value { get; set; }
  public List<TicketNodeAddDTO>? Nodes { get; set; }
}