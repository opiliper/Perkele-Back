using System.Text.Json;
using System.Text.Json.Serialization;
using Board.Contracts;
using Board.DTOs;
using Board.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Users.Contracts;
using Users.Models;

namespace Gateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1_0/Ticket")]
public class TicketController(IBus _bus) : ControllerBase
{
  private readonly IBus bus = _bus;
  private readonly JsonSerializerOptions json_options = new () { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpGet("{id}")]
  public async Task<ActionResult<TicketModel?>> GetTicket(uint id)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return Forbid();

    var ticket = await bus.Rpc.RequestAsync<TicketGetContract, TicketModel?>(new(id));
    if (ticket == null) {
      return NotFound();
    }
    return Ok(new JsonResult(ticket, json_options).Value);
  }

  [HttpPost]
  public async Task<ActionResult<TicketModel?>> CreateTicket([FromBody] TicketAddDTO ticketAddDTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return Forbid();

    TicketModel? ticket = await bus.Rpc.RequestAsync<TicketCreateContract, TicketModel?>(new(ticketAddDTO));
    if (ticket == null) {
      return BadRequest();
    }
    return Ok(new JsonResult(ticket, json_options).Value);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<TicketModel?>> UpdateTicket(uint id, [FromBody] TicketUpdateDTO ticketUpdateDTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return Forbid();

    var ticket = await bus.Rpc.RequestAsync<TicketUpdateContract, TicketModel?>(new(id, ticketUpdateDTO));
    if (ticket == null) {
      return NotFound();
    }
    return Ok(new JsonResult(ticket, json_options).Value);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<TicketModel?>> DeleteTicket(uint id)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return Forbid();

    var ticket = await bus.Rpc.RequestAsync<TicketDeleteContract, TicketModel?>(new(id));
    if (ticket == null) {
      return NotFound();
    }
    return Ok(new JsonResult(ticket, json_options).Value);
  }

  [HttpPost("{id}/nodes")]
  public async Task<ActionResult<TicketNodeModel?>> AddNode(uint id, [FromBody] TicketNodeAddDTO ticketNodeAddDTO)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return Forbid();

    var node = await bus.Rpc.RequestAsync<TicketNodeCreateContract, TicketNodeModel?>(new(id, ticketNodeAddDTO));
    if (node == null) {
      return BadRequest();
    }
    return Ok(new JsonResult(node, json_options).Value);
  }

  [HttpDelete("{id}/nodes/{key}")]
  public async Task<ActionResult<TicketNodeModel?>> DeleteNode(uint id, string key)
  {
    var ctx_user_id = Convert.ToUInt32(User.FindFirst("Id")!.Value);
    var user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Id: ctx_user_id));
    if (user == null) 
      return Forbid();

    var node = await bus.Rpc.RequestAsync<TicketNodeDeleteContract, TicketNodeModel?>(new(id, key));
    if (node == null) {
      return NotFound();
    }
    return Ok(new JsonResult(node, json_options).Value);
  }
}