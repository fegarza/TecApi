using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Departamentos
    {
        public Departamentos()
        {
            Personales = new HashSet<Personales>();
            Sesiones = new HashSet<Sesiones>();
        }

        public byte Id { get; set; }
        public string Titulo { get; set; }

        public virtual ICollection<Personales> Personales { get; set; }
        public virtual ICollection<Sesiones> Sesiones { get; set; }
    }
}
