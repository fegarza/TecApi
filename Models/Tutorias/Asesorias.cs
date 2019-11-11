using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Asesorias
    {
        public Asesorias()
        {
            AsesoriaHorario = new HashSet<AsesoriaHorario>();
            DepartamentosAsesorias = new HashSet<DepartamentosAsesorias>();
        }

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Asesor { get; set; }
        public string Aula { get; set; }
        public bool General { get; set; }

        public virtual ICollection<AsesoriaHorario> AsesoriaHorario { get; set; }
        public virtual ICollection<DepartamentosAsesorias> DepartamentosAsesorias { get; set; }
    }
}
