using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaJWT.Models.Custom;
using PruebaJWT.Services;

namespace PruebaJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IAutorizacionService _autorizacionService;

        // Constructor que se encarga de mandar la información a la variable
        public UsuariosController(IAutorizacionService autorizacionService)
        {
            _autorizacionService = autorizacionService;
        }

        [HttpPost]
        [Route("Autenticar")]
        // Este metodo sirve para recibir las credenciales del usuario y si está ok devolver el token
        public async Task<ActionResult> Autenticar([FromBody] AutorizacionRequest autorizacion)
        {
            var resultadoAutorizacion = _autorizacionService.DevolverToken(autorizacion);
            if(resultadoAutorizacion == null)
            {
                return Unauthorized();  
            }
            return Ok(resultadoAutorizacion); 
        }
    }
}

// El Refresh Token actua como un equivalente a las credenciales que se va a encargar de generar los tokens, está almacenada internamente dentro de la aplicacion y no debe ser visualizada por el usuario. Ademas el Refresh Token debe tener mas vida que el JWT

// En SQL se hace una tabla_prueba con la fecha de creacion y la fecha de expiracion, y una columna de esActivo que va a contener una condicion para ver si la fecha de expiracion pasó o no 