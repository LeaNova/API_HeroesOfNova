using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Tipo {
    [Key]
    public int idTipo { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
}