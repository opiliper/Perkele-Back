using System.Text.Json;
using System.Text.Json.Serialization;
using Board.Contracts;
using Board.DTOs;
using Board.Models;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gateway.Controllers;

[ApiController]
[Authorize]
[Route("api/v1_0/Board")]
public class BoardController(IBus _bus) : ControllerBase
{
  private readonly IBus bus = _bus;
  private readonly JsonSerializerOptions json_options = new () { ReferenceHandler = ReferenceHandler.Preserve };

  [HttpGet("{id}")]
  public async Task<ActionResult<BoardModel?>> GetBoard(uint id)
  {
    var board = await bus.Rpc.RequestAsync<BoardGetContract, BoardModel?>(new(id));
    if (board == null) {
      return NotFound();
    }
    return Ok(new JsonResult(board, json_options).Value);
  }

  [HttpPost]
  public async Task<ActionResult<BoardModel?>> CreateBoard([FromBody] BoardAddDTO boardAddDTO)
  {
    var board = await bus.Rpc.RequestAsync<BoardCreateContract, BoardModel?>(new(boardAddDTO));
    if (board == null) {
      return BadRequest();
    }
    return Ok(new JsonResult(board, json_options).Value);
  }

  [HttpPut("{id}")]
  public async Task<ActionResult<BoardModel?>> UpdateBoard(uint id, [FromBody] BoardUpdateDTO boardUpdateDTO)
  {
    var board = await bus.Rpc.RequestAsync<BoardUpdateContract, BoardModel?>(new(id, boardUpdateDTO));
    if (board == null) {
      return NotFound();
    }
    return Ok(new JsonResult(board, json_options).Value);
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult<BoardModel?>> DeleteBoard(uint id)
  {
    var board = await bus.Rpc.RequestAsync<BoardDeleteContract, BoardModel?>(new(id));
    if (board == null) {
      return NotFound();
    }
    return Ok(new JsonResult(board, json_options).Value);
  }
}