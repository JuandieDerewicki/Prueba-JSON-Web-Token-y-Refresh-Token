using Microsoft.EntityFrameworkCore;
using PruebaJWT.Models;


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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
