using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Healthy2020.Models
{
    public class Participante
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
        public int ActividadId { get; set; }
        [ForeignKey("ActividadId")]
        public Actividad Actividad { get; set; }
        public DateTime FechaUltMod { get; set; }
        public int Estado { get; set; }
    }
}
