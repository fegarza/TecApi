using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Response
{
    public class RUsuario
    {
        public string nombre { get; set; }
        public string nombreCompleto { get; set; }
        public string apellidoMaterno { get; set; }
        public string apellidoPaterno { get; set; }
        public string genero { get; set; }
        public string correo { get; set; }
        public string clave { get; set; }
        public string token { get; set; }
        public string tipo { get; set; }

    }
}
