using Ticket.DTOs;

namespace Ticket.Contracts;

public record TicketUpdateContract(uint Id, TicketUpdateDTO DTO);