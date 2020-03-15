using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class EstudiantesSesionesIndividuales
    {
        public int SesionIndividualId { get; set; }
        public int EstudianteId { get; set; }
        public int GrupoId { get; set; }

        public virtual Estudiantes Estudiante { get; set; }
        public virtual Grupos Grupo { get; set; }
        public virtual SesionesIndividuales SesionIndividual { get; set; }
    }
}
