using UserBoard.Enums;

namespace UserBoard.DTOs;

public record class UserBoardDTO(uint BoardId, uint UserId, UserBoardRoleEnum? UserBoardRole);