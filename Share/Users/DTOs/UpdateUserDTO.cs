namespace Users.DTOs;

public record UpdateUserDTO
(
  string? Name,
  string? Email,
  string? Password,
  byte[]? Image
);