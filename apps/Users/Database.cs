using Board.Models;
using Microsoft.EntityFrameworkCore;
using UserBoard.Models;
using Users.Models;

namespace Users;

public class UsersDBContext(DbContextOptions options) : PerkeleDBContext(options)
{
}