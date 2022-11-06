using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

public class DataContext : DbContext {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Rol> Roles { get; set; }
}