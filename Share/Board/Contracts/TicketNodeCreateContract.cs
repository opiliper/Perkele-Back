using Board.DTOs;

namespace Board.Contracts;

public record TicketNodeCreateContract(uint TicketId, TicketNodeAddDTO DTO);