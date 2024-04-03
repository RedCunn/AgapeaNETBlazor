using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class RestMessage
    {
        // clase que mapea la respuesta de nuestros servicios RESTFULL creados en el server asp.net core
        #region ...PROPS
        public int Code { get; set; }
        public String Message { get; set; } = "";
        public String Error { get; set; } = "";
        public String SessionToken { get; set; } = "";
        public Cliente? ClientData { get; set; } 
        public Object? Others { get; set; }

        #endregion

        #region ...METODOS

        #endregion


    }
}
