using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class Arma {
    [Key]
    public int idArma { get; set; }
    public string nombre { get; set; }
    //Bases del arma
    public int categoriaId { get; set; }
    public int rarezaId { get; set; }
    public int danioArma { get; set; }
    public int bonoArma { get; set; }
    //Bonos del arma
    public int bonoAtk { get; set; }
    public int bonoAtm { get; set; }
    public int bonoDef { get; set; }
    public int bonoDfm { get; set; }
    public int bonoCrt { get; set; }
    public int bonoAcc { get; set; }
    //Especialidad del arma
    public float modAtk { get; set; }
    public float modAtm { get; set; }
    public float modDef { get; set; }
    public float modDfm { get; set; }
    //Extras
    public int precio { get; set; }
    public float peso { get; set; }
    public string descripcion { get; set; }
    public bool disponible { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(categoriaId))]
    public Categoria? categoria { get; set; }
    [ForeignKey(nameof(rarezaId))]
    public Rareza? rareza { get; set; }
}