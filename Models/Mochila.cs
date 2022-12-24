using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Mochila {
    
    [Key]
    public int idMochila { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public int pesoMax { get; set; }
}