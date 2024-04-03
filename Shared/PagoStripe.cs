using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class PagoStripe
    {
        public string IdCliente { get; set; }
        public string IdPedido { get; set; }

        public string ChargeId {get;set;}
    }
}
