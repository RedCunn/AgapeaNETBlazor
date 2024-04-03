using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace Agapea_Blazor.Server.Models
{
    public class MiClienteIdentity : IdentityUser
    {
        //clase personalizada para añadir sobre las props. de IdentityUser datos propios q me interesan y q Identity no refleja
        #region ... props nuevas que añadimos a clase IdentityUser
        public String Nombre { get; set; } 
        public String Apellidos { get; set; }
        public String NIF { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public String Genero { get; set; }
        public String Descripcion { get; set; }
        public String ImagenAvatarBASE64 {  get; set; }
        #endregion
    }
}
