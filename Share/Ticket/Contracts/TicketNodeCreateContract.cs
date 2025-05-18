using Ticket.DTOs;

namespace Ticket.Contracts;

public record TicketNodeCreateContract(uint TicketId, TicketNodeAddDTO DTO);