using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class Pedido
    {
        #region...PROPS
        public String IdPedido { get; set; } = Guid.NewGuid().ToString();
        public String IdCliente { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public List<Item> ElementosPedido { get; set; } = new List<Item>();

        public Boolean RecogerEnTienda { get; set; } = false;
        public Direccion? DireccionEnvio { get; set; }
        public Direccion? DireccionFacturacion { get; set; }

        public Decimal Subtotal { get; set; } = 0;

        public Decimal GastosEnvio { get; set; } = 0;
        public Decimal Total { get; set; } = 0;
        public String EstadoPedido { get; set; } = "En preparación";

        public int NumItems { get; set; } = 0;
        #endregion

        #region...METODOS DE CLASE

        public int ContarItems()
        {
            int conteo = 0;

            foreach (Item item in ElementosPedido)
            {
                conteo += item.CantidadItem;
            }
            return this.NumItems = conteo;
        }

        public Decimal CalcularSubtotalPedido()
        {
            //Decimal _subtotal = 0;
            //foreach (Item item in this.ElementosPedido)
            //{
            //    _subtotal += item.LibroItem.Precio * item.CantidadItem;
            //}
            //return _subtotal;

            return this.ElementosPedido.Sum((Item item) => item.LibroItem.Precio * item.CantidadItem);
        }

        public Decimal CalcularGastosEnvio()
        {
            if (this.RecogerEnTienda == true)
            {
                return this.GastosEnvio = 0;
            }
            return this.GastosEnvio = 3;
        }

        public Decimal CalcularTotalPedido()
        {
            this.Subtotal = this.CalcularSubtotalPedido();
            this.Total = this.Subtotal + this.GastosEnvio;
            return this.Total;
        }

        #endregion

        #region...METODOS DE ACCION
        #region...METODOS PRIVADOS
        #endregion
        #endregion
    }
}
