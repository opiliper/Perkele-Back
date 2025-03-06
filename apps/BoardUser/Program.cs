using System.Text.Json.Serialization;
using BoardUser;
using BoardUser.Services;
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
builder.Services.AddTransient<UserBoardRequestsService>();
builder.Services.AddSingleton<UserBoardRMQController>();
builder.Services.AddSingleton<UserBoardRequestRMQController>();
var app = builder.Build();
var userBoardRMQController = app.Services.GetRequiredService<UserBoardRMQController>();
var userBoardRequestRMQController = app.Services.GetRequiredService<UserBoardRequestRMQController>();

app.Run();