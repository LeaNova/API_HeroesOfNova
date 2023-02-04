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
    [HttpGet("get/personaje={personajeId}")]
    public async Task<ActionResult<InvArma>> obtenerMisArmas(int personajeId) {
        try {
            var listaArmas = context.invArmas
                .Where(x => x.personajeId == personajeId)
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
}