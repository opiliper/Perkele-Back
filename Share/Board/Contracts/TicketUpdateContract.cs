using Board.DTOs;

namespace Board.Contracts;

public record TicketUpdateContract(uint Id, TicketUpdateDTO DTO);