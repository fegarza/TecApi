using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Logs
    {
        public int LogId { get; set; }
        public int UsuarioId { get; set; }
        public string Contenido { get; set; }
        public DateTime Fecha { get; set; }
    }
}
