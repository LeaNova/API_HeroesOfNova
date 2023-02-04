using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("tipo")]
public class TipoController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public TipoController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Obtener
    [HttpGet("get")]
    public async Task<ActionResult<Tipo>> obtener() {
        try {
            var listaTipos = await context.tipos.ToListAsync();

            return Ok(listaTipos);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}