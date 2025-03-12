namespace Users.DTOs;

public class CreateUserDTO
{
  public string Name { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
  public byte[]? Image { get; set; }
  public CreateUserDTO (
    string name,
    string email,
    string password,
    byte[]? image
  )
  {
    Name = name;
    Email = email;
    Password = password;
    Image = image;
  }

  public CreateUserDTO() {}
}
