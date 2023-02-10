using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("seccion")]
public class SeccionController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public SeccionController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Obtener
    [HttpGet("get")]
    public async Task<ActionResult<Seccion>> obtener() {
        try {
            var listaSecciones = await context.secciones.ToListAsync();

            return Ok(listaSecciones);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}