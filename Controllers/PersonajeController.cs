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
                p.fechaCreado = DateTime.Now;
                
                context.personajes.Add(p);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtenerId), new { id = p.idPersonaje }, p);
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
            int idUsuario = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            Personaje p = await context.personajes
            .FirstOrDefaultAsync(x => x.idPersonaje == id && x.usuarioId == idUsuario);
            
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

    [HttpPost("baja/{id}")]
    public async Task<ActionResult> baja(int id) {
        try {
            int idUsuario = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            Personaje p = context.personajes
                .AsNoTracking()
                .FirstOrDefault(x => x.idPersonaje == id && x.usuarioId == idUsuario);

            if(p != null) {
                p.disponible = !p.disponible;

                context.personajes.Update(p);
                await context.SaveChangesAsync();

                return Ok(p);
            }

            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion

    //Busquedas
    [HttpGet("get")]
    public async Task<ActionResult<Personaje>> obtener() {
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

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Personaje>> obtenerId(int id) {
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
}