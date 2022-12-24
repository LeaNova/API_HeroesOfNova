using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("personaje")]
public class PersonajeController : ControllerBase {

    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public PersonajeController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }
    
    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Personaje p) {
        try {
            if(ModelState.IsValid) {
                p.usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);

                context.personajes.Add(p);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtener), new { id = p.idPersonaje }, p);
            }

            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja - Baja logica
    [HttpDelete("borrar/{id}")]
    public async Task<ActionResult> borrar(int id) {
        try {
            Personaje p = await context.personajes
            .Include(x => x.raza)
            .Include(x => x.genero)
            .Include(x => x.mochila)
            .FirstOrDefaultAsync(x => x.idPersonaje == id);
            
            if(p != null) {
                context.personajes.Remove(p);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion

    //Busquedas
    [HttpGet("get/{id}")]
    public async Task<ActionResult<Personaje>> obtener(int id) {
        try {
            Personaje p = await context.personajes
            .Include(x => x.raza)
            .Include(x => x.genero)
            .Include(x => x.clase)
            .Include(x => x.mochila)
            .FirstOrDefaultAsync(x => x.idPersonaje == id);
            
            if(p != null) {
                return Ok(p);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    public async Task<ActionResult<Personaje>> obtenerTodos() {
        try {
            int id = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);

            var listaPersonajes = context.personajes
            .Include(x => x.raza)
            .Include(x => x.genero)
            .Include(x => x.clase)
            .Include(x => x.mochila)
            .Where(x => x.usuarioId == id);
            
            return Ok(listaPersonajes);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Level up
}