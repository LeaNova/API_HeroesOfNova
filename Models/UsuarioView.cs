using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class UsuarioView {
    public UsuarioView(Usuario u) {
        this.nombre = u.nombre;
        this.apellido = u.apellido;
        this.mail = u.mail;
        this.usuario = u.usuario;
        this.rolId = u.rolId;
        this.fechaCreado = u.fechaCreado;
        this.rol = u.rol;
    }

    public string nombre { get; set; }
    public string apellido { get; set; }
    public string mail {get; set; }
    public string usuario { get; set; }
    public int rolId { get; set; }
    public DateTime fechaCreado { get; set; }
    [ForeignKey(nameof(rolId))]
    public Rol? rol { get; set; }
}