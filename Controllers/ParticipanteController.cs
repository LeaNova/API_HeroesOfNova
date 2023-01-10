using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("participante")]
public class ParticipanteController: ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public ParticipanteController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<ActionResult> alta([FromForm] Participante p) {
        try {
            if(ModelState.IsValid) {
                p.usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);

                context.participantes.Add(p);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtenerId), new { grupoId = p.grupoId, usuarioId = p.usuarioId }, p);
            }

            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja
    [HttpDelete("borrar/grupo={grupoId}")]
    public async Task<IActionResult> borrar(int grupoId) {
        try {
            int usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            Participante p = context.participantes
                .AsNoTracking()
                .FirstOrDefault(x => x.grupoId == grupoId && x.usuarioId == usuarioId);

            if(p != null) {
                context.participantes.Remove(p);
                context.SaveChanges();
                
                return Ok();
            }
            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion
    [HttpPost("modificar/g{grupoId}_u")]
    public async Task<IActionResult> modificar([FromForm] Participante p, int grupoId) {
        try {
            if(ModelState.IsValid) {
                int usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
                Participante original = context.participantes
                    .AsNoTracking()
                    .FirstOrDefault(x => x.grupoId == grupoId && x.usuarioId == usuarioId);
                
                if(original != null) {
                    p.grupoId = grupoId;
                    p.usuarioId = usuarioId;

                    context.participantes.Update(p);
                    await context.SaveChangesAsync();

                    return Ok(p);
                }
                return BadRequest("Objeto vacío");
            }
            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Busquedas
    [HttpGet("get/g{grupoId}_u{usuarioId}")]
    public async Task<ActionResult<Participante>> obtenerId(int grupoId, int usuarioId) {
        try {
            Participante p = await context.participantes
                .FirstOrDefaultAsync(x => x.grupoId == grupoId && x.usuarioId == usuarioId);
            
            if(p != null) {
                return Ok(p);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/grupos")]
    public async Task<ActionResult<Participante>> obtenerMisGrupos() {
        try {
            int id = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            var listaParticipantes = await context.participantes
                .Include(x => x.grupo)
                .ThenInclude(x => x.usuario)
                .Include(x => x.personaje)
                .Where(x => x.usuarioId == id)
                .ToListAsync();

            if(listaParticipantes.Count() > 0) {
                foreach(Participante item in listaParticipantes) {
                    item.grupo.master = new UsuarioView(item.grupo.usuario);
                    item.grupo.usuario = null;
                }
                return Ok(listaParticipantes);
            }
            
            return BadRequest("No hay resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/grupo/{id}")]
    public async Task<ActionResult<Participante>> obtenerParticipantesGrupo(int id) {
        try {
            var listaParticipantes = await context.participantes
                .Include(x => x.usuario)
                .Include(x => x.personaje)
                .Where(x => x.grupoId == id)
                .ToListAsync();

            if(listaParticipantes.Count() > 0) {
                foreach(Participante item in listaParticipantes) {
                    item.jugador = new UsuarioView(item.usuario);
                    item.usuario = null;
                }
                return Ok(listaParticipantes);
            }
            
            return BadRequest("No hay resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}