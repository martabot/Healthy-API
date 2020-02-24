using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Healthy2020.Models
{
    public class Actividad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public DateTime FechaUltMod { get; set; }
        public int Estado { get; set; }
        public int CoordinadorId { get; set; }
        [ForeignKey("CoordinadorId")]
        public Usuario Coordinador { get; set; }
    }
}
