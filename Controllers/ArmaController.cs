using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("arma")]
public class ArmaController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public ArmaController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Arma a) {
        try {
            if(ModelState.IsValid) {
                a = corregir(a);
                context.armas.Add(a);
                context.SaveChanges();
                
                return CreatedAtAction(nameof(obtenerId), new { id = a.idArma }, a);
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
            Arma a = context.armas
                .AsNoTracking()
                .FirstOrDefault(x => x.idArma == id);

            if(a != null) {
                context.armas.Remove(a);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("baja/{id}")]
    public async Task<ActionResult<Arma>> baja(int id) {
        try {
            Arma a = context.armas
                .AsNoTracking()
                .FirstOrDefault(x => x.idArma == id);

            if(a != null) {
                a.disponible = !a.disponible;

                context.armas.Update(a);
                context.SaveChanges();

                return Ok(a);
            }

            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificacion
    [HttpPut("modificar/{id}")]
    public async Task<IActionResult> modificar([FromForm] Arma a, int id) {
        try {
            if(ModelState.IsValid) {
                Arma original = context.armas
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idArma == id);
                
                if(original != null) {
                    a = corregir(a);
                    a.idArma = id;
                    a.categoriaId = original.categoriaId;
                    a.rarezaId = original.rarezaId;

                    context.armas.Update(a);
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

    //Obtener
    [HttpGet("get")]
    public async Task<ActionResult<Arma>> obtener() {
        try {
            var listaArmas = new List<Arma>();

            if(User.IsInRole("Admin")) {
                listaArmas = await context.armas
                    .Include(x => x.categoria).Include(x => x.rareza)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaArmas = await context.armas
                    .Include(x => x.categoria).Include(x => x.rareza)
                    .Where(x => x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }

            return Ok(listaArmas);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Arma>> obtenerId(int id) {
        try {
            Arma a = context.armas
                .Include(x => x.categoria).Include(x => x.rareza)
                .FirstOrDefault(x => x.idArma == id);
            
            if(a != null) {
                return Ok(a);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search/{nombre}")]
    public async Task<ActionResult<Arma>> obtenerBusqueda(string nombre) {
        try {
            var listaArmas = new List<Arma>();

            if(User.IsInRole("Admin")) {
                listaArmas = await context.armas
                    .Include(x => x.categoria).Include(x => x.rareza)
                    .Where(x => x.nombre.Contains(nombre))
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaArmas = await context.armas
                    .Include(x => x.categoria).Include(x => x.rareza)
                    .Where(x => x.nombre.Contains(nombre) && x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }

            if(listaArmas.Count() > 0) {
                return Ok(listaArmas);
            }

            return BadRequest("Sin resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check/{nombre}")]
    public async Task<ActionResult<Arma>> check(string nombre) {
        try {
            Arma a = context.armas
                .FirstOrDefault(x => x.nombre == nombre);
            
            if(a != null) {
                return Ok(a);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    private Arma corregir(Arma a) {
        a.modAtk = a.modAtk > 100 ? a.modAtk/100 : a.modAtk/10;
        a.modAtm = a.modAtm > 100 ? a.modAtm/100 : a.modAtm/10;
        a.modDef = a.modDef > 100 ? a.modDef/100 : a.modDef/10;
        a.modDfm = a.modDfm > 100 ? a.modDfm/100 : a.modDfm/10;
        a.peso = a.peso > 100 ? a.peso/100 : a.peso/10;

        return a;
    }
}