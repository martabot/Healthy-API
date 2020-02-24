using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Healthy2020.Models
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Actividad> Actividad { get; set; }
        public DbSet<Evento> Evento { get; set; }
        public DbSet<Participante> Participante { get; set; }
        public DbSet<InscripcionEvento> InscripcionEvento { get; set; }
        public DbSet<MedallaVirtual> MedallaVirtual { get; set; }
    }
}
