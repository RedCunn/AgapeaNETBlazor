using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class OrderModel
    {
        #region ...propiedades clase ....
        //datos de envio
        public String TipoDireccionEnvio { get; set; } = "principal";
        public Direccion DireccionEnvio { get; set; } = new Direccion();
        public String NombreDestinatario { get; set; } = "";
        public String ApellidosDestinatario { get; set; } = "";
        public String TelefonoDestinatario { get; set; } = "";
        public String EmailDestinatario { get; set; } = "";

        //datos facturacion
        public String NombreFactura { get; set; } = "";
        public String DocFiscalFactura { get; set; } = "";
        public Direccion DireccionFacturacion { get; set; } = new Direccion();
        public String TipoDireccionFacturacion { get; set; } = "igualenvio";

        //datos pago
        public String metodoPago { get; set; } = "tarjeta";
        public String NumeroTarjeta { get; set; } = "";
        public String NombreBanco { get; set; } = "";
        public int MesCaducidad { get; set; } = 0;
        public int AnioCaducidad { get; set; } = 0;
        public int CVV { get; set; } = 0;

        #endregion
    }
}