using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class Usuario {
    
    [Key]
    public int idUsuario { get; set; }
    public string nombre { get; set; }
    public string apellido { get; set; }
    public string mail { get; set; }
    public string usuario { get; set; }
    public string pass { get; set; }
    public int rolId { get; set; }
    public DateTime fechaCreado { get; set; }
    [ForeignKey(nameof(rolId))]
    public Rol? rol { get; set; }
}