using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("grupo")]
public class GrupoController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public GrupoController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<ActionResult> alta([FromForm] Grupo g) {
        try {
            if(ModelState.IsValid) {
                g.masterId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
                g.fechaCreado = DateTime.Now;

                context.grupos.Add(g);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtenerId), new { id = g.idGrupo }, g);
            }

            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja - Baja logica
    [HttpDelete("borrar/{id}")]
    public async Task<IActionResult> borrar(int id) {
        try {
            int masterId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);

            Grupo g = context.grupos
                .AsNoTracking()
                .FirstOrDefault(x => x.idGrupo == id && x.masterId == masterId);

                if(g != null) {
                    var participantes = context.participantes
                        .AsNoTracking()
                        .Where(x => x.grupoId == g.idGrupo)
                        .ToList();
                    
                    context.participantes.RemoveRange(participantes);
                    context.grupos.Remove(g);
                    context.SaveChanges();

                    return Ok();
                }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("baja/{id}")]
    public async Task<ActionResult> baja(int id) {
        try {
            Grupo g = context.grupos
                .AsNoTracking()
                .FirstOrDefault(x => x.idGrupo == id);
            
            if(g != null) {
                g.disponible = !g.disponible;

                context.grupos.Update(g);
                await context.SaveChangesAsync();

                return Ok(g);
            }

            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion
    [HttpPost("modificar/{id}")]
    public async Task<IActionResult> modificar([FromForm] Grupo g, int id) {
        try {
            if(ModelState.IsValid) {
                Grupo original = context.grupos
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idGrupo == id);

                if(original != null) {
                    g.idGrupo = id;
                    g.masterId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
                    g.fechaCreado = DateTime.Now;

                    context.grupos.Update(g);
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

    //Busquedas
    [HttpGet("get")] //Uso para ADMIN
    public async Task<ActionResult<Grupo>> obtener() {
        try {
            var listaGrupos = context.grupos
                .Include(x => x.usuario);
            
            if(listaGrupos.Count() > 0) {
                foreach(Grupo item in listaGrupos) {
                    item.master = new UsuarioView(item.usuario);
                    item.usuario = null;
                }

                return Ok(listaGrupos);
            }

            return BadRequest("No hay resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get_own")]
    public async Task<ActionResult<Grupo>> obtenerPropios() {
        try {
            int id = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            var listaGrupos = context.grupos
                .Where(x => x.masterId == id);
            
            if(listaGrupos.Count() > 0) {
                return Ok(listaGrupos);
            }

            return BadRequest("No hay resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Grupo>> obtenerId(int id) {
        try {
            Grupo g = context.grupos
                .Include(x => x.usuario)
                .FirstOrDefault(x => x.idGrupo == id);
            
            if(g != null) {
                g.master = new UsuarioView(g.usuario);
                g.usuario = null;
                return Ok(g);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get_nombre/{nombre}")]
    public async Task<ActionResult<Grupo>> obtenerNombre(string nombre) {
        try {
            var listaGrupos = context.grupos
                .Include(x => x.usuario)
                .Where(x => x.disponible)
                .Where(x => x.nombre.StartsWith(nombre));
            
            if(listaGrupos.Count() > 0) {
                foreach(Grupo item in listaGrupos) {
                    item.master = new UsuarioView(item.usuario);
                    item.usuario = null;
                }
                
                return Ok(listaGrupos);
            }
            
            return BadRequest("No hay resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}