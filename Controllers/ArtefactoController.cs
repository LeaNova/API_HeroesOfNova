using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("artefacto")]

public class ArtefactoController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public ArtefactoController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Artefacto a) {
        try {
            if(ModelState.IsValid) {
                a = corregir(a);
                context.artefactos.Add(a);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtenerId), new { id = a.idArtefacto }, a);
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
            Artefacto a = context.artefactos
                .AsNoTracking()
                .FirstOrDefault(x => x.idArtefacto == id);

            if(a != null) {
                context.artefactos.Remove(a);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("baja/{id}")]
    public async Task<ActionResult<Artefacto>> baja(int id) {
        try {
            Artefacto a = context.artefactos
                .AsNoTracking()
                .FirstOrDefault(x => x.idArtefacto == id);

            if(a != null) {
                a.disponible = !a.disponible;

                context.artefactos.Update(a);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion
    [HttpPut("modificar/{id}")]
    public async Task<IActionResult> modificar([FromForm] Artefacto a, int id) {
        try {
            if(ModelState.IsValid) {
                Artefacto original = context.artefactos
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idArtefacto == id);

                if(original != null) {
                    a = corregir(a);
                    a.idArtefacto = id;
                    a.seccionId = original.seccionId;
                    a.rarezaId = original.rarezaId;

                    context.artefactos.Update(a);
                    await context.SaveChangesAsync();

                    return Ok(a);
                }
                return BadRequest("Objeto vacío");
            }
            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Busquedas
    [HttpGet("get")]
    public async Task<ActionResult<Artefacto>> obtener() {
        try {
            var listaArtefactos = new List<Artefacto>();

            if(User.IsInRole("Admin")) {
                listaArtefactos = await context.artefactos
                    .Include(x => x.seccion).Include(x => x.rareza)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaArtefactos = await context.artefactos
                    .Include(x => x.seccion).Include(x => x.rareza)
                    .Where(x => x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }
            
            return Ok(listaArtefactos);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Artefacto>> obtenerId(int id) {
        try {
            Artefacto a = context.artefactos
                .Include(x => x.seccion).Include(x => x.rareza)
                .FirstOrDefault(x => x.idArtefacto == id);
            
            if(a != null) {
                return Ok(a);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search/{nombre}")]
    public async Task<ActionResult<Artefacto>> obtenerBusqueda(string nombre) {
        try {
            var listaArtefactos = new List<Artefacto>();

            if(User.IsInRole("Admin")) {
                listaArtefactos = await context.artefactos
                    .Include(x => x.seccion)
                    .Where(x => x.nombre.Contains(nombre))
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaArtefactos = await context.artefactos
                    .Include(x => x.seccion)
                    .Where(x => x.nombre.Contains(nombre) && x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }

            if(listaArtefactos.Count() > 0) {
                return Ok(listaArtefactos);
            }

            return BadRequest("Sin resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check/{nombre}")]
    public async Task<ActionResult<Artefacto>> check(string nombre) {
        try {
            Artefacto a = context.artefactos
                .FirstOrDefault(x => x.nombre == nombre);
            
            if(a != null) {
                return Ok(a);
            }
            
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    private Artefacto corregir(Artefacto a) {
        a.peso = a.peso > 100 ? a.peso/100 : a.peso/10;

        return a;
    }
}