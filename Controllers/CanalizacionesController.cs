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

        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarTodos()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {

                    var result = db.Canalizaciones.Select(s => new
                    {
                        descripcion = s.Descripcion,
                        estudiante = new
                        {
                            id = s.Estudiante.Id,
                            numeroDeControl = s.Estudiante.NumeroDeControl,
                            usuario = new
                            {
                                id = s.Estudiante.Usuario.Id,
                                nombreCompleto = s.Estudiante.Usuario.NombreCompleto
                            }
                        },
                        personal = new
                        {
                            id = s.Personal.Id,
                            usuario = new
                            {
                                id = s.Personal.Usuario.Id,
                                nombreCompleto = s.Personal.Usuario.NombreCompleto
                            }
                        },
                        Atencion = new
                        {
                            id = s.Atencion.AreaId,
                            titulo = s.Atencion.Titulo
                        }
                    }).ToList();
                    if (result.Count > 0)
                    {
                        miRespuesta.data = result;
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "exito";
                    }
                    else
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.mensaje = "No hay canalizaciones registradas";
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