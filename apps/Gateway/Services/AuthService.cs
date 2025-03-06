using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Auth.Contracts;
using EasyNetQ;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Users.Contracts;
using Users.Models;

using json_serializer = System.Text.Json.JsonSerializer;

namespace Gateway.Services;

public class AuthService(IBus _bus)
{
  private readonly IBus bus = _bus;
  public async Task<string> Login(LoginContract contract)
  {
    UserModel? user = await bus.Rpc.RequestAsync<GetUserContract, UserModel?>(new(Email: contract.Email));
    if (user == null)
      return LoginResults.USER_NOT_FOUND;

    if (!IsPasswordRight(user, contract.Password))
      return LoginResults.WRONG_PASSWORD;

    return CreateJWT(user);
  }
  public bool IsPasswordRight(UserModel user, string password)
  {
    bool result = false;

    PasswordHasher<UserLightModel> hasher = new();
    var hash_check = hasher.VerifyHashedPassword((UserLightModel)user, user.Password_Hash!, password);
    if (hash_check != PasswordVerificationResult.Failed)
    {
      result = true;
    }
    return result;
  }

  public string CreateJWT(UserModel user)
  {
    List<Claim> claims = [
      new("Id", Convert.ToString(user.Id))
    ];
    // создаем JWT-токен
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

    return new JwtSecurityTokenHandler().WriteToken(jwt);
  }

  public async Task<RegisterResult?> Register(CreateUserContract contract) {
    var user = await bus.Rpc.RequestAsync<CreateUserContract, UserModel?>(contract);
    if (user == null) return null;

    return new() { User = user, Jwt = CreateJWT(user) };
  }
}

public static class LoginResults
{
  public const string USER_NOT_FOUND = "No Such User Idiot";
  public const string WRONG_PASSWORD = "Wrong Password Bruh";
}

public class RegisterResult
{
  public required UserModel User { get; set; }
  public required string Jwt { get; set; }
}