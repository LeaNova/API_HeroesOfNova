using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("armadura")]
public class ArmaduraController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public ArmaduraController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Armadura a) {
        try {
            if(ModelState.IsValid) {
                a = corregir(a);
                context.armaduras.Add(a);
                context.SaveChanges();
                
                return CreatedAtAction(nameof(obtenerId), new { id = a.idArmadura }, a);
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
            Armadura a = context.armaduras
                .AsNoTracking()
                .FirstOrDefault(x => x.idArmadura == id);

            if(a != null) {
                context.armaduras.Remove(a);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("baja/{id}")]
    public async Task<ActionResult<Armadura>> baja(int id) {
        try {
            Armadura a = context.armaduras
                .AsNoTracking()
                .FirstOrDefault(x => x.idArmadura == id);

            if(a != null) {
                a.disponible = !a.disponible;

                context.armaduras.Update(a);
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
    public async Task<IActionResult> modificar([FromForm] Armadura a, int id) {
        try {
            if(ModelState.IsValid) {
                Armadura original = context.armaduras
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idArmadura == id);
                
                if(original != null) {
                    a = corregir(a);
                    a.idArmadura = id;
                    a.rarezaId = original.rarezaId;

                    context.armaduras.Update(a);
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
    public async Task<ActionResult<Armadura>> obtener() {
        try {
            var listaArmaduras = new List<Armadura>();

            if(User.IsInRole("Admin")) {
                listaArmaduras = await context.armaduras
                    .Include(x => x.rareza)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaArmaduras = await context.armaduras
                    .Include(x => x.rareza)
                    .Where(x => x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }

            return Ok(listaArmaduras);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Armadura>> obtenerId(int id) {
        try {
            Armadura a = context.armaduras
                .Include(x => x.rareza)
                .FirstOrDefault(x => x.idArmadura == id);

            if(a != null) {
                return Ok(a);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search/{nombre}")]
    public async Task<ActionResult<Armadura>> obtenerBusqueda(string nombre) {
        try {
            var listaArmaduras = new List<Armadura>();
            
            if(User.IsInRole("Admin")) {
                listaArmaduras = await context.armaduras
                    .Where(x => x.nombre.Contains(nombre))
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaArmaduras = await context.armaduras
                    .Where(x => x.nombre.Contains(nombre) && x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }

            if(listaArmaduras.Count() > 0) {
                return Ok(listaArmaduras);
            }

            return BadRequest("Sin resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check/{nombre}")]
    public async Task<ActionResult<Armadura>> check(string nombre) {
        try {
            Armadura a = context.armaduras
                .FirstOrDefault(x => x.nombre == nombre);

            if(a != null) {
                return Ok(a);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    private Armadura corregir(Armadura a) {
        a.modDef = a.modDef > 100 ? a.modDef/100 : a.modDef/10;
        a.modDfm = a.modDfm > 100 ? a.modDfm/100 : a.modDfm/10;
        a.peso = a.peso > 100 ? a.peso/100 : a.peso/10;

        return a;
    }
}