using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Rol {
    
    [Key]
    public int idRol { get; set; }
    public string nombre { get; set; }
}