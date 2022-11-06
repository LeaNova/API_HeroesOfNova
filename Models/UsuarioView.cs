using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class UsuarioView {

    public UsuarioView(Usuario u) {
        this.nombre = u.nombre;
        this.apellido = u.apellido;
        this.mail = u.mail;
        this.rolId = u.rolId;
    }

    public string nombre { get; set; }
    public string apellido { get; set; }
    public string mail {get; set; }
    public int? rolId { get; set; }
    [ForeignKey(nameof(rolId))]
    public Rol rol { get; set; }
}