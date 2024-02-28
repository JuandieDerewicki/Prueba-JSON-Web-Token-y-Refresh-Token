using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PruebaJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaisesController : ControllerBase
    {
        [Authorize] // Solo puede ser consultado por aquel usuario que tenga un JWT y esté autenticado
        [HttpGet]
        [Route("Lista")]
        // Este metodo de lista solo deberia ser consultado por los usuarios autorizados
        public async Task<ActionResult> Lista()
        {
            var listaPaises = await Task.FromResult(new List<string> { "Francia", "Argentina", "Croacia", "Marruecos" });
            return Ok(listaPaises); 
        }
    }
}
