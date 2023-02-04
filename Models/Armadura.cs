using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Armadura {
    [Key]
    public int idArmadura { get; set; }
    public string nombre { get; set; }
    //Bonos de la armadura
    public int bonoDef { get; set; }
    public int bonoDfm { get; set; }
    public int bonoDex { get; set; }
    public int bonoEva { get; set; }
    public float modDef { get; set; }
    public float modDfm { get; set; }
    //Extras
    public int precio { get; set; }
    public float peso { get; set; }
    public string descripcion { get; set; }
}