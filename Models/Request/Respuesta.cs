using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Request
{
    public class Respuesta
    {
        public int code { get; set; }
        public object data { get; set; }
        public string mensaje { get; set; }


        public Respuesta() { }

        public Respuesta(int _code, string _mensaje)
        {
            code = _code;
            mensaje = _mensaje;
        }

    }

}
