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
    /// Todo lo relacionado con las canalizaciones
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class CanalizacionesController : ControllerBase
    {

        /// <summary>
        /// Muestra todos las acciones
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        public Respuesta Index(int cant, int pag, string orderBy)
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
                    });
                    if (!String.IsNullOrEmpty(orderBy))
                    {
                        result = result.OrderBy(orderBy);
                    }
                    if (cant != 0 & pag != 0)
                    {
                        int x = ((cant * pag) - cant);
                        result = result.Skip((cant * pag) - cant).Take(cant);
                    }

                    if (result.Count() > 0)
                    {
                        miRespuesta.mensaje = "exito";
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.data = result.ToList();
                    }
                    else
                    {
                        miRespuesta.mensaje = "no hay registros";
                        miRespuesta.code = StatusCodes.Status404NotFound;
                        miRespuesta.data = null;
                    }

                }
            }
            catch (Exception ex)
            {
                miRespuesta.code = StatusCodes.Status500InternalServerError;
                miRespuesta.mensaje = "error interno";
                miRespuesta.data = ex;
            }

            return miRespuesta;
        }


        /// <summary>
        /// Insertar una nueva canalizacion
        /// </summary>
        /// <param name="canalizacion">El objeto en formato JSON</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpPost]
        public Respuesta Store([FromBody] Canalizaciones canalizacion)
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


        [HttpPut]
        public Respuesta Update([FromBody] Canalizaciones canalizacion)
        {
            Respuesta respuesta = new Respuesta();
            if (canalizacion.Id != 0)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Canalizaciones.Where(w => w.Id == canalizacion.Id);
                        result.First().Descripcion = canalizacion.Descripcion;
                        result.First().Fecha = canalizacion.Fecha;
                        result.First().Estado = canalizacion.Estado;
                        db.SaveChanges();
                        respuesta.code = StatusCodes.Status200OK;
                        respuesta.mensaje = "Archivo editado con exito";
                    }
                    catch (Exception e)
                    {
                        respuesta.data = e;
                        respuesta.code = StatusCodes.Status400BadRequest;
                        respuesta.mensaje = "Error al editar el archivo";
                    }
                }
            }
            else
            {
                respuesta.code = StatusCodes.Status400BadRequest;
                respuesta.mensaje = "No existe tal archivo";
            }


            return respuesta;
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
                    db.Canalizaciones.Remove(db.Canalizaciones.Where(w => w.Id == int.Parse(id)).First());
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Canalizacion eliminada con exito";
                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al eliminar la canalizacion";
                }
            }
            return respuesta;
        }


    }
}