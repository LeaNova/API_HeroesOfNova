using API_HeroesOfNova;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

configuration.AddUserSecrets(System.Reflection.Assembly.GetExecutingAssembly());

// Add services to the container.
builder.WebHost.ConfigureKestrel(serverOptions =>
{
	serverOptions.ListenAnyIP(5000);
	serverOptions.ListenAnyIP(5001, listenOptions => listenOptions.UseHttps() );
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddJwtBearer(options => {
			options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
				ValidateIssuer = true,
				ValidateAudience = true,
				ValidateLifetime = true,
				ValidateIssuerSigningKey = true,
				ValidIssuer = configuration["TokenAuthentication:Issuer"],
				ValidAudience = configuration["TokenAuthentication:Audience"],
				IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(
					configuration["TokenAuthentication:SecretKey"]))
			};
            options.Events = new JwtBearerEvents {
                OnMessageReceived = context => {
                    var accessToken = context.Request.Query["access_token"];
                    var path = context.HttpContext.Request.Path;
                    if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/Propietario/token")) {
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                }
            };
        });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<DataContext>(
    dbContextOptions => dbContextOptions
        .UseMySql(
		configuration["ConnectionStrings:DefaultConnection"],
		ServerVersion.AutoDetect(configuration["ConnectionStrings:DefaultConnection"]))
        .LogTo(Console.WriteLine, LogLevel.Information)
        .EnableSensitiveDataLogging()
        .EnableDetailedErrors()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
