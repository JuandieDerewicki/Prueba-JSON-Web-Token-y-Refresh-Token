using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PruebaJWT.Models.Custom;
using PruebaJWT.Services;
using System.IdentityModel.Tokens.Jwt;

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


        // Este metodo permite obtener el token y el refresh token a traves de las credenciales
        [HttpPost]
        [Route("Autenticar")]
        // Este metodo sirve para recibir las credenciales del usuario y si está ok devolver el token
        public async Task<ActionResult> Autenticar([FromBody] AutorizacionRequest autorizacion)
        {
            var resultadoAutorizacion = await _autorizacionService.DevolverToken(autorizacion);
            if(resultadoAutorizacion == null)
            {
                return Unauthorized();  
            }
            return Ok(resultadoAutorizacion); 
        }

        // Este metodo permite obtener otro token en base al refresh token
        [HttpPost]
        [Route("ObtenerRefreshToken")]
        public async Task<ActionResult> ObtenerRefreshToken([FromBody] RefreshTokenRequest request)
        {
            // Validar si el token expiró o no, ya que no deberiamos generar uno nuevo si no expiró
            var tokenHandler = new JwtSecurityTokenHandler(); // este nos ayuda a obtener esa informacion
            var tokenExpiradoSupuestamente = tokenHandler.ReadJwtToken(request.TokenExpirado); // Leemos el token que el usuario ha enviado que supuestamente expiró 

            if (tokenExpiradoSupuestamente.ValidTo > DateTime.UtcNow) // Validacion de si expiró o no
            {
                return BadRequest(new AutorizacionResponse { Resultado = false, Msg = "Token no ha expirado" });
            }

            // En caso de que el token ha expirado, necesitamos obtener el idusuario que se encuentra almacenado dentro del JWT que estamos validando ya que tenemos que generar un refresh token con el idusuario
            string idUsuario = tokenExpiradoSupuestamente.Claims.First(x =>
            x.Type == JwtRegisteredClaimNames.NameId).Value.ToString(); // En claims guardamos la info del usuario que genera el token

            // generamos la respuesta del token
            var autorizacionResponse = await _autorizacionService.DevolverRefreshToken(request, int.Parse(idUsuario));

            // Para devolver validamos en el resultado
            if(autorizacionResponse.Resultado)
            {
                return Ok(autorizacionResponse);
            }
            else
            {
                return BadRequest(autorizacionResponse);
            }
        }
    }
}

// El Refresh Token actua como un equivalente a las credenciales que se va a encargar de generar los tokens, está almacenada internamente dentro de la aplicacion y no debe ser visualizada por el usuario. Ademas el Refresh Token debe tener mas vida que el JWT

// En SQL se hace una tabla_prueba con la fecha de creacion y la fecha de expiracion, y una columna de esActivo que va a contener una condicion para ver si la fecha de expiracion pasó o no 