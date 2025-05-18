using Microsoft.EntityFrameworkCore;

namespace Ticket;

public class TicketDBContext(DbContextOptions<TicketDBContext> options) : PerkeleDBContext(options)
{
}