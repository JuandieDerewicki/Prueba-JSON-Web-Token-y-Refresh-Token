namespace PruebaJWT.Models.Custom
{
    public class AutorizacionResponse
    {
        // Va a ser la respuesta luego de que el usuario haya ingresado las credenciales 
        public string Token { get; set; }
        public bool Resultado { get; set; }

        public string Msg {  get; set; }

        // Luego en el appsettings vamos a crear la configuración, con la llave para poder crear nuestro JWT
    }
}
