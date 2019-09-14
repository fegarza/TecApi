using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(SesionesIndividuales.MetaData))]
    public partial class SesionesIndividuales
    {
        sealed class MetaData
        {
            [Existe("personal", ErrorMessage = "el personal dado no existe")]
            [Required(ErrorMessage = "no se ha dado el personalId")]
            public int PersonalId { get; set; }
            [Existe("estudiante", ErrorMessage = "el estudiante dado no existe")]
            [Required(ErrorMessage = "no se ha dado el estudianteId")]
            public int EstudianteId { get; set; }

        }
    }

}
