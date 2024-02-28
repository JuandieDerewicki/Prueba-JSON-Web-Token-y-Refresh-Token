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
