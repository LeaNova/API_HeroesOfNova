using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Categoria {
    [Key]
    public int idCategoria { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
}