using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Carreras
    {
        public Carreras()
        {
            Estudiantes = new HashSet<Estudiantes>();
        }

        public byte Id { get; set; }
        public short Carcve { get; set; }
        public string Titulo { get; set; }

        public virtual ICollection<Estudiantes> Estudiantes { get; set; }
    }
}
