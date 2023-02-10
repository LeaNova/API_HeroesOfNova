using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("inventario/armadura")]
public class InvArmaduraController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public InvArmaduraController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] InvArmadura invArmadura) {
        try {
            if(ModelState.IsValid) {
                InvArmadura original = context.invArmaduras
                    .FirstOrDefault(x => x.personajeId == invArmadura.personajeId && x.armaduraId == invArmadura.armaduraId);

                if(original != null) {
                    await añadir(original, invArmadura.cantidad);

                    return Ok();
                }

                context.invArmaduras.Add(invArmadura);
                context.SaveChanges();

                return Ok();
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
    
    //Baja
    [HttpDelete("borrar/personaje={personajeId}_armadura={armaduraId}")]
    public async Task<IActionResult> borrar(int personajeId, int armaduraId) {
        try {
            InvArmadura invArmadura = context.invArmaduras
                .AsNoTracking()
                .FirstOrDefault(x => x.personajeId == personajeId && x.armaduraId == armaduraId);

            if(invArmadura != null) {
                context.invArmaduras.Remove(invArmadura);
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
    [HttpGet("get/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvArmadura>> obtenerMisArmaduras(int mochilaId, int personajeId) {
        try {
            var listaArmaduras = context.invArmaduras
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Include(x => x.armadura)
                .ToList();

            if(listaArmaduras.Count() > 0) {
                return Ok(listaArmaduras);
            }
            
            return BadRequest("Sin armaduras");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> añadir(InvArmadura invArmadura, int cantidad) {
        try {
            invArmadura.cantidad += cantidad;

            context.invArmaduras.Update(invArmadura);
            context.SaveChanges();

            return Ok();
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}