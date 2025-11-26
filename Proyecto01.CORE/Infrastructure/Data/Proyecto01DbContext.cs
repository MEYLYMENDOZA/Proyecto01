using Microsoft.EntityFrameworkCore;
using Proyecto01.CORE.Core.Entities;

namespace Proyecto01.CORE.Infrastructure.Data
{
    public class Proyecto01DbContext : DbContext
    {
        public Proyecto01DbContext(DbContextOptions<Proyecto01DbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<Area> Areas { get; set; }
        public DbSet<Personal> Personales { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<RolesSistema> RolesSistema { get; set; }
        public DbSet<EstadoUsuarioCatalogo> EstadosUsuario { get; set; }
        public DbSet<EstadoSolicitudCatalogo> EstadosSolicitud { get; set; }
        public DbSet<ConfigSla> ConfigSlas { get; set; }
        public DbSet<TipoSolicitudCatalogo> TiposSolicitud { get; set; }
        public DbSet<RolRegistro> RolesRegistro { get; set; }
        public DbSet<Alerta> Alertas { get; set; }
        public DbSet<TipoAlertaCatalogo> TiposAlerta { get; set; }
        public DbSet<EstadoAlertaCatalogo> EstadosAlerta { get; set; }
        public DbSet<Reporte> Reportes { get; set; }
        public DbSet<ReporteDetalle> ReporteDetalles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolPermisos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Area
            modelBuilder.Entity<Area>(entity =>
            {
                entity.ToTable("area");
                entity.HasKey(e => e.IdArea);
                entity.Property(e => e.IdArea).HasColumnName("id_area");
                entity.Property(e => e.NombreArea).IsRequired().HasMaxLength(100).HasColumnName("nombre_area");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
            });

            // Configuración de Personal
            modelBuilder.Entity<Personal>(entity =>
            {
                entity.ToTable("personal");
                entity.HasKey(e => e.IdPersonal);
                entity.Property(e => e.IdPersonal).HasColumnName("id_personal");
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Nombres).HasMaxLength(100).HasColumnName("nombres");
                entity.Property(e => e.Apellidos).HasMaxLength(100).HasColumnName("apellidos");
                entity.Property(e => e.Documento).HasMaxLength(20).HasColumnName("documento");
                entity.Property(e => e.Estado).HasMaxLength(20).HasColumnName("estado");
                entity.Property(e => e.CreadoEn).IsRequired().HasColumnName("creado_en");

                entity.HasOne(e => e.Usuario)
                    .WithOne(u => u.Personal)
                    .HasForeignKey<Personal>(e => e.IdUsuario)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("usuario");
                entity.HasKey(e => e.IdUsuario);
                entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
                entity.Property(e => e.Username).IsRequired().HasMaxLength(50).HasColumnName("username");
                entity.Property(e => e.Correo).IsRequired().HasMaxLength(100).HasColumnName("correo");
                entity.Property(e => e.PasswordHash).HasMaxLength(255).HasColumnName("password_hash");
                entity.Property(e => e.IdRolSistema).HasColumnName("id_rol_sistema");
                entity.Property(e => e.IdEstadoUsuario).HasColumnName("id_estado_usuario");
                entity.Property(e => e.CreadoEn).IsRequired().HasColumnName("creado_en");

                entity.HasOne(e => e.RolesSistema)
                    .WithMany(r => r.Usuarios)
                    .HasForeignKey(e => e.IdRolSistema)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EstadoUsuario)
                    .WithMany(e => e.Usuarios)
                    .HasForeignKey(e => e.IdEstadoUsuario)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de Solicitud
            modelBuilder.Entity<Solicitud>(entity =>
            {
                entity.ToTable("solicitud");
                entity.HasKey(e => e.IdSolicitud);
                entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
                entity.Property(e => e.IdPersonal).HasColumnName("id_personal");
                entity.Property(e => e.IdRolRegistro).HasColumnName("id_rol_registro");
                entity.Property(e => e.IdSla).HasColumnName("id_sla");
                entity.Property(e => e.IdArea).HasColumnName("id_area");
                entity.Property(e => e.IdEstadoSolicitud).HasColumnName("id_estado_solicitud");
                entity.Property(e => e.FechaSolicitud).HasColumnName("fecha_solicitud");
                entity.Property(e => e.FechaIngreso).HasColumnName("fecha_ingreso");
                entity.Property(e => e.NumDiasSla).HasColumnName("num_dias_sla");
                entity.Property(e => e.ResumenSla).HasMaxLength(500).HasColumnName("resumen_sla");
                entity.Property(e => e.OrigenDato).HasMaxLength(50).HasColumnName("origen_dato");
                entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
                entity.Property(e => e.CreadoEn).IsRequired().HasColumnName("creado_en");
                entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
                entity.Property(e => e.ActualizadoEn).HasColumnName("actualizado_en");

                entity.HasOne(e => e.Personal)
                    .WithMany(p => p.Solicitudes)
                    .HasForeignKey(e => e.IdPersonal)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.RolRegistro)
                    .WithMany(r => r.Solicitudes)
                    .HasForeignKey(e => e.IdRolRegistro)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.ConfigSla)
                    .WithMany(c => c.Solicitudes)
                    .HasForeignKey(e => e.IdSla)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Area)
                    .WithMany(a => a.Solicitudes)
                    .HasForeignKey(e => e.IdArea)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EstadoSolicitud)
                    .WithMany(e => e.Solicitudes)
                    .HasForeignKey(e => e.IdEstadoSolicitud)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UsuarioCreadoPor)
                    .WithMany(u => u.SolicitudesCreadas)
                    .HasForeignKey(e => e.CreadoPor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UsuarioActualizadoPor)
                    .WithMany(u => u.SolicitudesActualizadas)
                    .HasForeignKey(e => e.ActualizadoPor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de RolesSistema
            modelBuilder.Entity<RolesSistema>(entity =>
            {
                entity.ToTable("roles_sistema");
                entity.HasKey(e => e.IdRolSistema);
                entity.Property(e => e.IdRolSistema).HasColumnName("id_rol_sistema");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
                entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
                entity.Property(e => e.EsActivo).IsRequired().HasColumnName("es_activo");
            });

            // Configuración de EstadoUsuarioCatalogo
            modelBuilder.Entity<EstadoUsuarioCatalogo>(entity =>
            {
                entity.ToTable("estado_usuario_catalogo");
                entity.HasKey(e => e.IdEstadoUsuario);
                entity.Property(e => e.IdEstadoUsuario).HasColumnName("id_estado_usuario");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
            });

            // Configuración de EstadoSolicitudCatalogo
            modelBuilder.Entity<EstadoSolicitudCatalogo>(entity =>
            {
                entity.ToTable("estado_solicitud_catalogo");
                entity.HasKey(e => e.IdEstadoSolicitud);
                entity.Property(e => e.IdEstadoSolicitud).HasColumnName("id_estado_solicitud");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
            });

            // Configuración de ConfigSla
            modelBuilder.Entity<ConfigSla>(entity =>
            {
                entity.ToTable("config_sla");
                entity.HasKey(e => e.IdSla);
                entity.Property(e => e.IdSla).HasColumnName("id_sla");
                entity.Property(e => e.CodigoSla).IsRequired().HasMaxLength(50).HasColumnName("codigo_sla");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
                entity.Property(e => e.DiasUmbral).HasColumnName("dias_umbral");
                entity.Property(e => e.IdTipoSolicitud).HasColumnName("id_tipo_solicitud");
                entity.Property(e => e.EsActivo).IsRequired().HasColumnName("es_activo");
                entity.Property(e => e.CreadoPor).HasColumnName("creado_por");
                entity.Property(e => e.CreadoEn).IsRequired().HasColumnName("creado_en");
                entity.Property(e => e.ActualizadoPor).HasColumnName("actualizado_por");
                entity.Property(e => e.ActualizadoEn).HasColumnName("actualizado_en");

                entity.HasOne(e => e.TipoSolicitud)
                    .WithMany(t => t.ConfigSlas)
                    .HasForeignKey(e => e.IdTipoSolicitud)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UsuarioCreadoPor)
                    .WithMany(u => u.ConfigSlasCreadas)
                    .HasForeignKey(e => e.CreadoPor)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.UsuarioActualizadoPor)
                    .WithMany(u => u.ConfigSlasActualizadas)
                    .HasForeignKey(e => e.ActualizadoPor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de TipoSolicitudCatalogo
            modelBuilder.Entity<TipoSolicitudCatalogo>(entity =>
            {
                entity.ToTable("tipo_solicitud_catalogo");
                entity.HasKey(e => e.IdTipoSolicitud);
                entity.Property(e => e.IdTipoSolicitud).HasColumnName("id_tipo_solicitud");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
            });

            // Configuración de RolRegistro
            modelBuilder.Entity<RolRegistro>(entity =>
            {
                entity.ToTable("rol_registro");
                entity.HasKey(e => e.IdRolRegistro);
                entity.Property(e => e.IdRolRegistro).HasColumnName("id_rol_registro");
                entity.Property(e => e.NombreRol).IsRequired().HasMaxLength(100).HasColumnName("nombre_rol");
                entity.Property(e => e.BloqueTeach).HasMaxLength(100).HasColumnName("bloque_teach");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
                entity.Property(e => e.EsActivo).IsRequired().HasColumnName("es_activo");
            });

            // Configuración de Alerta
            modelBuilder.Entity<Alerta>(entity =>
            {
                entity.ToTable("alerta");
                entity.HasKey(e => e.IdAlerta);
                entity.Property(e => e.IdAlerta).HasColumnName("id_alerta");
                entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");
                entity.Property(e => e.IdTipoAlerta).HasColumnName("id_tipo_alerta");
                entity.Property(e => e.IdEstadoAlerta).HasColumnName("id_estado_alerta");
                entity.Property(e => e.Nivel).HasMaxLength(50).HasColumnName("nivel");
                entity.Property(e => e.Mensaje).HasMaxLength(500).HasColumnName("mensaje");
                entity.Property(e => e.FechaCreacion).IsRequired().HasColumnName("fecha_creacion");
                entity.Property(e => e.FechaLectura).HasColumnName("fecha_lectura");

                entity.HasOne(e => e.Solicitud)
                    .WithMany(s => s.Alertas)
                    .HasForeignKey(e => e.IdSolicitud)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.TipoAlerta)
                    .WithMany(t => t.Alertas)
                    .HasForeignKey(e => e.IdTipoAlerta)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.EstadoAlerta)
                    .WithMany(e => e.Alertas)
                    .HasForeignKey(e => e.IdEstadoAlerta)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de TipoAlertaCatalogo
            modelBuilder.Entity<TipoAlertaCatalogo>(entity =>
            {
                entity.ToTable("tipo_alerta_catalogo");
                entity.HasKey(e => e.IdTipoAlerta);
                entity.Property(e => e.IdTipoAlerta).HasColumnName("id_tipo_alerta");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
            });

            // Configuración de EstadoAlertaCatalogo
            modelBuilder.Entity<EstadoAlertaCatalogo>(entity =>
            {
                entity.ToTable("estado_alerta_catalogo");
                entity.HasKey(e => e.IdEstadoAlerta);
                entity.Property(e => e.IdEstadoAlerta).HasColumnName("id_estado_alerta");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
            });

            // Configuración de Reporte
            modelBuilder.Entity<Reporte>(entity =>
            {
                entity.ToTable("reporte");
                entity.HasKey(e => e.IdReporte);
                entity.Property(e => e.IdReporte).HasColumnName("id_reporte");
                entity.Property(e => e.TipoReporte).HasMaxLength(50).HasColumnName("tipo_reporte");
                entity.Property(e => e.Formato).HasMaxLength(20).HasColumnName("formato");
                entity.Property(e => e.FiltrosJson).HasColumnType("text").HasColumnName("filtros_json");
                entity.Property(e => e.RutaArchivo).HasMaxLength(500).HasColumnName("ruta_archivo");
                entity.Property(e => e.GeneradoPor).HasColumnName("generado_por");
                entity.Property(e => e.FechaGeneracion).IsRequired().HasColumnName("fecha_generacion");

                entity.HasOne(e => e.Usuario)
                    .WithMany(u => u.Reportes)
                    .HasForeignKey(e => e.GeneradoPor)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configuración de ReporteDetalle (tabla intermedia)
            modelBuilder.Entity<ReporteDetalle>(entity =>
            {
                entity.ToTable("reporte_detalle");
                entity.HasKey(e => new { e.IdReporte, e.IdSolicitud });
                entity.Property(e => e.IdReporte).HasColumnName("id_reporte");
                entity.Property(e => e.IdSolicitud).HasColumnName("id_solicitud");

                entity.HasOne(e => e.Reporte)
                    .WithMany(r => r.ReporteDetalles)
                    .HasForeignKey(e => e.IdReporte)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Solicitud)
                    .WithMany(s => s.ReporteDetalles)
                    .HasForeignKey(e => e.IdSolicitud)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de Permiso
            modelBuilder.Entity<Permiso>(entity =>
            {
                entity.ToTable("permiso");
                entity.HasKey(e => e.IdPermiso);
                entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");
                entity.Property(e => e.Codigo).IsRequired().HasMaxLength(50).HasColumnName("codigo");
                entity.Property(e => e.Descripcion).HasMaxLength(250).HasColumnName("descripcion");
                entity.Property(e => e.Nombre).HasMaxLength(100).HasColumnName("nombre");
            });

            // Configuración de RolPermiso (tabla intermedia)
            modelBuilder.Entity<RolPermiso>(entity =>
            {
                entity.ToTable("rol_permiso");
                entity.HasKey(e => new { e.IdRolSistema, e.IdPermiso });
                entity.Property(e => e.IdRolSistema).HasColumnName("id_rol_sistema");
                entity.Property(e => e.IdPermiso).HasColumnName("id_permiso");

                entity.HasOne(e => e.RolesSistema)
                    .WithMany(r => r.RolPermisos)
                    .HasForeignKey(e => e.IdRolSistema)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Permiso)
                    .WithMany(p => p.RolPermisos)
                    .HasForeignKey(e => e.IdPermiso)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
