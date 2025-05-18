using Users.Models;

namespace Auth.Results;
public class RegisterResult
{
   public required UserModel User { get; set; }
   public required string Jwt { get; set; }
}