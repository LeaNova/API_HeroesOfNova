using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("inventario/item")]
public class InvItemController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public InvItemController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] InvItem invItem) {
        try {
            if(ModelState.IsValid) {
                InvItem original = context.invItems
                    .FirstOrDefault(x => x.personajeId == invItem.personajeId && x.itemId == invItem.itemId);

                if(original != null) {
                    await añadir(original, invItem.cantidad);

                    return Ok();
                }

                context.invItems.Add(invItem);
                context.SaveChanges();

                return Ok();
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
    
    //Baja
    [HttpDelete("borrar/personaje={personajeId}_item={itemId}")]
    public async Task<IActionResult> borrar(int personajeId, int itemId) {
        try {
            InvItem invItem = context.invItems
                .AsNoTracking()
                .FirstOrDefault(x => x.personajeId == personajeId && x.itemId == itemId);

            if(invItem != null) {
                context.invItems.Remove(invItem);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificar
    [HttpPut("modificar/{mochilaId}/{personajeId}/{itemId}")]
    public async Task<ActionResult<InvItem>> modificar(int mochilaId, int personajeId, int itemId) {
        try {
            InvItem original = context.invItems
                .AsNoTracking()
                .FirstOrDefault(x => x.mochilaId == mochilaId && x.personajeId == personajeId && x.itemId == itemId);

            if(original != null) {
                InvItem invItem = new InvItem(mochilaId, personajeId, itemId);
                invItem.cantidad = original.cantidad-1;

                context.invItems.Update(invItem);
                await context.SaveChangesAsync();

                if(invItem.cantidad == 0) {
                    await borrarItem(invItem);
                }

                return Ok(invItem);
            }
            
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Obtener
    [HttpGet("get/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvItem>> obtenerMisItems(int mochilaId, int personajeId) {
        try {
            var listaItems = context.invItems
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Include(x => x.item).ThenInclude(x => x.tipo)
                .ToList();

            if(listaItems.Count() < 0) {
                return BadRequest("Sin items");
            }
            
            return Ok(listaItems);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/consumibles/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<Item>> consumibles(int mochilaId, int personajeId) {
        try {
            var listaItems = await context.invItems
                .Include(x => x.item).ThenInclude(x => x.tipo)
                .Where(x => x.item.tipo.nombre == "Poción" || x.item.tipo.nombre == "Comida")
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .ToListAsync();
            /*
            if(listaItems.Count() < 0) {
                return BadRequest("Sin resultados");
            }
            */
            return Ok(listaItems);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> añadir(InvItem invItem, int cantidad) {
        try {
            invItem.cantidad += cantidad;

            context.invItems.Update(invItem);
            context.SaveChanges();

            return Ok();
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> borrarItem(InvItem invItem) {
        try {
            context.invItems.Remove(invItem);
            context.SaveChanges();

            return Ok();
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}