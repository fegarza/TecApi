using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class SesionesIndividuales
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }
        public int EstudianteId { get; set; }
        public string Descripcion { get; set; }
        public int AccionTutorialId { get; set; }

        public virtual AccionesTutoriales AccionTutorial { get; set; }
        public virtual Estudiantes Estudiante { get; set; }
        public virtual Personales Personal { get; set; }
    }
}
