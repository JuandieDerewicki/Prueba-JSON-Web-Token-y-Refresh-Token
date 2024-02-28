using PruebaJWT.Models.Custom;

namespace PruebaJWT.Services
{
    public interface IAutorizacionService
    {
        Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion); 
    }
}
