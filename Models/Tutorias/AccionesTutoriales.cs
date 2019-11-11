using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class AccionesTutoriales
    {
        public AccionesTutoriales()
        {
            Sesiones = new HashSet<Sesiones>();
            SesionesIndividuales = new HashSet<SesionesIndividuales>();
        }

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }
        public int PersonalId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Obligatorio { get; set; }
        public bool? Activo { get; set; }
        public string Tipo { get; set; }

        public virtual Personales Personal { get; set; }
        public virtual ICollection<Sesiones> Sesiones { get; set; }
        public virtual ICollection<SesionesIndividuales> SesionesIndividuales { get; set; }
    }
}
