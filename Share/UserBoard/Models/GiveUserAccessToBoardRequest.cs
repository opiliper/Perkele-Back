using Board.Models;
using Users.Models;

namespace UserBoard.Models;

public class UserBoardRequestModel
{
  public uint UserId { get; set; }
  public virtual UserModel User { get; set; }
  public uint BoardId { get; set; }
  public virtual BoardModel Board { get; set; }
  public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}