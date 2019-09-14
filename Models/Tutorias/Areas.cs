using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Areas
    {
        public Areas()
        {
            Atenciones = new HashSet<Atenciones>();
        }

        public byte Id { get; set; }
        public string Titulo { get; set; }
        public string Correo { get; set; }

        public virtual ICollection<Atenciones> Atenciones { get; set; }
    }
}
