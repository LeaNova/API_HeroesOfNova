using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class Personaje {

    [Key]
    public int idPersonaje { get; set; }
    public int usuarioId { get; set; }
    public string nombre { get; set; }
    public int razaId { get; set; }
    public int generoId { get; set; }
    public int claseId { get; set; }
    public int vida { get; set; }
    public int nivel { get; set; }
    public int experiencia { get; set; }

    //Estadisticas
    public int ataque { get; set; }
    public int atkMagico { get; set; }
    public int defensa { get; set; }
    public int defMagico { get; set; }
    public int agilidad { get; set; }
    public int evasion { get; set; }
    public int critico { get; set; }
    public int precision { get; set; }

    //Extras
    public string descripcion { get; set; }
    public int mochilaId { get; set; }
    public bool disponible { get; set; }
    public DateTime fechaCreado { get; set; }

    //Clases foráneas
    [ForeignKey(nameof(usuarioId))]
    public Usuario? usuario { get; set; }
    [ForeignKey(nameof(razaId))]
    public Raza? raza { get; set; }
    [ForeignKey(nameof(generoId))]
    public Genero? genero { get; set; }
    [ForeignKey(nameof(claseId))]
    public Clase? clase { get; set; }
    [ForeignKey(nameof(mochilaId))]
    public Mochila? mochila { get; set; }
}