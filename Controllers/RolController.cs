using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Route("rol")]
public class RolController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public RolController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Obtener
    [HttpGet("get")]
    public async Task<ActionResult<Rol>> obtenerTodos() {
        try {
            var listaRoles = await context.roles.ToListAsync();

            return Ok(listaRoles);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}
