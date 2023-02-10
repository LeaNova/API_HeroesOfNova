using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Seccion {
    [Key]
    public int idSeccion { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
}