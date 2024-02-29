namespace PruebaJWT.Models.Custom
{
    public class RefreshTokenRequest
    {
        public string TokenExpirado { get; set; }
        public string RefreshToken { get; set; }    

        // Para generar un token, siempre necesitamos el token anterior y pasarle el refresh token con el que vamos a generar un JWT
    }
}
 