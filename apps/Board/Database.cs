using Microsoft.EntityFrameworkCore;
using Board.Models;
using UserBoard.Models;
using Users.Models;

namespace Board;

public class BoardDBContext(DbContextOptions<BoardDBContext> options) : PerkeleDBContext(options)
{
}