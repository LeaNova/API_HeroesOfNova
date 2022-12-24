using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("mochila")]
public class MochilaController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public MochilaController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Mochila m) {
        try {
            if(ModelState.IsValid) {
                context.mochilas.Add(m);
                context.SaveChanges();
                
                return CreatedAtAction(nameof(obtener), new { id = m.idMochila }, m);
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja
    [HttpDelete("borrar/{id}")]
    public async Task<IActionResult> baja(int id) {
        try {
            Mochila m = context.mochilas
                .AsNoTracking()
                .FirstOrDefault(x => x.idMochila == id);

            if(m != null) {
                context.mochilas.Remove(m);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion
    [HttpPost("modificar/{id}")]
    public async Task<IActionResult> modificar([FromForm] Mochila m, int id) {
        try {
            if(ModelState.IsValid) {
                Mochila original = context.mochilas
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idMochila == id);
                
                if(original != null) {
                    m.idMochila = id;

                    context.mochilas.Update(m);
                    await context.SaveChangesAsync();

                    return Ok(m);
                }
                return BadRequest("Objeto vacío");
            }
            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Obtener
    [HttpGet("get/{id}")]
    public async Task<ActionResult<Mochila>> obtener(int id) {
        try {
            Mochila m = context.mochilas
                .FirstOrDefault(x => x.idMochila == id);
            
            if(m != null) {
                return Ok(m);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    public async Task<ActionResult<Mochila>> obtenerTodos() {
        try {
            var listaMochilas = await context.mochilas.ToListAsync();

            return Ok(listaMochilas);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}