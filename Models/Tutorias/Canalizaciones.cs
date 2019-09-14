using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Canalizaciones
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }
        public int EstudianteId { get; set; }
        public byte AtencionId { get; set; }
        public string Descripcion { get; set; }
        public DateTime Fecha { get; set; }
        public string Estado { get; set; }

        public virtual Atenciones Atencion { get; set; }
        public virtual Estudiantes Estudiante { get; set; }
        public virtual Personales Personal { get; set; }
    }
}
