using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Estudiantes
    {
        public Estudiantes()
        {
            Canalizaciones = new HashSet<Canalizaciones>();
            EstudiantesSesiones = new HashSet<EstudiantesSesiones>();
            EstudiantesSesionesIndividuales = new HashSet<EstudiantesSesionesIndividuales>();
        }

        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int? GrupoId { get; set; }
        public byte CarreraId { get; set; }
        public string NumeroDeControl { get; set; }
        public byte SesionesIniciales { get; set; }
        public int? Semestre { get; set; }
        public string Estado { get; set; }
        public string FotoLink { get; set; }

        public virtual Carreras Carrera { get; set; }
        public virtual Grupos Grupo { get; set; }
        public virtual Usuarios Usuario { get; set; }
        public virtual EstudiantesDatos EstudiantesDatos { get; set; }
        public virtual ICollection<Canalizaciones> Canalizaciones { get; set; }
        public virtual ICollection<EstudiantesSesiones> EstudiantesSesiones { get; set; }
        public virtual ICollection<EstudiantesSesionesIndividuales> EstudiantesSesionesIndividuales { get; set; }
    }
}
