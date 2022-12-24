using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("clase")]
public class ClaseController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public ClaseController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Clase c) {
        try {
            if(ModelState.IsValid) {
                context.clases.Add(c);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtener), new { id = c.idClase }, c);
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja
    [HttpDelete("borrar/{id}")]
    public async Task<IActionResult> baja(string id) {
        try {
            Clase c = context.clases
                .AsNoTracking()
                .FirstOrDefault(x => x.idClase == id);

            if(c != null) {
                context.clases.Remove(c);
                context.SaveChanges();

                return Ok();
            }
            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificar
    [HttpPost("modificar/{id}")]
    public async Task<IActionResult> modificar([FromForm] Clase c, string id) {
        try {
            if(ModelState.IsValid) {
                Clase original = context.clases
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idClase == id);

                if(original != null) {
                    c.idClase = id;

                    context.clases.Update(c);
                    await context.SaveChangesAsync();

                    return Ok(c);
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
    public async Task<ActionResult<Clase>> obtener(string id) {
        try {
            Clase c = context.clases
                .FirstOrDefault(x => x.idClase == id);
            
            if(c != null) {
                return Ok(c);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    public async Task<ActionResult<Clase>> obtenerTodos() {
        try {
            var listaClases = await context.clases.ToListAsync();

            return Ok(listaClases);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}
