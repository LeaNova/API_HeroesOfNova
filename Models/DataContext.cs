using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

public class DataContext : DbContext {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    //Creacion de usuario
    public DbSet<Usuario> usuarios { get; set; }
    public DbSet<Rol> roles { get; set; }

    //Creacion de Personaje
    public DbSet<Raza> razas { get; set; }
    public DbSet<Genero> generos { get; set; }
    public DbSet<Clase> clases { get; set; }
    public DbSet<Personaje> personajes { get; set; }
    public DbSet<Mochila> mochilas { get; set; }
}