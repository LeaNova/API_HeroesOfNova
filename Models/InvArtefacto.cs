using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class InvArtefacto {
    public InvArtefacto() {}
    public InvArtefacto(int mochilaId, int personajeId, int artefactoId, int cantidad) {
        this.mochilaId = mochilaId;
        this.personajeId = personajeId;
        this.artefactoId = artefactoId;
        this.cantidad = cantidad;
    }

    public int mochilaId { get; set; }
    public int personajeId { get; set; }
    public int artefactoId { get; set; }
    public int cantidad { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(artefactoId))]
    public Artefacto? artefacto { get; set; }
}