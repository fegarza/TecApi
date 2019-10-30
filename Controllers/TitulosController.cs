using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{

    /// <summary>
    /// Todo lo relacionado a los titulos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class TitulosController : ControllerBase
    {
        /// <summary>
        /// Mostrar todos los titulos disponibles
        /// </summary>
        /// <returns>Un modelo de respusta</returns>
        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarTodos()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    
                    var result = db.Titulos
                         .Select(s => new
                         {
                             id = s.Id,
                             titulo = s.Titulo
                         })
                         .ToList();
                    if (result.Count > 0)
                    {
                        miRespuesta.data = result;
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "exito";
                    }
                    else
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.mensaje = "No hay titulos registrados";
                    }
                }
            }
            catch (Exception ex)
            {
                miRespuesta.code = StatusCodes.Status500InternalServerError;
                miRespuesta.mensaje = "error interno";
            }

            return miRespuesta;
        }

    }
}