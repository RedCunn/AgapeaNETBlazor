using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class Cuenta
    {

        #region ...PROPS

        [Required(ErrorMessage = "* el email es obligado")]
        [EmailAddress(ErrorMessage = "* formato de email inválido")]
        public String email { get; set; } = "";

        [Required(ErrorMessage = "* la contraseña es obligada")]
        [MinLength(8, ErrorMessage = "* la contraseña ha de tener mínimo de 8 caracteres")]
        [MaxLength(100, ErrorMessage = "* no se admite una contraseña tan larga")]
        [RegularExpression(@"^(?=.*\d)(?=.*[\u0021-\u002b\u003c-\u0040])(?=.*[A-Z])(?=.*[a-z])\S{8,}$", ErrorMessage ="* la contraseña debe contener minus. y MAYUS.")]
        public String password { get; set; } = "";
        public String login { get; set; } = "";
        public Boolean isActive { get; set; } = false;
        public String accountPic { get; set; } = "";

        #endregion

        #region ...metodos...

        #endregion
    }
}
