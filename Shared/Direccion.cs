﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agapea_Blazor.Shared
{
    public class Direccion
    {

        #region...PROPS
        public String IdDireccion { get; set; } = Guid.NewGuid().ToString();
        public String IdCliente { get; set; } = "";
        public String Calle { get; set; } = "";
        public int CP { get; set; } = 0;
        public Provincia ProvinciaDirec { get; set; } = new Provincia();
        public Municipio MunicipioDirec { get; set; } = new Municipio();

        public String Pais { get; set; } = "España";
        public Boolean EsPrincipal { get; set; } = false;
        public Boolean EsFacturacion { get; set; } = false;
        #endregion
    }
}
