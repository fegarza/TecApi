using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Response
{
    public class RGrupo
    {
        public string titulo { get; set; }
        public List<REstudiante> estudiantes = new List<REstudiante>();
        public RPersonal tutor { get; set; }
        public RGrupo(string _titulo, RPersonal _tutor)
        {
            titulo = _titulo;
            tutor = _tutor;
        }
        public RGrupo(string _titulo, RPersonal _tutor, List<REstudiante> _estudiantes)
        {
            titulo = _titulo;
            tutor = _tutor;
            estudiantes= _estudiantes;
        }


    }
}
