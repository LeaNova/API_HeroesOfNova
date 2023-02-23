using System.ComponentModel.DataAnnotations;

namespace API_HeroesOfNova;

public class Raza {

    [Key]
    public int idRaza { get; set; }
    public string nombre { get; set; }
    public int vidaBase { get; set; }
    public int energiaBase { get; set; }
    public int baseAtk { get; set; } //Ataque
    public int baseAtm { get; set; } //Ataque magico
    public int baseDef { get; set; } //Defensa
    public int baseDfm { get; set; } //Defensa magica
    public int baseDex { get; set; } //Agilidad
    public int baseEva { get; set; } //Evasion
    public int baseCrt { get; set; } //Critico
    public int baseAcc { get; set; } //Precision
    public string descripcion { get; set; }
    public bool disponible { get; set; }
}