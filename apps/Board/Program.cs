using System.Text.Json.Serialization;
using Board;
using Board.Controllers;
using Board.Services;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BoardDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")),
  ServiceLifetime.Transient,
  ServiceLifetime.Transient);
builder.Services.AddTransient<BoardService>();
builder.Services.AddTransient<TicketService>();
builder.Services.AddSingleton<BoardRMQController>();
builder.Services.AddSingleton<TicketRMQController>();
builder.Services.AddSingleton(_ =>
  RabbitHutch.CreateBus(builder.Configuration.GetConnectionString("EasyNetQ"),
  x => x.EnableSystemTextJson(new () { ReferenceHandler = ReferenceHandler.IgnoreCycles }))
);
using var app = builder.Build();
app.Services.GetRequiredService<BoardRMQController>();
app.Services.GetRequiredService<TicketRMQController>();

app.Run("http://localhost:5252/");