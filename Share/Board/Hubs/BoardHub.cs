using Board.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Board.Hubs;

[Authorize]
public class BoardHub : Hub
{
   public override async Task OnConnectedAsync()
   {
      var boardId = Context.GetHttpContext().Request.Query["BoardId"];
      await Groups.AddToGroupAsync(Context.ConnectionId, $"board-{boardId}");
      await base.OnConnectedAsync();
   }

   public async Task CreateTicket(TicketModel ticket)
   {
      await Clients.OthersInGroup($"board-{ticket.BoardId}").SendAsync("TicketCreate", ticket);
   }

   public async Task DeleteTicket(TicketModel ticket)
   {
      await Clients.OthersInGroup($"board-{ticket.BoardId}").SendAsync("TicketDelete", ticket);
   }

   public async Task ChangeColumn(TicketModel ticket)
   {
      await Clients.OthersInGroup($"board-{ticket.BoardId}").SendAsync("ColumnChange", ticket);
   }
}