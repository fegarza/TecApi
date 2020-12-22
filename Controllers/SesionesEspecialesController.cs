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
    [Authorize(Roles = "A, C, J, D, T")]
    public class SesionesEspecialesController : ControllerBase
    {


     
        /// <summary>
        /// Insertar una sesionEspecial
        /// </summary>
        /// <param name="sesionEspecial">El objeto sesion en formato JSON</param>
        /// <returns>Un modelo de respuesta</returns>
        [HttpPost]
        public Respuesta Store(SesionesEspeciales sesionEspecial)
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

                miRespuesta.mensaje = "datos invalidos";
                miRespuesta.code = StatusCodes.Status400BadRequest;
            }
            else
            {
                try
                {
                    using (TUTORIASContext db = new TUTORIASContext())
                    {
                        db.SesionesEspeciales.Add(sesionEspecial);
                        db.SaveChanges();
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "exito";
                        miRespuesta.data = sesionEspecial;
                    }
                }
                catch (Exception ex)
                {
                    miRespuesta.mensaje = "error";
                    miRespuesta.code = StatusCodes.Status500InternalServerError;
                    miRespuesta.data = ex;
                    Console.WriteLine("ERROOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOR");
                    Console.WriteLine("ERROOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOR");
                    Console.WriteLine(ex);
                }
            }

            return miRespuesta;
        }


        [HttpDelete]
        [Route("{id}")]
        public Respuesta Delete(string id)
        {
            Respuesta respuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    db.SesionesEspeciales.Remove(db.SesionesEspeciales.Where(w => w.Id == int.Parse(id)).First());
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Sesion inidividual eliminada con exito";
                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al eliminar la sesion";
                }
            }
            return respuesta;
        }
    }
}