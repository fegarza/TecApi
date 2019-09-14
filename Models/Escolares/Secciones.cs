using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Secciones
    {
        public Secciones()
        {
            Actividades = new HashSet<Actividades>();
        }

        public int SeccionId { get; set; }
        public string Titulo { get; set; }
        public byte? PesoMaximo { get; set; }

        public virtual ICollection<Actividades> Actividades { get; set; }
    }
}
