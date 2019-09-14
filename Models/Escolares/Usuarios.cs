using System;
using System.Collections.Generic;

namespace TecAPI.Models.Escolares
{
    public partial class Usuarios
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; }
        public int PersonalId { get; set; }
        public int DepartamentoId { get; set; }
        public string Genero { get; set; }
    }
}
