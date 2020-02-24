using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Healthy2020.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Ciut { get; set; }
        public string Mail { get; set; }
        public string Domicilio { get; set; }
        public string Telefono { get; set; }
    }
}
