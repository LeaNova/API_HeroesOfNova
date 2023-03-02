using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Rareza {

    [Key]
    public int idRareza { get; set; }
    public string nombre { get; set; }
    public string iniciales { get; set; }
    public int nivelMin { get; set; }
    public string codColor { get; set; }
}