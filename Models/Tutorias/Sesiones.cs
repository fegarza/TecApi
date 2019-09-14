using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Sesiones
    {
        public Sesiones()
        {
            EstudiantesSesiones = new HashSet<EstudiantesSesiones>();
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public byte DepartamentoId { get; set; }
        public int AccionTutorialId { get; set; }

        public virtual AccionesTutoriales AccionTutorial { get; set; }
        public virtual Departamentos Departamento { get; set; }
        public virtual ICollection<EstudiantesSesiones> EstudiantesSesiones { get; set; }
    }
}
