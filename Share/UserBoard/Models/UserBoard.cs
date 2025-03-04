using Board.Models;
using UserBoard.Enums;
using Users.Models;

namespace UserBoard.Models;

public class UserBoardModel
{
  public uint BoardId { get; set; }
  public BoardModel Board { get; set; }
  public uint UserId { get; set; }
  public UserModel Model { get; set; }
  public UserBoardRoleEnum Role { get; set; }
}