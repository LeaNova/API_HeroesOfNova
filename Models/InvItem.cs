using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class InvItem {
    public InvItem() {}
    public InvItem(int mochilaId, int personajeId, int itemId) {
        this.mochilaId = mochilaId;
        this.personajeId = personajeId;
        this.itemId = itemId;
    }

    public int mochilaId { get; set; }
    public int personajeId { get; set; }
    public int itemId { get; set; }
    public int cantidad { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(itemId))]
    public Item? item { get; set; }
    
}