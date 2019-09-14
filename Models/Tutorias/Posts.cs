using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Posts
    {
        public int Id { get; set; }
        public int PersonalId { get; set; }
        public string Cargo { get; set; }
        public string Titulo { get; set; }
        public string Contenido { get; set; }

        public virtual Personales Personal { get; set; }
    }
}
