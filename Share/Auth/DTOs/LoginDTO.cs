using System.ComponentModel.DataAnnotations;

namespace Auth.DTOs;

public class LoginDTO
{
  [Required(ErrorMessage = "Почта должна присутствовать")]
  [EmailAddress(ErrorMessage = "Невалидная почта")]
  public string Email { get; set; } = null!;
  [Required(ErrorMessage = "Пароль должен присутствовать")]
  public string Password { get; set; } = null!;
}