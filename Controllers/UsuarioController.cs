using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MySqlConnector;

namespace API_HeroesOfNova;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("usuario")]
public class UsuarioController : ControllerBase {
    private readonly DataContext context;
    private readonly IConfiguration configuration;

    public UsuarioController(DataContext context, IConfiguration configuration) {
        this.context = context;
        this.configuration = configuration;
    }

    //Alta
    [HttpPost("signin")]
    [AllowAnonymous]
    public async Task<IActionResult> alta([FromForm] Usuario u) {
        try {
            if(ModelState.IsValid && isValid(new UsuarioView(u))) {
                string pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
					password: u.pass,
					salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
					prf: KeyDerivationPrf.HMACSHA1,
					iterationCount: 1000,
					numBytesRequested: 256 / 8));
                
                u.pass = pass;
                u.fechaCreado = DateTime.Now;

                context.usuarios.Add(u);
                context.SaveChanges();

                return CreatedAtAction(nameof(obtener), new { id = u.idUsuario }, u);
            }
            return BadRequest("Error en datos");
        } catch (MySqlException ex) {
            return BadRequest(ex.Message);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        } 
    }

    //Baja

    //Modificacion
    [HttpPost("actualizar/perfil")]
    public async Task<IActionResult> actualizarPerfil([FromForm] UsuarioView uView) {
        try {
            if(ModelState.IsValid && isValid(uView)) {
                Usuario original = await context.usuarios
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.mail == "mail");
            }
            return BadRequest("Error en datos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Busquedas
    [HttpGet("users")]
    [AllowAnonymous]
    public async Task<ActionResult<Usuario>> obtenerTodos() {
        try {
            var listaUsuarios = await context.usuarios
                .Include(x => x.rol)
                .ToListAsync();

            return Ok(listaUsuarios);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("get")]
    [AllowAnonymous]
    public async Task<ActionResult<Usuario>> obtener() {
        try {
            int id = Int32.Parse(User.Claims.First(x => x.Type == "id").Value);
            Usuario u = context.usuarios
            .Include(x => x.rol)
            .FirstOrDefault(x => x.idUsuario == id);

            UsuarioView uView = new UsuarioView(u);
            return Ok(uView);
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check_mail/{mail}")]
    [AllowAnonymous]
    public async Task<ActionResult<Usuario>> obtenerMail(string mail) {
        try {
            Usuario u = context.usuarios
            .Include(x => x.rol)
            .FirstOrDefault(x => x.mail == mail);

            if(u != null) {
                UsuarioView uView = new UsuarioView(u);
                return Ok(uView);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check_usuario/{usuario}")]
    [AllowAnonymous]
    public async Task<ActionResult<Usuario>> obtenerUsuario(string usuario) {
        try {
            Usuario u = context.usuarios
            .Include(x => x.rol)
            .FirstOrDefault(x => x.usuario == usuario);

            if(u != null) {
                UsuarioView uView = new UsuarioView(u);
                return Ok(uView);
            }
            
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Log in
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> login([FromForm] LoginView lView) {
        try {
            Usuario u = await context.usuarios
                .Include(x => x.rol)
                .FirstOrDefaultAsync(x => x.mail == lView.mail);

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: lView.pass,
				salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
				prf: KeyDerivationPrf.HMACSHA1,
				iterationCount: 1000,
				numBytesRequested: 256 / 8));

            if(u != null && u.pass == hashed) {
                var key = new SymmetricSecurityKey(
					System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
				var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

				var claims = new List<Claim> {
					new Claim(ClaimTypes.Name, u.nombre + " " + u.apellido),
					new Claim("mail", u.mail),
					new Claim("id", u.idUsuario.ToString()),
					new Claim(ClaimTypes.Role, u.rol.nombre)
				};

				var token = new JwtSecurityToken(
					issuer: configuration["TokenAuthentication:Issuer"],
					audience: configuration["TokenAuthentication:Audience"],
					claims: claims,
					expires: DateTime.Now.AddMinutes(60),
					signingCredentials: credenciales
				);

				return Ok(new JwtSecurityTokenHandler().WriteToken(token));
            }
            return BadRequest("Usuario o contraseña incorrecto");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Validacion
    private bool isValid(UsuarioView uView) {
        if(uView.nombre.Any(char.IsDigit)) {
            return false;
        }
        if(uView.apellido.Any(char.IsDigit)) {
            return false;
        }
        if(!(uView.mail.Contains("@gmail") || uView.mail.Contains("@hotmail"))) {
            return false;
        }
        if(uView.rolId < 1) {
            return false;
        }
        return true;
    }
}
