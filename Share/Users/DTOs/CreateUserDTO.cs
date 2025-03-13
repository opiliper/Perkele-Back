namespace Users.DTOs;
using System.ComponentModel.DataAnnotations;

public class CreateUserDTO
{
  [Required(ErrorMessage = "Имя должно присутствовать")]
  public string Name { get; set; }
  [Required(ErrorMessage = "Почта должна присутствовать")]
  [EmailAddress(ErrorMessage = "Невалидная почта")]
  public string Email { get; set; }
  [Required(ErrorMessage = "Пароль должен присутствовать")]
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
