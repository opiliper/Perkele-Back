namespace Users.DTOs;

public record CreateUserDTO
(
  string? Name,
  string Email,
  string Password,
  byte[]? Image
);
