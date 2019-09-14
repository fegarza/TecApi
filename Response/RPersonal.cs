using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Response
{
    public class RPersonal : RUsuario
    {
        public string id { get; set; }
        public string departamento{get;set; }
        public RPersonal jefe { get; set; }
        public RGrupo grupo { get; set; }
        public string cargo { get; set; }
        public RPersonal(string _nombre, string _apellidoMaterno, string _apellidoPaterno, string _genero, string _correo, string _cargo)
        {
            nombre = _nombre;
            apellidoMaterno = _apellidoMaterno;
            apellidoPaterno = _apellidoPaterno;
            genero = _genero;
            correo = _correo;
            cargo = _cargo;
        }
        public RPersonal(string _nombre, string _apellidoMaterno, string _apellidoPaterno, string _genero, string _correo, string _cargo, RGrupo _grupo)
        {
            grupo = _grupo;
            nombre = _nombre;
            apellidoMaterno = _apellidoMaterno;
            apellidoPaterno = _apellidoPaterno;
            genero = _genero;
            correo = _correo;
            cargo = _cargo;
        }
        public RPersonal(string _nombre, string _apellidoMaterno, string _apellidoPaterno)
        {
            nombre = _nombre;
            apellidoMaterno = _apellidoMaterno;
            apellidoPaterno = _apellidoPaterno;
        }
    }
}
