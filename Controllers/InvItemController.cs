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

    //Obtener
    [HttpGet("get/personaje={personajeId}")]
    public async Task<ActionResult<InvItem>> obtenerMisArmaduras(int personajeId) {
        try {
            var listaItems = context.invItems
                .Where(x => x.personajeId == personajeId)
                .Include(x => x.item).ThenInclude(x => x.tipo)
                .ToList();

            if(listaItems.Count() > 0) {
                return Ok(listaItems);
            }
            
            return BadRequest("Sin items");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}