using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Healthy2020.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public int ActividadId { get; set; }
        [ForeignKey("ActividadId")]
        public Actividad Actividad { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaUltMod { get; set; }
        public int Estado { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
    }
}
