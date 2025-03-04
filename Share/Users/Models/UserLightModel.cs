namespace Users.Models;

// нужна для создания хешей паролей смотри использование в UsersService.cs
public class UserLightModel
{
  public required uint Id { get; set; }
  public required DateTime Created_At { get; set; }
  public required DateTime Updated_At { get; set; }
  public static explicit operator UserLightModel(UserModel user)
  {
    return new() { Created_At = user.Created_At, Id = user.Id, Updated_At = user.Updated_At };
  }
}