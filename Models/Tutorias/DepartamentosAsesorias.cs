using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class DepartamentosAsesorias
    {
        public int AsesoriaId { get; set; }
        public byte DepartamentoId { get; set; }

        public virtual Asesorias Asesoria { get; set; }
        public virtual Departamentos Departamento { get; set; }
    }
}
