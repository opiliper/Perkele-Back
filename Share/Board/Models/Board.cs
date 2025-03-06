using UserBoard.Models;

namespace Board.Models;

public class BoardModel
{
  public uint Id { get; set; }
  public string Name { get; set; }
  public List<TicketModel> Tickets { get; set; }
  public virtual ICollection<UserBoardModel> UserBoards { get; set; } = [];
}