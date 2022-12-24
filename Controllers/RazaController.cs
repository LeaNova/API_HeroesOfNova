using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("raza")]
public class RazaController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public RazaController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Raza r) {
        try {
            if(ModelState.IsValid) {
                context.razas.Add(r);
                context.SaveChanges();
                
                return CreatedAtAction(nameof(obtenerId), new { id = r.idRaza }, r);
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
            Raza r = context.razas
                .AsNoTracking()
                .FirstOrDefault(x => x.idRaza == id);

            if(r != null) {
                context.razas.Remove(r);
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
    public async Task<IActionResult> modificar([FromForm] Raza r, int id) {
        try {
            if(ModelState.IsValid) {
                Raza original = context.razas
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idRaza == id);
                
                if(original != null) {
                    r.idRaza = id;

                    context.razas.Update(r);
                    await context.SaveChangesAsync();

                    return Ok(r);
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
    public async Task<ActionResult<Raza>> obtenerId(int id) {
        try {
            Raza r = context.razas
                .FirstOrDefault(x => x.idRaza == id);
            
            if(r != null) {
                return Ok(r);
            }
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{str}")]
    public async Task<ActionResult<Raza>> obtenerNombre(string nombre) {
        try {
            Raza r = context.razas
                .FirstOrDefault(x => x.nombre == nombre);
            
            if(r != null) {
                return Ok(r);
            }
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    public async Task<ActionResult<Raza>> obtenerTodos() {
        try {
            var listaRaza = await context.razas.ToListAsync();

            return Ok(listaRaza);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}
