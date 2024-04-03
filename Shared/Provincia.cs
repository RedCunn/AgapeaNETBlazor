using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class Provincia
    {

        #region...PROPS (deben coincidir con props del json q nos devuelve el servicio externo)

        public String CCOM { get; set; } = "";
        public String CPRO { get; set; } = "";
        public String PRO { get; set; } = "";

        #endregion
    }
}
