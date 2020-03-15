using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Departamentos
    {
        public Departamentos()
        {
            DepartamentosAsesorias = new HashSet<DepartamentosAsesorias>();
            Personales = new HashSet<Personales>();
            Sesiones = new HashSet<Sesiones>();
            SesionesIndividuales = new HashSet<SesionesIndividuales>();
        }

        public byte Id { get; set; }
        public string Titulo { get; set; }
        public TimeSpan? HoraInicio { get; set; }
        public TimeSpan? HoraFin { get; set; }
        public bool? Dia { get; set; }

        public virtual ICollection<DepartamentosAsesorias> DepartamentosAsesorias { get; set; }
        public virtual ICollection<Personales> Personales { get; set; }
        public virtual ICollection<Sesiones> Sesiones { get; set; }
        public virtual ICollection<SesionesIndividuales> SesionesIndividuales { get; set; }
    }
}
