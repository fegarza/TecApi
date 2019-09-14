using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Actividades
    {
        public Actividades()
        {
            Creditos = new HashSet<Creditos>();
            Grupos = new HashSet<Grupos>();
        }

        public int ActividadId { get; set; }
        public int SeccionId { get; set; }
        public string Titulo { get; set; }
        public byte Peso { get; set; }

        public virtual Secciones Seccion { get; set; }
        public virtual ICollection<Creditos> Creditos { get; set; }
        public virtual ICollection<Grupos> Grupos { get; set; }
    }
}
