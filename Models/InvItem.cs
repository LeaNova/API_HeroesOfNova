using System.ComponentModel.DataAnnotations.Schema;

namespace API_HeroesOfNova;

public class InvItem {
    public int mochilaId { get; set; }
    public int personajeId { get; set; }
    public int itemId { get; set; }
    public int cantidad { get; set; }
    //Clases foráneas
    [ForeignKey(nameof(itemId))]
    public Item? item { get; set; }
    
}