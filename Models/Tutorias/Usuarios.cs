using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Usuarios
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string NombreCompleto { get; set; }
        public string Genero { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }
        public string Tipo { get; set; }
        public string Publica { get; set; }

        public virtual Estudiantes Estudiantes { get; set; }
        public virtual Personales Personales { get; set; }
    }
}
