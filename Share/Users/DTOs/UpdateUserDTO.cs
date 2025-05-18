using System.ComponentModel.DataAnnotations;

namespace Users.DTOs;

public class UpdateUserDTO
{
  [Required(ErrorMessage = "Имя необходимо")]
  public string? Name { get; set; }
  [Required(ErrorMessage = "Почта необходима")]
  [EmailAddress(ErrorMessage = "Невалидная почта")]
  public string? Email { get; set; }
  [Required(ErrorMessage = "Пароль необходим")]
  public string? Password { get; set; }
  [MaxLength(1 * 1024 * 1024, ErrorMessage = "Слишком большой размер изображения")]
  public byte[]? Image { get; set; } = null;

  public UpdateUserDTO(
    string? name,
    string? email,
    string? password,
    byte[]? image
  )
  {
    Name = name;
    Email = email;
    Password = password;
    Image = image;
  }
  public UpdateUserDTO()
  {

  }
}