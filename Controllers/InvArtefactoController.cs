using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("inventario/artefacto")]
public class InvArtefactoController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public InvArtefactoController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("crear")]
    public async Task<IActionResult> alta([FromForm] InvArtefacto invArtefacto) {
        try {
            if(ModelState.IsValid) {
                InvArtefacto original = context.invArtefactos
                    .FirstOrDefault(x => x.personajeId == invArtefacto.personajeId && x.artefactoId == invArtefacto.artefactoId);
                
                if(original != null) {
                    await añadir(original, invArtefacto.cantidad);

                    return Ok();
                }

                context.invArtefactos.Add(invArtefacto);
                context.SaveChanges();

                return Ok();
            }
            return BadRequest("Error en crear");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Baja
    [HttpDelete("borrar/personaje={personajeId}_artefacto={artefactoId}")]
    public async Task<IActionResult> borrar(int personajeId, int artefactoId) {
        try {
            InvArtefacto invArtefacto = context.invArtefactos
                .AsNoTracking()
                .FirstOrDefault(x => x.personajeId == personajeId && x.artefactoId == artefactoId);
            
            if(invArtefacto != null) {
                context.invArtefactos.Remove(invArtefacto);
                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Modificar

    //Busqueda
    [HttpGet("get/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvArtefacto>> obtenerMisArtefactos(int mochilaId, int personajeId) {
        try {
            var listaArtefactos = context.invArtefactos
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Include(x => x.artefacto).ThenInclude(x => x.seccion)
                .ToList();
            
            if(listaArtefactos.Count() > 0) {
                return Ok(listaArtefactos);
            }

            return BadRequest("Sin artefactos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/corona/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvArtefacto>> obtenerMisCoronas(int mochilaId, int personajeId) {
        try {
            var listaArtefactos = context.invArtefactos
                .Include(x => x.artefacto).ThenInclude(x => x.seccion)
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Where(x => x.artefacto.seccion.nombre == "Corona")
                .ToList();
            
            if(listaArtefactos.Count() > 0) {
                return Ok(listaArtefactos);
            }

            return BadRequest("Sin artefactos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/izquierda/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvArtefacto>> obtenerMisIzquierdas(int mochilaId, int personajeId) {
        try {
            var listaArtefactos = context.invArtefactos
                .Include(x => x.artefacto).ThenInclude(x => x.seccion)
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Where(x => x.artefacto.seccion.nombre == "Izquierda")
                .ToList();
            
            if(listaArtefactos.Count() > 0) {
                return Ok(listaArtefactos);
            }

            return BadRequest("Sin artefactos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/derecha/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvArtefacto>> obtenerMisDerechas(int mochilaId, int personajeId) {
        try {
            var listaArtefactos = context.invArtefactos
                .Include(x => x.artefacto).ThenInclude(x => x.seccion)
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Where(x => x.artefacto.seccion.nombre == "Derecha")
                .ToList();
            
            if(listaArtefactos.Count() > 0) {
                return Ok(listaArtefactos);
            }

            return BadRequest("Sin artefactos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get/adorno/mochila={mochilaId}/personaje={personajeId}")]
    public async Task<ActionResult<InvArtefacto>> obtenerMisAdornos(int mochilaId, int personajeId) {
        try {
            var listaArtefactos = context.invArtefactos
                .Include(x => x.artefacto).ThenInclude(x => x.seccion)
                .Where(x => x.mochilaId == mochilaId && x.personajeId == personajeId)
                .Where(x => x.artefacto.seccion.nombre == "Adorno")
                .ToList();
            
            if(listaArtefactos.Count() > 0) {
                return Ok(listaArtefactos);
            }

            return BadRequest("Sin artefactos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    public async Task<IActionResult> añadir(InvArtefacto invArtefacto, int cantidad) {
        try {
            invArtefacto.cantidad += cantidad;

            context.invArtefactos.Update(invArtefacto);
            context.SaveChanges();

            return Ok();
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}