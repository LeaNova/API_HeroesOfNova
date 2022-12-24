using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Genero {

    [Key]
    public int idGenero { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
}