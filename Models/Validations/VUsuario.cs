using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Models.Tutorias
{
   [ModelMetadataType(typeof(Usuarios.MetaData))]
    public partial class Usuarios
    {
        sealed class MetaData
        {

            [Required(ErrorMessage = "el email es requerido")]
            [EmailAddress(ErrorMessage = "el email no es valido")]
            [Unico("email", ErrorMessage = "el email ya existe")]
            public string Email { get; set; }
            [Required(ErrorMessage = "la clave es requerida")]
            [MinLength(6 , ErrorMessage = "la clave debe tener un mínimo de 6 digitos")]
            public string Clave { get; set; }
        }

       


    }


    


    public class CustomAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            return true;
        }
    }
    
}
