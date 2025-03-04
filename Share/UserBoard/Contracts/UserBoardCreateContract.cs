using UserBoard.Enums;

namespace UserBoard.Contracts;

public record UserBoardCreateContract(
  uint BoardId,
  uint UserId,
  UserBoardRoleEnum Role
);