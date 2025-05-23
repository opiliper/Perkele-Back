using Board.Models;

namespace Ticket.Models;

public class TicketModel
{
   public uint Id { get; set; }
   public string? Title { get; set; }
   public string Value { get; set; }
   public string Column { get; set; }
   public BoardModel Board { get; set; }
   public uint BoardId { get; set; }
   public List<TicketNodeModel> Nodes { get; set; }
}