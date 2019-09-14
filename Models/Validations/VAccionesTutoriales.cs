using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(AccionesTutoriales.MetaData))]
    public partial class AccionesTutoriales
    {
        sealed class MetaData
        {
            [Required(ErrorMessage ="es necesario introducir el id del personal")]
            [Range(1, int.MaxValue, ErrorMessage = "el personal id {0} deberia estar entre {1} y {2}.")]
            [Existe("personal", ErrorMessage = "el personal no existe en el sistema")]
            public int PersonalId { get; set; }
            [Required(ErrorMessage = "es necesario introducir el contenido de la accion")]
            public string Contenido { get; set; }
            [Required(ErrorMessage = "es necesario introducir el titulo de la accion")]
            public string Titulo { get; set; }
            [DataType(DataType.DateTime, ErrorMessage ="la fecha no es valida")]
            public DateTime Fecha { get; set; }
            [Required(ErrorMessage = "es necesario saber si debe ser obligatorio")]
            public bool Obligatorio { get; set; }

        }
    }

}
