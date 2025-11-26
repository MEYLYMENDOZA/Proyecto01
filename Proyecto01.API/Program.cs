using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Infrastructure.Data;
using Proyecto01.CORE.Core.Interfaces;
using Proyecto01.CORE.Core.Services;
using Proyecto01.CORE.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var _config = builder.Configuration;
var cnx = _config.GetConnectionString("DefaultConnection");

// Configuración DbContext para SQL Server
builder.Services.AddDbContext<Proyecto01DbContext>(options =>
    options.UseSqlServer(cnx));

// Configuración de CORS para permitir conexiones desde Android
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAndroid", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Registro de Repositorios
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IPersonalRepository, PersonalRepository>();
builder.Services.AddScoped<ISolicitudRepository, SolicitudRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IAlertaRepository, AlertaRepository>();
builder.Services.AddScoped<IConfigSlaRepository, ConfigSlaRepository>();
builder.Services.AddScoped<IPermisoRepository, PermisoRepository>();
builder.Services.AddScoped<IReporteRepository, ReporteRepository>();
builder.Services.AddScoped<IRolRegistroRepository, RolRegistroRepository>();
builder.Services.AddScoped<IRolesSistemaRepository, RolesSistemaRepository>();
builder.Services.AddScoped<IEstadoAlertaCatalogoRepository, EstadoAlertaCatalogoRepository>();
builder.Services.AddScoped<IEstadoSolicitudCatalogoRepository, EstadoSolicitudCatalogoRepository>();
builder.Services.AddScoped<IEstadoUsuarioCatalogoRepository, EstadoUsuarioCatalogoRepository>();
builder.Services.AddScoped<ITipoAlertaCatalogoRepository, TipoAlertaCatalogoRepository>();
builder.Services.AddScoped<ITipoSolicitudCatalogoRepository, TipoSolicitudCatalogoRepository>();

// Registro de Servicios
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IPersonalService, PersonalService>();
builder.Services.AddScoped<ISolicitudService, SolicitudService>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IAlertaService, AlertaService>();
builder.Services.AddScoped<IConfigSlaService, ConfigSlaService>();
builder.Services.AddScoped<IPermisoService, PermisoService>();
builder.Services.AddScoped<IReporteService, ReporteService>();
builder.Services.AddScoped<IRolRegistroService, RolRegistroService>();
builder.Services.AddScoped<IRolesSistemaService, RolesSistemaService>();
builder.Services.AddScoped<IEstadoAlertaCatalogoService, EstadoAlertaCatalogoService>();
builder.Services.AddScoped<IEstadoSolicitudCatalogoService, EstadoSolicitudCatalogoService>();
builder.Services.AddScoped<IEstadoUsuarioCatalogoService, EstadoUsuarioCatalogoService>();
builder.Services.AddScoped<ITipoAlertaCatalogoService, TipoAlertaCatalogoService>();
builder.Services.AddScoped<ITipoSolicitudCatalogoService, TipoSolicitudCatalogoService>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Habilitar CORS para permitir conexiones desde Android
app.UseCors("AllowAndroid");

// NO usar redirección HTTPS en desarrollo - comentado para permitir HTTP desde Android
// IMPORTANTE: Descomenta esta línea en producción
// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();