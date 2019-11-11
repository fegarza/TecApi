using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Archivos
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Tipo { get; set; }
        public string Descripcion { get; set; }
        public string Link { get; set; }
        public DateTime Fecha { get; set; }
    }
}
