using Board.Models;
using Microsoft.EntityFrameworkCore;
using UserBoard.Models;
using Users.Models;

namespace BoardUser;

public class UserBoardDBContext(DbContextOptions options) : PerkeleDBContext(options)
{
}