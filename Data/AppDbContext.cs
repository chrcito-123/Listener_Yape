using Microsoft.EntityFrameworkCore;
using Listener_Yape.Models;

namespace Listener_Yape.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Aplicacion> Aplicaciones { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Suscripcion> Suscripciones { get; set; }
        public DbSet<UsuarioAplicacion> UsuarioAplicaciones { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<TokenRevocado> TokenRevocados { get; set; }

    }
}
