using System.Collections;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("item")]
public class ItemController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public ItemController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] Item i) {
        try {
            if(ModelState.IsValid) {
                i = corregir(i);
                context.items.Add(i);
                context.SaveChanges();
                
                return CreatedAtAction(nameof(obtenerId), new { id = i.idItem }, i);
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
            Item i = context.items
                .AsNoTracking()
                .FirstOrDefault(x => x.idItem == id);

            if(i != null) {
                context.items.Remove(i);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("baja/{id}")]
    public async Task<ActionResult<Item>> baja(int id) {
        try {
            Item i = context.items
                .AsNoTracking()
                .FirstOrDefault(x => x.idItem == id);

            if(i != null) {
                i.disponible = !i.disponible;

                context.items.Update(i);
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
    public async Task<IActionResult> modificar([FromForm] Item i, int id) {
        try {
            if(ModelState.IsValid) {
                Item original = context.items
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idItem == id);
                
                if(original != null) {
                    i = corregir(i);
                    i.idItem = id;
                    i.tipoId = original.tipoId;

                    context.items.Update(i);
                    await context.SaveChangesAsync();

                    return Ok(i);
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
    public async Task<ActionResult<Item>> obtener() {
        try {
            var listaItems = new List<Item>();

            if(User.IsInRole("Admin")) {
                listaItems = await context.items
                    .Include(x => x.tipo)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            } else {
                listaItems = await context.items
                    .Include(x => x.tipo)
                    .Where(x => x.disponible)
                    .OrderBy(x => x.nombre)
                    .ToListAsync();
            }

            return Ok(listaItems);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Item>> obtenerId(int id) {
        try {
            Item i = context.items
                .Include(x => x.tipo)
                .FirstOrDefault(x => x.idItem == id);
            
            if(i == null) {
                return BadRequest("Objeto vacío");
            }

            return Ok(i);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("search/{nombre}")]
    public async Task<ActionResult<Item>> obtenerBusqueda(string nombre) {
        try {
            var listaItems = new List<Item>();
            
            if(User.IsInRole("Admin")) {
                listaItems = await context.items
                    .Include(x => x.tipo)
                    .Where(x => x.nombre.Contains(nombre))
                    .ToListAsync();
            } else {
                listaItems = await context.items
                    .Include(x => x.tipo)
                    .Where(x => x.nombre.Contains(nombre) && x.disponible)
                    .ToListAsync();
            }

            if(listaItems.Count() > 0) {
                return Ok(listaItems);
            }

            return BadRequest("Sin resultados");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check/{nombre}")]
    public async Task<ActionResult<Item>> check(string nombre) {
        try {
            Item i = context.items
                .FirstOrDefault(x => x.nombre == nombre);
            
            if(i == null) {
                return BadRequest("Objeto vacío");
            }

            return Ok(i);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    private Item corregir(Item i) {
        i.peso = i.peso > 100 ? i.peso/100 : i.peso/10; 

        return i;
    }
}