using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecAPI.Models.Tutorias;

namespace TecAPI.Response
{
    public class REstudiante : RUsuario
    {
       
        public string numeroDeControl { get; set; }
        public string curp { get; set; }

        public RGrupo grupo { get; set; }
        

        public REstudiante(string _nombre, string _apellidoM, string _apellidoP, string _numControl, string _genero, string _correo, RGrupo _grupo)
        {
            nombre = _nombre;
            apellidoMaterno =  _apellidoM;
            apellidoPaterno = _apellidoP;
            numeroDeControl = _numControl;
            genero =  _genero;
            correo = _correo;
            grupo = _grupo;
        }
        public REstudiante(string _nombre, string _apellidoM, string _apellidoP, string _numControl, string _genero, string _correo)
        {
            nombre = _nombre;
            apellidoMaterno = _apellidoM;
            apellidoPaterno = _apellidoP;
            numeroDeControl = _numControl;
            genero = _genero;
            correo = _correo;
             
        }
        public REstudiante(Estudiantes _e) { }
        public REstudiante() { }
    }
}
