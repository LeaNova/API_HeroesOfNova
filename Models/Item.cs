using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class Item {
    [Key]
    public int idItem { get; set; }
    public string nombre { get; set; }
    public int tipoId { get; set; }
    public int rarezaId { get; set; }
    //Efecto si posee
    public int bonoVida { get; set; }
    public int bonoEnergia { get; set; }
    public int bonoAtk { get; set; }
    public int bonoAtm { get; set; }
    public int bonoDef { get; set; }
    public int bonoDfm { get; set; }
    public int bonoDex { get; set; }
    public int bonoEva { get; set; }
    public int bonoCrt { get; set; }
    public int bonoAcc { get; set; }
    //Extras
    public int precio { get; set; }
    public float peso { get; set; }
    public string descripcion { get; set; }
    public bool disponible { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(tipoId))]
    public Tipo? tipo { get; set; }
    [ForeignKey(nameof(rarezaId))]
    public Rareza? rareza { get; set; }
}