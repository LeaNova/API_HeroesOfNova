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
[AllowAnonymous]
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
    [HttpPut("actualizar/perfil")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> actualizarPerfil([FromForm] UsuarioEdit uEdit) {
        try {
            if(ModelState.IsValid && isValid(uEdit)) {
                string mail = User.Claims.First(x => x.Type == "mail").Value;
                Usuario original = await context.usuarios
                    .Include(x => x.rol)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.mail == mail);

                if(original != null) {
                    original.nombre = uEdit.nombre;
                    original.apellido = uEdit.apellido;
                    original.usuario = uEdit.usuario;

					var key = new SymmetricSecurityKey(
						System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"]));
					var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

					var claims = new List<Claim> {
						new Claim(ClaimTypes.Name, original.nombre + " " + original.apellido),
                        new Claim("usuario", original.usuario),
                        new Claim("mail", original.mail),
						new Claim("id", original.idUsuario.ToString()),
						new Claim(ClaimTypes.Role, original.rol.nombre),
					};

					var token = new JwtSecurityToken(
						issuer: configuration["TokenAuthentication:Issuer"],
						audience: configuration["TokenAuthentication:Audience"],
						claims: claims,
						expires: DateTime.Now.AddMinutes(60*8),
						signingCredentials: credenciales
					);

                    context.usuarios.Update(original);
                    context.SaveChanges();

                    return Ok(original);
                }

                return BadRequest("Usuario ineccistente");
            }
            return BadRequest("Error en datos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("actualizar/contraseña")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> actualizarContraseña([FromForm] UsuarioPass uPass) {
        try {
            if(ModelState.IsValid) {
                string mail = User.Claims.First(x => x.Type == "mail").Value;
                Usuario original = await context.usuarios
                    .Include(x => x.rol)
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.mail == mail);

                if(original != null) {
                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: uPass.oldPass,
                        salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                        prf: KeyDerivationPrf.HMACSHA1,
                        iterationCount: 1000,
                        numBytesRequested: 256 / 8));
                    
                    if(original.pass == hashed) {
                        original.pass = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                            password: uPass.newPass,
                            salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
                            prf: KeyDerivationPrf.HMACSHA1,
                            iterationCount: 1000,
                            numBytesRequested: 256 / 8));
                        
                        context.usuarios.Update(original);
                        await context.SaveChangesAsync();

                        return Ok(original);
                    }
                    return BadRequest("Contraseña incorrecta");
                }
                return BadRequest("Usuario ineccistente");
            }
            return BadRequest("Error en datos");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Busquedas
    [HttpGet("get")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

    [HttpGet("get/{id}")]
    public async Task<ActionResult<Usuario>> obtenerId(int id) {
        try {
            Usuario u = context.usuarios
            .Include(x => x.rol)
            .FirstOrDefault(x => x.idUsuario == id);

            if(u != null) {
                UsuarioView uView = new UsuarioView(u);
                return Ok(uView);
            }

            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("check_mail/{mail}")]
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

    [HttpGet("check_cambio/{usuario}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<Usuario>> obtenerCambioDisponible(string usuario) {
        try {
            string uOriginal = User.Claims.First(x => x.Type == "usuario").Value;
            Usuario u = context.usuarios
                .Include(x => x.rol)
                .FirstOrDefault(x => x.usuario == usuario && usuario != uOriginal);

            if(u != null) {
                UsuarioView uView = new UsuarioView(u);
                return Ok(uView);
            }
            
            return BadRequest("Objeto vacío");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("users")] //Uso para ADMIN
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<ActionResult<Usuario>> obtenerTodos() {
        try {
            if(User.IsInRole("Admin")) {
                var listaUsuarios = await context.usuarios
                    .Include(x => x.rol)
                    .ToListAsync();

                return Ok(listaUsuarios);
            }
            return BadRequest("XD");
        } catch (Exception ex) {
            return BadRequest(ex.Message);
        }
    }

    //Log in
    [HttpPost("login")]
    public async Task<IActionResult> login([FromForm] LoginView lView) {
        try {
            Usuario u = await context.usuarios
                .Include(x => x.rol)
                .FirstOrDefaultAsync(x => x.usuario == lView.usuario);

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
                    new Claim("usuario", u.usuario),
					new Claim("mail", u.mail),
					new Claim("id", u.idUsuario.ToString()),
					new Claim(ClaimTypes.Role, u.rol.nombre)
				};

				var token = new JwtSecurityToken(
					issuer: configuration["TokenAuthentication:Issuer"],
					audience: configuration["TokenAuthentication:Audience"],
					claims: claims,
					expires: DateTime.Now.AddMinutes(60*8),
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

    private bool isValid(UsuarioEdit uEdit) {
        if(uEdit.nombre.Any(char.IsDigit)) {
            return false;
        }
        if(uEdit.apellido.Any(char.IsDigit)) {
            return false;
        }
        return true;
    }
}
