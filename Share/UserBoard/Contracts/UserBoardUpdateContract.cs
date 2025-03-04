using UserBoard.Enums;

namespace UserBoard.Contracts;

public record UserBoardUpdateContract(uint BoardId, uint UserId, UserBoardRoleEnum? Role);