using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Healthy2020.Models
{
    public class Usuario
    {

        public int Id { get; set; }
        public int Empresa { get; set; }
        public string Mail { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Dni { get; set; }
        public string Password { get; set; }
        public string Rol { get; set; }
        public int AdminUltModId { get; set; }
        [ForeignKey("AdminUltModId")]
        public Usuario AdminUltMod { get; set; }
        public string FechaUltMod { get; set; }
        public int Conducta { get; set; }
        public int EstadoCuenta { get; set; }
        public string FecNac { get; set; }
        public int Fumador { get; set; }
        public string FumaUltMod { get; set; }

    }
}
