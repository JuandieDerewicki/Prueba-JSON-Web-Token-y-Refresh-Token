namespace PruebaJWT.Models.Custom
{
    public class AutorizacionRequest
    {
        // Esto va a ser utilizado al momento de recibir la información del formulario de logeo
        public string NombreUsuario { get; set; }
        public string Clave { get; set; }   
    }
}
