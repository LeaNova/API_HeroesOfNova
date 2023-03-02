using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("rareza")]
public class RarezaController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public RarezaController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Obtener
    [HttpGet("get")]
    public async Task<ActionResult<Rareza>> obtener() {
        try {
            var listaRarezas = await context.rarezas.ToListAsync();

            return Ok(listaRarezas);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}