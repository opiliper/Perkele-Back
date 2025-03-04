using Users.DTOs;

namespace Users.Contracts;

public record UpdateUserContract(uint Id, UpdateUserDTO DTO);