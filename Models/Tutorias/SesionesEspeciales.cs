using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class SesionesEspeciales
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int EstudianteId { get; set; }
        public int PersonalId { get; set; }
        public string Comentarios { get; set; }

        public virtual Estudiantes Estudiante { get; set; }
        public virtual Personales Personal { get; set; }
    }
}
