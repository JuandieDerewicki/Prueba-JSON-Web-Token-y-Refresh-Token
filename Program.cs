using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PruebaJWT.Models;
using PruebaJWT.Services;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Referencia del servicio de la base de datos
// A nuestro contexto de nuestra bd le decimos que puede utilizar esa cadena declarada en appsetings.json
builder.Services.AddDbContext<DbpruebaJwtContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSql"));
});

// Llamado al servicio para que pueda ser usado 
builder.Services.AddScoped<IAutorizacionService, AutorizacionService>();

// Configuramos el JWT para poder usarlo dentro del proyecto
// Primero mostramos la forma de como obtenemos la llave secreta pero se hace un cambio pq el origen es diferente 
var key = builder.Configuration.GetValue<String>("JwtSettings:Key"); // Declaramos la key pq vamos a acceder a ese valor en appsettings
var keyBytes = Encoding.ASCII.GetBytes(key); // Convertimos la llave en un array

// Agregamos la autenticacion con sus configuraciones
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => // Configuraciones para el JSON Web Tokens Bearer
{
    config.RequireHttpsMetadata = false; // Sacamos el requerimiento de http
    config.SaveToken = true; // Guardamos el token 
    config.TokenValidationParameters = new TokenValidationParameters // Configuraciones para el token por validacion por parametros
    {
        ValidateIssuerSigningKey = true, // Validamos el usuario ya que usamos credenciales
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes), 
        ValidateIssuer = false, // No nos interesa validar quien solicita, ya que nosotros hacemos la autenticacion en base a credenciales
        ValidateAudience = false, // No necesitamos validar desde donde solicta el usuario 
        ValidateLifetime = true, // Validamos el tiempo de vida del token 
        ClockSkew = TimeSpan.Zero // No debe existir ningun tiempo de desviacion en cuanto al tiempo de creacion del token
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // validamos que se use la autenticacion

app.UseAuthorization();

app.MapControllers();

app.Run();
