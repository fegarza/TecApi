using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Atenciones
    {
        public Atenciones()
        {
            Canalizaciones = new HashSet<Canalizaciones>();
        }

        public byte Id { get; set; }
        public string Titulo { get; set; }
        public byte AreaId { get; set; }

        public virtual Areas Area { get; set; }
        public virtual ICollection<Canalizaciones> Canalizaciones { get; set; }
    }
}
