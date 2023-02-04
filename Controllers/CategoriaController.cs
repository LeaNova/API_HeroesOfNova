using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("categoria")]
public class CategoriaController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public CategoriaController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Obtener
    [HttpGet("get")]
    public async Task<ActionResult<Categoria>> obtener() {
        try {
            var listaCategorias = await context.categorias.ToListAsync();

            return Ok(listaCategorias);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }
}