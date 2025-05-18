using System.Text;
using System.Text.Json.Serialization;
using Board.Hubs;
using EasyNetQ;
using Gateway.Services;
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
      opts.Events = new JwtBearerEvents
      {
        OnMessageReceived = context =>
        {
          var accessToken = context.Request.Query["access_token"];

          // если запрос направлен хабу
          var path = context.HttpContext.Request.Path;
          if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/api/v1_0/Board/Hub"))
          {
            // получаем токен из строки запроса
            context.Token = accessToken;
          }
          return Task.CompletedTask;
        }
      };
    }
);
builder.Services.AddAuthorization();
builder.Services.AddTransient<AuthService>();
builder.Services.AddSignalR();
using var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<BoardHub>("/api/v1_0/Board/Hub");

app.Run();

public static class AuthOptions
{
  public const string ISSUER = "Issuer"; // издатель токена
  public const string AUDIENCE = "Audience"; // потребитель токена
  const string KEY = "SomeSuperSecretKeySomeSuperSecretKeySomeSuperSecretKeySomeSuperSecretKey";   // ключ для шифрации
  public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
      new(Encoding.UTF8.GetBytes(KEY));
}