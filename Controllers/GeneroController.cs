using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("genero")]
public class GeneroController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public GeneroController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Genero g) {
        try {
            if(ModelState.IsValid) {
                context.generos.Add(g);
                context.SaveChanges();
                
                return CreatedAtAction(nameof(obtenerId), new { id = g.idGenero }, g);
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja
    [HttpDelete("borrar/{id}")]
    public async Task<IActionResult> borrar(int id) {
        try {
            Genero g = context.generos
                .AsNoTracking()
                .FirstOrDefault(x => x.idGenero == id);

            if(g != null) {
                context.generos.Remove(g);
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
    public async Task<IActionResult> modificar([FromForm] Genero g, int id) {
        try {
            if(ModelState.IsValid) {
                Genero original = context.generos
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idGenero == id);
                
                if(original != null) {
                    g.idGenero = id;

                    context.generos.Update(g);
                    await context.SaveChangesAsync();

                    return Ok(g);
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
    public async Task<ActionResult<Genero>> obtenerId(int id) {
        try {
            Genero g = context.generos
                .FirstOrDefault(x => x.idGenero == id);
            
            if(g != null) {
                return Ok(g);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    public async Task<ActionResult<Genero>> obtenerTodos() {
        try {
            var listaGeneros = await context.generos.ToListAsync();

            return Ok(listaGeneros);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}
