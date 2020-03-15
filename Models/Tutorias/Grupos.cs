using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Grupos
    {
        public Grupos()
        {
            Estudiantes = new HashSet<Estudiantes>();
            EstudiantesSesiones = new HashSet<EstudiantesSesiones>();
            EstudiantesSesionesIndividuales = new HashSet<EstudiantesSesionesIndividuales>();
        }

        public int Id { get; set; }
        public int PersonalId { get; set; }
        public string Salon { get; set; }

        public virtual Personales Personal { get; set; }
        public virtual ICollection<Estudiantes> Estudiantes { get; set; }
        public virtual ICollection<EstudiantesSesiones> EstudiantesSesiones { get; set; }
        public virtual ICollection<EstudiantesSesionesIndividuales> EstudiantesSesionesIndividuales { get; set; }
    }
}
