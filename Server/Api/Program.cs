using api;
using Api.Security;
using Api.Services;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mqtt.Controllers;
using StateleSSE.AspNetCore;
using StateleSSE.AspNetCore.Extensions;

var builder = WebApplication.CreateBuilder(args);

var appOptions = builder.Services.AddAppOptions(builder.Configuration);



builder.Services.AddCors();

builder.Services.AddDbContext<MyDbContext>((sp, options) =>
{
    options.UseNpgsql(appOptions.DBConnectionString, npgsqlOptions => npgsqlOptions.EnableRetryOnFailure());
    options.AddEfRealtimeInterceptor(sp);
});

builder.Services.AddMqttControllers();
builder.Services.AddControllers();
builder.Services.AddInMemorySseBackplane();
builder.Services.AddEfRealtime();
builder.Services.AddOpenApiDocument();
builder.Services.AddScoped<CommandHistoryService>();
builder.Services.AddScoped<IPasswordHasher<User>, KonciousArgon2idPasswordHasher>();

var app = builder.Build();
app.UseCors(config => config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
await app.GenerateApiClientsFromOpenApi("/../../client/src/generated-ts-client.ts");

var mqtt = app.Services.GetRequiredService<IMqttClientService>();
await mqtt.ConnectAsync("broker.hivemq.com", 8883, useTls:true);


app.Run();
