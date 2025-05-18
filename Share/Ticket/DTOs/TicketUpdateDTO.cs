namespace Ticket.DTOs;

public class TicketUpdateDTO
{
  public string? Title { get; set; }
  public string? Value { get; set; }
  public string? Column { get; set; }
  public List<TicketNodeUpdateDTO>? Nodes { get; set; }
}