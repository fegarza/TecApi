using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(Personales.MetaData))]
    public partial class Personales
    {
        sealed class MetaData
        {
            [Required(ErrorMessage = "es necesario introducir el departamento")]
            [Existe("departamento", ErrorMessage = "el departamento no existe en el sistema")]
            [Range(1, byte.MaxValue, ErrorMessage = "El departamento id {0} deberia estar entre {1} y {2}.")]
            public byte DepartamentoId { get; set; }

            [Required(ErrorMessage = "es necesario poner los datos del usuario")]
            public Usuarios Usuario { get; set; }

            [Required(ErrorMessage = "es necesario introducir la Clave del personal")]
            [Unico("clave", ErrorMessage = "el personal ya existe en el sistema")]
            [Existe("clave", ErrorMessage = "el personal no existe")]
            public string Clave { get; set; }

            [Required(ErrorMessage = "es necesario introducir el titulo del personal")]
            [Existe("titulo", ErrorMessage = "el titulo no existe")]
            public byte TituloId { get; set; }

        }
    }

}
