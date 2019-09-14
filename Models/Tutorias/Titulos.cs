using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Titulos
    {
        public Titulos()
        {
            Personales = new HashSet<Personales>();
        }

        public byte Id { get; set; }
        public string Titulo { get; set; }

        public virtual ICollection<Personales> Personales { get; set; }
    }
}
