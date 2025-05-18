using System.Text.Json.Serialization;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Ticket;
using Ticket.Controllers;
using Ticket.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TicketDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")),
   ServiceLifetime.Transient,
   ServiceLifetime.Transient);
builder.Services.AddTransient<TicketService>();
builder.Services.AddSingleton<TicketRMQController>();
builder.Services.AddSingleton(
   x => RabbitHutch.CreateBus(
      builder.Configuration.GetConnectionString("EasyNetQ"),
      x => x.EnableSystemTextJson(new() { ReferenceHandler = ReferenceHandler.IgnoreCycles })
   )
); // EasyNetQ

var app = builder.Build();
var a = app.Services.GetRequiredService<TicketRMQController>();

app.Run();
