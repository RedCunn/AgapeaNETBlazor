using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class Cliente :IValidatableObject
    {
        /*
         OJITO !!! las validaciones con DataAnnotations solo valen para propiedades de 1º nivel,
                --> para obj nested dentro de una clase 2 soluciones :

        IValidateObject => Implementar en la clase que hace de nest la validacion de prop. nested obj con la interface IValidateObject 
                        que eexige implementar metodo Validate(..) que se lanza cada vez q se intenta validar una prop del obj que hace de nest, cliente en este caso
        
        Validator personalizado => imagina que no te dejan tocar la clase modelo Cliente, tienes que crearte un componente validador personalizado, que no tiene vista 
                                   solo codigo, al que le pasas el EditContext con el modelo Cliente q va en su interior (como CascadingValue)
                                    - debe tener como prop un objeto ValidationMessageStore -> lista de errores de validacion
                                    - tienes que añadir manejadores de evento al EditContext -> .OnValidRequest += (sender, eventargs) => {....}
                                                                                             \-> .OnFieldChanged += (sender, eventargs) => {....}                        
         */
        #region ... PROPS

        public String IdCliente { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "* el Nombre es obligatorio")]
        [MaxLength(50, ErrorMessage = "* no se admiten más de 50 caracteres en el nombre")]
        public String Nombre { get; set; } = "";

        [Required(ErrorMessage = "* los Apellidos son obligatorios")]
        [MaxLength(50, ErrorMessage = "* no se admiten más de 50 caracteres en los apellidos")]
        public String Apellidos { get; set; } = "";

        [Required(ErrorMessage = "* el teléfono es obligatorio")]
        public String Telefono { get; set; } = "";
        public Cuenta Credentials { get; set; } = new Cuenta();
        
        public String Genero { get; set; } = "";
        public String Descripcion { get; set; } = ""; 
        public DateTime FechaNacimiento {  get; set; }
        public List<Direccion> DireccionesCliente { get; set; } = new List<Direccion>();

        public List<Pedido> PedidosCliente { get; set; } = new List<Pedido>();


        #endregion



        #region ...METODOS
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // el resultado de la validacion del obj cliente por medio de DataAnnotations es una lista de objs de tipo ValidationResult 
            // --> asi que creo lista para objs props nested , como Credentials :
            List<ValidationResult> _errorList = new List<ValidationResult>();
            // -> para provocar la validacion desde codigo hay una clase Validator con metodos para validar props, objs...
            Validator.TryValidateObject(this.Credentials, new ValidationContext(this.Credentials), _errorList, true);

            return _errorList;

        }
        #endregion
    }
}
