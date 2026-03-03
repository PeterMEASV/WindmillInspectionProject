using api;
using DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var appOptions = builder.Services.AddAppOptions(builder.Configuration);

builder.Services.AddCors();
builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DBConnectionString);
});
builder.Services.AddControllers();


var app = builder.Build();
app.UseCors(config => config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


app.Run();
