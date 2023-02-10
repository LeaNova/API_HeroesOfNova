using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class InvArma {
    public InvArma() {}
    public InvArma(int mochilaId, int personajeId, int armaId, int cantidad) {
        this.mochilaId = mochilaId;
        this.personajeId = personajeId;
        this.armaId = armaId;
        this.cantidad = cantidad;
    }

    public int mochilaId { get; set; }
    public int personajeId { get; set; }
    public int armaId { get; set; }
    public int cantidad { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(armaId))]
    public Arma? arma { get; set; }
}