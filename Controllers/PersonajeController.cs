using System.Collections.Generic;
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

                await setEquipamiento(p);

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
            Personaje p = await context.personajes.FirstOrDefaultAsync(x => x.idPersonaje == id && x.usuarioId == idUsuario);
            
            if(p != null) {
                var participo = context.participantes
                    .FirstOrDefault(x => x.usuarioId == idUsuario && x.personajeId == p.idPersonaje);

                var armaduras = context.invArmaduras
                    .Where(x => x.personajeId == p.idPersonaje)
                    .ToList();

                var armas = context.invArmas
                    .Where(x => x.personajeId == p.idPersonaje)
                    .ToList();

                var items = context.invItems
                    .Where(x => x.personajeId == p.idPersonaje)
                    .ToList();
                    
                var artefactos = context.invArtefactos
                    .Where(x => x.personajeId == p.idPersonaje)
                    .ToList();

                if(participo != null) context.participantes.Remove(participo);

                context.invArmaduras.RemoveRange(armaduras);
                context.invArmas.RemoveRange(armas);
                context.invItems.RemoveRange(items);
                context.invArtefactos.RemoveRange(artefactos);
                context.personajes.Remove(p);

                context.SaveChanges();

                return Ok();
            }

            return BadRequest("Error en borrar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("baja/{id}")]
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
    [HttpPut("modificar/{idPersonaje}")]
    public async Task<ActionResult<Personaje>> modificar([FromForm] PersonajeUpdate personajeU, int idPersonaje) {
        try {
            if(ModelState.IsValid) {
                int usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
                Personaje original = context.personajes
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idPersonaje == idPersonaje && x.usuarioId == usuarioId);

                if(original != null) {
                    original.nombre = personajeU.nombre;
                    original.razaId = personajeU.razaId;
                    original.generoId = personajeU.generoId;
                    original.claseId = personajeU.claseId;

                    context.personajes.Update(original);
                    await context.SaveChangesAsync();

                    return Ok(original);
                }

                return BadRequest("Objeto vacío");
            }
            return BadRequest("Error en actualizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("finalizar/{idPersonaje}")]
    public async Task<ActionResult<Personaje>> finalizar([FromForm] PersonajeFinalizar personajeF, int idPersonaje) {
        try {
            if(ModelState.IsValid) {
                int usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
                Personaje original = context.personajes
                    .AsNoTracking()
                    .FirstOrDefault(x => x.idPersonaje == idPersonaje && x.usuarioId == usuarioId);

                if(original != null) {
                    original.nivel = personajeF.nivel;
                    original.experiencia = personajeF.experiencia;
                    original.vida = personajeF.vida;
                    original.energia = personajeF.energia;
                    original.ataque = personajeF.ataque;
                    original.atkMagico = personajeF.atkMagico;
                    original.defensa = personajeF.defensa;
                    original.defMagico = personajeF.defMagico;
                    original.agilidad = personajeF.agilidad;
                    original.evasion = personajeF.evasion;
                    original.critico = personajeF.critico;
                    original.precision = personajeF.precision;
                    original.mochilaId = personajeF.mochilaId;

                    context.personajes.Update(original);
                    await context.SaveChangesAsync();

                    return Ok(original);
                }

                return BadRequest("Objeto vacío");
            }
            return BadRequest("Error en finalizar");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("cambiar_mochila/{idPersonaje}")]
    public async Task<ActionResult<Personaje>> cambiarMochila([FromForm] int mochilaId, int idPersonaje) {
        try {
            int usuarioId = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            Personaje original = context.personajes
                .AsNoTracking()
                .FirstOrDefault(x => x.idPersonaje == idPersonaje && x.usuarioId == usuarioId);

            if(original != null) {
                original.mochilaId = mochilaId;

                context.personajes.Update(original);
                await context.SaveChangesAsync();

                //cambiarTodo(idPersonaje, mochilaId);

                return Ok(original);
            }
            
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

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

    private async Task<IActionResult> setEquipamiento(Personaje personaje) {
        try {
            InvArmadura invArmadura = new InvArmadura(personaje.mochilaId, personaje.idPersonaje, 1, 1);
            InvArma invArma = new InvArma(personaje.mochilaId, personaje.idPersonaje, 1, 1);
            var listaArtefactos = new List<InvArtefacto>();
            listaArtefactos.Add(new InvArtefacto(personaje.mochilaId, personaje.idPersonaje, 1, 1));
            listaArtefactos.Add(new InvArtefacto(personaje.mochilaId, personaje.idPersonaje, 2, 1));
            listaArtefactos.Add(new InvArtefacto(personaje.mochilaId, personaje.idPersonaje, 3, 1));
            listaArtefactos.Add(new InvArtefacto(personaje.mochilaId, personaje.idPersonaje, 4, 1));

            context.invArmaduras.Add(invArmadura);
            context.invArmas.Add(invArma);
            context.invArtefactos.AddRange(listaArtefactos);

            context.SaveChanges();

            return Ok();
        } catch(Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}