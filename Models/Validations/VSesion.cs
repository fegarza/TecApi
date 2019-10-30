﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TecAPI.Models.Tutorias;

namespace TecAPI.Models.Tutorias
{
    [ModelMetadataType(typeof(Sesiones.MetaData))]
    public partial class Sesiones
    {
        sealed class MetaData
        {
            [NotNull(ErrorMessage = "la fecha no ha sido introducida")]
            [DataType(DataType.Date, ErrorMessage = "la fecha no es valida")]
            [Required(ErrorMessage = "la fecha no ha sido introducida")]
            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public DateTime? Fecha { get; set; }
            [Existe("departamento", ErrorMessage = "el departamento no existe en el sistema")]
            [Required(ErrorMessage = "el DepartamentoId no ha sido introducido")]
            public byte DepartamentoId { get; set; }
            [Required(ErrorMessage = "la AccionTutorialId no ha sido introducida")]
            [Existe("accionTutorial", ErrorMessage = "la accion tutorial no existe en el sistema")]
            public int AccionTutorialId { get; set; }
        }
    }
  


}
