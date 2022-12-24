using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Clase {

    [Key]
    public string idClase { get; set; }
    public string nombre { get; set; }
    public string descripcion { get; set; }
    public float modVida { get; set; }
    public float modAtk { get; set; }
    public float modAtm { get; set; }
    public float modDef { get; set; }
    public float modDfm { get; set; }
    public float modDex { get; set; }
    public float modEva { get; set; }
    public float modCrt { get; set; }
    public float modAcc { get; set; }
}