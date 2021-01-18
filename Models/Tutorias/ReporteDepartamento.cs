using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
    public class ReporteDepartamento
    {
        [Key]
        public string Tutor { get; set; }
        public Int16 SesionesGrupales { get; set; }
        public Int16 SesionesIndividuales { get; set; }
        public int Estudiantes { get; set; }
        public int EstudiantesPrimeroYSegundoHombres { get; set; }
        public int EstudiantesPrimeroYSegundoMujeres { get; set; }
        public int EstudiantesMayoresHombres { get; set; }
        public int EstudiantesMayoresMujeres { get; set; }
        public string AreasDeCanalizacion { get; set; }
    }
}
