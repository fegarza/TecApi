using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(Canalizaciones.MetaData))]
    public partial class Canalizaciones
    {
        sealed class MetaData
        {
            [Required(ErrorMessage ="es necesario introducir el id del personal")]
            [Range(1, int.MaxValue, ErrorMessage = "el personal id {0} deberia estar entre {1} y {2}.")]
            [Existe("personal", ErrorMessage = "el personal no existe en el sistema")]
            public int PersonalId { get; set; }

            [Required(ErrorMessage = "es necesario introducir el id del estudiante")]
            [Range(1, int.MaxValue, ErrorMessage = "el estudiante id {0} deberia estar entre {1} y {2}.")]
            [Existe("estudiante", ErrorMessage = "el estudiante no existe en el sistema")]
            public int EstudianteId { get; set; }

            [Required(ErrorMessage = "es necesario introducir el id de la atencion")]
            [Existe("atencion", ErrorMessage = "el personal no existe en el sistema")]
            [Range(1, byte.MaxValue, ErrorMessage = "la atencion id {0} deberia estar entre {1} y {2}.")]
            public byte AtencionId { get; set; }

            [Required(ErrorMessage = "es necesario introducir la descripcion de la canalizacion")]
            public string Descripcion { get; set; }

        }
    }

}
