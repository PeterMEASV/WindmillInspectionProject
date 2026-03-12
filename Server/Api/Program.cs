using api;
using Api.Security;
using Api.Services;
using Api.Services.Classes;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
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
builder.Services.AddScoped<ITelemetryService, TelemetryService>();
builder.Services.AddControllers();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, JwtService>();
builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = JwtService.ValidationParameters(
            builder.Configuration
        );
        // Add this for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("Token validated successfully");
                return Task.CompletedTask;
            },
        };
    });
builder.Services.AddAuthorization();


var app = builder.Build();

app.UseCors(config => config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();

await app.GenerateApiClientsFromOpenApi("/../../client/src/generated-ts-client.ts");

var mqtt = app.Services.GetRequiredService<IMqttClientService>();
await mqtt.ConnectAsync("broker.hivemq.com", 8883, useTls:true);

app.Run();