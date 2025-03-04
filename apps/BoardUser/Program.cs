using System.Text.Json.Serialization;
using BoardUser;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using UserBoard.Controllers;
using UserBoard.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<UserBoardDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")),
  ServiceLifetime.Transient,
  ServiceLifetime.Transient);
builder.Services.AddSingleton(_ =>
  RabbitHutch.CreateBus(builder.Configuration.GetConnectionString("EasyNetQ"),
  x => x.EnableSystemTextJson(new () { ReferenceHandler = ReferenceHandler.IgnoreCycles }))
);
builder.Services.AddTransient<UserBoardService>();
builder.Services.AddSingleton<UserBoardRMQController>();
var app = builder.Build();
var rmqController = app.Services.GetRequiredService<UserBoardRMQController>();

app.Run();