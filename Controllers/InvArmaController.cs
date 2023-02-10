using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("inventario/arma")]
public class InvArmaController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public InvArmaController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] InvArma invArma) {
        try {
            if(ModelState.IsValid) {
                InvArma original = context.invArmas
                    .FirstOrDefault(x => x.personajeId == invArma.personajeId && x.armaId == invArma.armaId);

                if(original != null) {
                    await añadir(original, invArma.cantidad);

                    return Ok();
                }

                context.invArmas.Add(invArma);
                context.SaveChanges();

                return Ok();
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
    
    //Baja
    [HttpDelete("borrar/personaje={personajeId}_arma={armaId}")]
    public async Task<IActionResult> borrar(int personajeId, int armaId) {
        try {
            InvArma invArma = context.invArmas
                .AsNoTracking()
                .FirstOrDefault(x => x.personajeId == personajeId && x.armaId == armaId);

            if(invArma != null) {
                context.invArmas.Remove(invArma);
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
    public async Task<ActionResult<InvArma>> obtenerMisArmas(int mochilaId, int personajeId) {
        try {
            var listaArmas = context.invArmas
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Include(x => x.arma).ThenInclude(x => x.categoria)
                .ToList();
            
            if(listaArmas.Count() > 0) {
                return Ok(listaArmas);
            }

            return BadRequest("Sin armas");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> añadir(InvArma invArma, int cantidad) {
        try {
            invArma.cantidad += cantidad;

            context.invArmas.Update(invArma);
            context.SaveChanges();

            return Ok();
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}