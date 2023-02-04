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
                c = corregir(c);

                context.clases.Add(c);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtenerId), new { id = c.idClase }, c);
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
    [HttpPut("modificar/{id}")]
    public async Task<IActionResult> modificar([FromForm] Clase c, int id) {
        try {
            if(ModelState.IsValid) {
                Clase original = context.clases
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idClase == id);

                if(original != null) {
                    c = corregir(c);
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
    [HttpGet("get")]
    public async Task<ActionResult<Clase>> obtener() {
        try {
            var listaClases = await context.clases
                .OrderBy(x => x.nombre)
                .ToListAsync();

            return Ok(listaClases);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Clase>> obtenerId(int id) {
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

    [HttpGet("check_nombre/{nombre}")]
    public async Task<ActionResult<Clase>> obtenerNombre(string nombre) {
        try {
            Clase c = context.clases
                .FirstOrDefault(x => x.nombre == nombre);
            
            if(c != null) {
                return Ok(c);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    public Clase corregir(Clase c) {
        c.modVida = c.modVida > 100 ? c.modVida/100 : c.modVida/10;
        c.modEnergia = c.modEnergia > 100 ? c.modEnergia/100 : c.modEnergia/10;
        c.modAtk = c.modAtk > 100 ? c.modAtk/100 : c.modAtk/10;
        c.modAtm = c.modAtm > 100 ? c.modAtm/100 : c.modAtm/10;
        c.modDef = c.modDef > 100 ? c.modDef/100 : c.modDef/10;
        c.modDfm = c.modDfm > 100 ? c.modDfm/100 : c.modDfm/10;
        c.modDex = c.modDex > 100 ? c.modDex/100 : c.modDex/10;
        c.modEva = c.modEva > 100 ? c.modEva/100 : c.modEva/10;
        c.modCrt = c.modCrt > 100 ? c.modCrt/100 : c.modCrt/10;
        c.modAcc = c.modAcc > 100 ? c.modAcc/100 : c.modAcc/10;

        return c;
    }
}
