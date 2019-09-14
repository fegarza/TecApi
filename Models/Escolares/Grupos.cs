using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Grupos
    {
        public int GrupoId { get; set; }
        public string Titulo { get; set; }
        public int ActividadId { get; set; }
        public int Limite { get; set; }
        public int ResponsableId { get; set; }

        public virtual Actividades Actividad { get; set; }
    }
}
