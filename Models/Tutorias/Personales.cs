using System;
using System.Collections.Generic;

namespace TecAPI.Models.Tutorias
{
    public partial class Personales
    {
        public Personales()
        {
            AccionesTutoriales = new HashSet<AccionesTutoriales>();
            Canalizaciones = new HashSet<Canalizaciones>();
            Posts = new HashSet<Posts>();
            SesionesIndividuales = new HashSet<SesionesIndividuales>();
        }

        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public byte DepartamentoId { get; set; }
        public byte TituloId { get; set; }
        public string Cargo { get; set; }
        public string Cve { get; set; }

        public virtual Departamentos Departamento { get; set; }
        public virtual Titulos Titulo { get; set; }
        public virtual Usuarios Usuario { get; set; }
        public virtual Grupos Grupos { get; set; }
        public virtual ICollection<AccionesTutoriales> AccionesTutoriales { get; set; }
        public virtual ICollection<Canalizaciones> Canalizaciones { get; set; }
        public virtual ICollection<Posts> Posts { get; set; }
        public virtual ICollection<SesionesIndividuales> SesionesIndividuales { get; set; }
    }
}
