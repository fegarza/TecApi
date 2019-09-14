using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Reportes
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public DateTime FechaDeOficio { get; set; }
        public int CarreraId { get; set; }
    }
}
