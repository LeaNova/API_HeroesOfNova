using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

public class DataContext : DbContext {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    //Contexto de Usuarios
    public DbSet<Usuario> usuarios { get; set; }
    public DbSet<Rol> roles { get; set; }

    //Contexto de Personaje y demás
    public DbSet<Raza> razas { get; set; }
    public DbSet<Genero> generos { get; set; }
    public DbSet<Clase> clases { get; set; }
    public DbSet<Personaje> personajes { get; set; }
    public DbSet<Mochila> mochilas { get; set; }

    //Contexto de Grupos y Participantes
    public DbSet<Grupo> grupos { get; set; }
    public DbSet<Participante> participantes { get; set; }

    //Contexto de Armas, Armaduras, Items y demás
    public DbSet<Arma> armas { get; set; }
    public DbSet<Categoria> categorias { get; set; }
    public DbSet<Armadura> armaduras { get; set; }
    public DbSet<Item> items { get; set; }
    public DbSet<Tipo> tipos { get; set; }
    public DbSet<InvArma> invArmas { get; set; }
    public DbSet<InvArmadura> invArmaduras { get; set; }
    public DbSet<InvItem> invItems { get; set; }

    //Setteo de doble Key
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Participante>().HasKey(par => new { par.grupoId, par.usuarioId });
        modelBuilder.Entity<InvArma>().HasKey(invWpn => new { invWpn.mochilaId, invWpn.personajeId, invWpn.armaId });
        modelBuilder.Entity<InvArmadura>().HasKey(invArm => new { invArm.mochilaId, invArm.personajeId, invArm.armaduraId });
        modelBuilder.Entity<InvItem>().HasKey(invItm => new { invItm.mochilaId, invItm.personajeId, invItm.itemId });
    }

}