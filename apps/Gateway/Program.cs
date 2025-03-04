using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using EasyNetQ;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddSingleton(_ =>
  RabbitHutch.CreateBus(builder.Configuration.GetConnectionString("EasyNetQ"),
  x => x.EnableSystemTextJson(new() { ReferenceHandler = ReferenceHandler.IgnoreCycles })));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
      opts.TokenValidationParameters = new()
      {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,
        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
        ValidateLifetime = true
      };
    }
);
builder.Services.AddAuthorization();
using var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public static class AuthOptions
{
  public const string ISSUER = "Issuer"; // издатель токена
  public const string AUDIENCE = "Audience"; // потребитель токена
  const string KEY = "SomeSuperSecretKeySomeSuperSecretKeySomeSuperSecretKeySomeSuperSecretKey";   // ключ для шифрации
  public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
      new(Encoding.UTF8.GetBytes(KEY));
}