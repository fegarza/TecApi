using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(Estudiantes.MetaData))]
    public partial class Estudiantes  
    {
        [NotMapped]
        public string Curp { get; set; }

        sealed class MetaData
        {
            [MinLength(8, ErrorMessage ="el número de control debe tener 8 dígitos")]
            [MaxLength(8, ErrorMessage ="el número de control debe tener 8 dígitos")]
            [Existe("numeroDeControl", ErrorMessage ="el número de control no existe")]
            [Unico("numeroDeControl", ErrorMessage = "el número de control ya ha sido registrado")]
            [Required(ErrorMessage = "el número de control es requerido")]
            public string NumeroDeControl { get; set; }
            [Required(ErrorMessage = "datos de el usuario es requerido")]
            public Usuarios Usuario { get; set; }

            [Required(ErrorMessage = "el curp es requerido")]
            public string Curp { get; set; }
           
        }
    }

}
