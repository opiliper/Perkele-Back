namespace Ticket.Models;

public class TicketNodeModel
{
  public TicketModel Ticket { get; set; }
  public uint TicketId { get; set; }
  public string Key { get; set; }
  public string Value { get; set; }
}