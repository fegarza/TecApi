using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class EstudiantesSesiones
    {
        public int SesionId { get; set; }
        public int EstudianteId { get; set; }
        public int GrupoId { get; set; }

        public virtual Estudiantes Estudiante { get; set; }
        public virtual Grupos Grupo { get; set; }
        public virtual Sesiones Sesion { get; set; }
    }
}
