using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class Participante {
    public int grupoId { get; set; }
    public int usuarioId { get; set; }
    public int personajeId { get; set; }
    
    [ForeignKey(nameof(grupoId))]
    public Grupo? grupo { get; set; }
    [ForeignKey(nameof(usuarioId))]
    public Usuario? usuario { get; set; }
    [NotMapped]
    public UsuarioView jugador { get; set; }
    [ForeignKey(nameof(personajeId))]
    public Personaje? personaje { get; set; }
    
}