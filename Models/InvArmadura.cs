using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class InvArmadura {
    public InvArmadura(int mochilaId, int personajeId, int armaduraId, int cantidad) {
        this.mochilaId = mochilaId;
        this.personajeId = personajeId;
        this.armaduraId = armaduraId;
        this.cantidad = cantidad;
    }

    public int mochilaId { get; set; }
    public int personajeId { get; set; }
    public int armaduraId { get; set; }
    public int cantidad { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(armaduraId))]
    public Armadura? armadura { get; set; }

}