using UserBoard.Models;

namespace Users.Models;

public class UserModel
{
  public uint Id { get; set; }
  public string Name { get; set; }
  public string Email { get; set; }
  public string? Password_Hash { get; set; } = string.Empty;
  public byte[]? Image { get; set; }
  public List<UserBoardModel> HasAccessTo { get; set; } = [];
  public List<UserBoardRequestModel> AccessRequests { get; set; } = [];
  public DateTime Created_At { get; set; } = DateTime.Now.ToUniversalTime();
  public DateTime Updated_At { get; set; }
}