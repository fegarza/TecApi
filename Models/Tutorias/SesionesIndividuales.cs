using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class SesionesIndividuales
    {
        public SesionesIndividuales()
        {
            EstudiantesSesionesIndividuales = new HashSet<EstudiantesSesionesIndividuales>();
        }

        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public byte DepartamentoId { get; set; }
        public int AccionTutorialId { get; set; }
        public bool? Visible { get; set; }

        public virtual AccionesTutoriales AccionTutorial { get; set; }
        public virtual Departamentos Departamento { get; set; }
        public virtual ICollection<EstudiantesSesionesIndividuales> EstudiantesSesionesIndividuales { get; set; }
    }
}
