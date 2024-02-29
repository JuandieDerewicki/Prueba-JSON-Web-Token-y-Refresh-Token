using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using PruebaJWT.Models;
using PruebaJWT.Models.Custom;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using Microsoft.OpenApi.Writers;

namespace PruebaJWT.Services
{
    public class AutorizacionService : IAutorizacionService
    {
        // Declaramos las variables ya que vamos a estar usando nuestro contexto de la bd, y tmb tenemos que leer la llave que tenemos en el appsettings

        // Se recibe el valor de esto mediante un constructor 
        private readonly DbpruebaJwtContext _context;
        private readonly IConfiguration _configuration;

        public AutorizacionService(DbpruebaJwtContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Metodo para generar el token 
        private string GenerarToken(string idUsuario)
        {
            // Declaramos la key pq vamos a acceder a ese valor en appsettings
            var key = _configuration.GetValue<String>("JwtSettings:Key");
            
            // Convertimos la llave en un array
            var keyBytes = Encoding.ASCII.GetBytes(key);

            // Crear informacion del usuario para nuestro token
            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier,idUsuario)); // De esta forma identificamos al usuario

            // Credencial para nuestro token donde le pasamos dos parametros
            var credencialesToken = new SigningCredentials(
            new SymmetricSecurityKey(keyBytes), 
            SecurityAlgorithms.HmacSha256Signature
            );

            // Creacion de detalle del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Ponemos todas las descripciones pasandoles los claims que tiene la info del idusuario
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(1), // Expiracion del token 
                SigningCredentials = credencialesToken // Credenciales del token 
            };

            // Creacion de controladores de JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            // Este token recibe el token handler y se crea con toda la configuracion del tokenDescriptor
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

            // Obtener el token
            string tokenCreado = tokenHandler.WriteToken(tokenConfig);

            // Devolvemos el token creado
            return tokenCreado;
        }

        public string GenerarRefreshToken()
        {
            var byteArray = new byte[64]; // Contenido de 64
            var refreshToken = ""; // Aca va a ir el RefreshToken

            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(byteArray); // Le pasamos el array de bytes   
                refreshToken = Convert.ToBase64String(byteArray); // Obtenemos el refresh token 
            }
            return refreshToken;    
        }

        // Metodo para guardar dentro de nuestra tabla HistorialRefreshToken en la base de datos
        private async Task<AutorizacionResponse> GuardarHistorialRefreshToken(int idUsuario, string token, string refreshToken)
        {
            var historialRefreshToken = new HistorialRefreshToken
            {
                RefreshToken = refreshToken,
                Token = token,
                IdUsuario = idUsuario,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(2),
            }; // Historial para poder crear con los datos de nuestra tabla un nuevo historial de refresh token en nuestra tabla

            await _context.HistorialRefreshTokens.AddAsync(historialRefreshToken); // enviamos a la tabla
            await _context.SaveChangesAsync(); // guardamos los cambios de manera asincrona

            // Retornamos una autorizacion response pasandole todo lo que recibimos por parametro
            return new AutorizacionResponse { Token = token, RefreshToken = refreshToken, Resultado = true, Msg = "Ok" };
        }

        public async Task<AutorizacionResponse> DevolverToken(AutorizacionRequest autorizacion)
        {
            // Vamos a encontrar el primero y sino retorna un nulo
            var usuario_encontrado = _context.Usuarios.FirstOrDefault(x =>
                x.NombreUsuario == autorizacion.NombreUsuario && 
                x.Clave == autorizacion.Clave
                // Devuelve el usuario cuando coincide con las dos, y en el caso que exista se almacena en usuario_encontrado
            );

            if(usuario_encontrado == null)
            {
                return await Task.FromResult<AutorizacionResponse>(null); // retorno una rta cuando el usuario no se encuentra
            }

            string tokenCreado = GenerarToken(usuario_encontrado.IdUsuario.ToString()); // Llamamos al metodo y le pasamos el usuario con sus parametros

            // Ahora hacemos la logica para que tambien devuelva el refresh token creado 
            string refreshTokenCreado = GenerarRefreshToken();

            // Devolvemos el token y le pasamos el token con el resultado y el mensaje creando un nueva autorizacionresponse
            //return new AutorizacionResponse() {  Token = tokenCreado, Resultado = true, Msg = "Ok" };

            // Devolvemos la rta del metodo
            return await GuardarHistorialRefreshToken(usuario_encontrado.IdUsuario, tokenCreado, refreshTokenCreado);   
        }

        public async Task<AutorizacionResponse> DevolverRefreshToken(RefreshTokenRequest refreshTokenRequest, int idUsuario)
        {
            // Validar si existe el refresh token que estamos enviando, donde primero validamos con la base de datos
            var refreshTokenEncontrado = _context.HistorialRefreshTokens.FirstOrDefault(x =>
            x.Token == refreshTokenRequest.TokenExpirado && // el token de la bd tiene que ser igual al expirado
            x.RefreshToken == refreshTokenRequest.RefreshToken && // Tiene que existir el refresh token 
            x.IdUsuario == idUsuario);  // El token tiene que corresponder a este usuario
            
            if(refreshTokenEncontrado == null)
            {
                return new AutorizacionResponse { Resultado = false, Msg = "No existe el refreshToken" };
            }

            // Generamos el refresh token y el token 
            var refreshTokenCreado = GenerarRefreshToken();
            var tokenCreado = GenerarToken(idUsuario.ToString());

            // Llamamos al metodo 
            return await GuardarHistorialRefreshToken(idUsuario, tokenCreado, refreshTokenCreado);

            // Con esto ya tendriamos el Refresh Token y podemos enviarle al usuario cada vez que solicite un acess token, un jwt junto con su refresh token 
        }
    }
}
