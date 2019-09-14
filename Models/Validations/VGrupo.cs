using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TecAPI.Models.Tutorias;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(Grupos.MetaData))]
    public partial class Grupos
    {
        sealed class MetaData
        {   
            [Unico("grupoPersonal", ErrorMessage = "el personal ya tiene un grupo registrado")]
            [Existe("grupoPersonal", ErrorMessage = "el personal no existe en el sistema")]
            [Range(1, int.MaxValue, ErrorMessage = "El personal id {0} deberia estar entre {1} y {2}.")]
            public int PersonalId { get; set; }
            
        }




    }
}
