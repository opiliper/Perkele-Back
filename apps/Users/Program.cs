using System.Text.Json.Serialization;
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using Users;
using Users.Controllers;
using Users.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UsersDBContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")), 
  ServiceLifetime.Transient,
  ServiceLifetime.Transient); // PostgreSQL
builder.Services.AddTransient<UsersService>();
builder.Services.AddSingleton(
  x => RabbitHutch.CreateBus(
    builder.Configuration.GetConnectionString("EasyNetQ"),
    x => x.EnableSystemTextJson(new () { ReferenceHandler = ReferenceHandler.IgnoreCycles })
  )
); // EasyNetQ
builder.Services.AddSingleton<UsersRMQController>();

using var app = builder.Build();
using var usersRmq = app.Services.GetRequiredService<UsersRMQController>();

app.Run();