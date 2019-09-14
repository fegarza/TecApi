using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TecAPI.Models.Tutorias;

namespace TecAPI.Responses
{
    public class UsuarioResponse
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Genero { get; set; }
        public string Token { get; set; }
        public string Tipo { get; set; }

        public UsuarioResponse(Usuarios _user)
        {
            Id = _user.Id;
            Nombre = _user.Nombre;
            ApellidoPaterno = _user.ApellidoPaterno;
            ApellidoMaterno = _user.ApellidoMaterno;
            Genero = _user.Genero;
            Tipo = _user.Tipo;
        }


    }
}
