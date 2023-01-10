using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class Grupo {

    [Key]
    public int idGrupo { get; set; }
    public int masterId { get; set; }
    public string nombre {get; set; }
    public string descripcion { get; set; }
    public string pass { get; set; }
    public bool disponible { get; set; }
    public DateTime fechaCreado { get; set; }
    [ForeignKey(nameof(masterId))]
    public Usuario? usuario { get; set; }
    [NotMapped]
    public UsuarioView? master { get; set; }
}