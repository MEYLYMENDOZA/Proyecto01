using Proyecto01.API; // Para tu Contexto
using Microsoft.EntityFrameworkCore; // Para el método UseSqlServer


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var _config = builder.Configuration;
var cnx = _config.GetConnectionString("DefaultConnection");

// ✅ Línea de configuración CORREGIDA para Entity Framework Core
builder.Services.AddDbContext<Proyecto01Context>(options =>
    options.UseSqlServer(cnx));
// -------------------------------------------------------------

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();