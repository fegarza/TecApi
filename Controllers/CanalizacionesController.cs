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
    [Route("api/[controller]")]
    [ApiController]
    public class CanalizacionesController : ControllerBase
    {

        [AllowAnonymous]
        [HttpPost]
        public Respuesta InsertarNuevo([FromBody] Canalizaciones canalizacion)
        {
            Respuesta miRespuesta = new Respuesta();

            if (!ModelState.IsValid)
            {
                List<string> errores = new List<string>();
                foreach (Microsoft.AspNetCore.Mvc.ModelBinding.ModelError e in ModelState.Values.SelectMany(modelState => modelState.Errors).ToList())
                {
                    errores.Add(e.ErrorMessage);
                }
                miRespuesta.data = errores;
                miRespuesta.code = StatusCodes.Status409Conflict;
                miRespuesta.mensaje = "error con el formulario";
            }
            else
            {
                try
                {
                    using (TUTORIASContext db = new TUTORIASContext())
                    {
                        canalizacion.Fecha = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd hh:mm tt"));
                        canalizacion.Estado = "a";
                        db.Canalizaciones.Add(canalizacion);
                        db.SaveChanges();
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "Insertado correctamente";
                    }
                }
                catch
                {
                    miRespuesta.code = StatusCodes.Status500InternalServerError;
                    miRespuesta.mensaje = "error en el sistema";
                }
            }






            return miRespuesta;
        }

    }
}