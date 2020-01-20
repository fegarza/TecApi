using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{


    /// <`ummary>
    /// Todo lo relacionado con las acciones tutoriales
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class AccionesController : ControllerBase
    {
        /// <summary>
        /// Mostrar todas las acciones tutoriales disponibles
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        public Respuesta Index(int cant, int pag)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.AccionesTutoriales.OrderByDescending(o => o.Fecha)
                      .Select(s =>
                           new
                           {
                               id = s.Id,
                               personalId = s.PersonalId,
                               titulo = s.Titulo,
                               contenido = s.Contenido,
                               fecha = s.Fecha.ToString("MM/dd/yyyy"),
                               obligatorio = s.Obligatorio,
                               tipo = s.Tipo,
                               activo = s.Activo
                           }
                    );
                    if (cant != 0 & pag != 0)
                    {
                        int x = ((cant * pag) - cant);
                         result = result.Skip((cant * pag) - cant).Take(cant);
                    }
                    
                    if(result.Count() > 0)
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
                catch (Exception ex)
                {
                    miRespuesta.code = StatusCodes.Status500InternalServerError;
                    miRespuesta.mensaje = "error interno";
                    miRespuesta.data = ex;
                }

            }
            return miRespuesta;
        }

        /// <summary>
        /// Registra una nueva accion tutorial
        /// </summary>
        /// <param name="accion">el objeto en formato JSON</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpPost]
        [Authorize(Roles = "A, C")]
        public Respuesta Store([FromBody] AccionesTutoriales accion)
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
                        try
                        {
                            accion.Activo = true;
                            db.AccionesTutoriales.Add(accion);
                            db.SaveChanges();
                            miRespuesta.mensaje = "Se ha insertado correctamente";
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = accion;
                        }
                        catch (Exception ex)
                        {
                            miRespuesta.mensaje = $"{ex.ToString()}";
                            miRespuesta.code = StatusCodes.Status500InternalServerError;
                        }
                    }
                }
                catch (Exception ex)
                {
                    miRespuesta.mensaje = $"{ex.ToString()}";
                    miRespuesta.code = StatusCodes.Status500InternalServerError; ;
                }
            }

            return miRespuesta;


        }

        /// <summary>
        /// Actualizar una accion tutorial
        /// </summary>
        /// <param name="miAccion">Objeto a actualizar</param>
        /// <returns>un estado de la operacion</returns>
        [AllowAnonymous]
        [HttpPut]
        [Authorize(Roles = "A, C")]
        public Respuesta Update([FromBody] AccionesTutoriales miAccion)
        {
            Respuesta miRespuesta = new Respuesta();

            if (miAccion != null)
            {
                if (!String.IsNullOrEmpty(miAccion.Id.ToString()))
                {
                    try
                    {
                        using (TUTORIASContext db = new TUTORIASContext())
                        {
                            var result = db.AccionesTutoriales.Where(r => r.Id == miAccion.Id);
                            if (result.Count() > 0)
                            {
                                try
                                {
                                    List<string> acciones = new List<string>();
                                    List<string> errores = new List<string>();

                                    if (!String.IsNullOrEmpty(miAccion.Titulo))
                                    {
                                        result.First().Titulo = miAccion.Titulo;
                                        acciones.Add("se ha cambiado el titulo con exito");
                                    }
                                    if (!String.IsNullOrEmpty(miAccion.Contenido))
                                    {
                                        result.First().Contenido = miAccion.Contenido;
                                        acciones.Add("se ha cambiado el contenido con exito");
                                    }
                                    if (miAccion.Fecha != null)
                                    {
                                        result.First().Fecha = miAccion.Fecha;
                                        acciones.Add("se ha cambiado la fecha con exito");
                                    }
                                    if (miAccion.Obligatorio != result.First().Obligatorio)
                                    {
                                        result.First().Obligatorio = miAccion.Obligatorio;
                                        acciones.Add("se ha cambiado la configuracion con exito");
                                    }
                                    if(miAccion.Activo != null)
                                    {
                                        result.First().Activo = miAccion.Activo;
                                        acciones.Add("se ha cambiado el estado con exito");
                                    }
                                    if (!string.IsNullOrEmpty(miAccion.Tipo) )
                                    {
                                        result.First().Tipo = miAccion.Tipo;
                                        acciones.Add("se ha cambiado el tipo con exito");
                                    }

                                    db.SaveChanges();
                                    miRespuesta.mensaje = "exito";
                                    miRespuesta.data = new { acciones, errores };
                                    miRespuesta.code = StatusCodes.Status200OK;
                                }
                                catch
                                {
                                    miRespuesta.mensaje = "error al establecer datos a la accion tutorial";
                                    miRespuesta.code = StatusCodes.Status400BadRequest;
                                }
                            }
                            else
                            {
                                miRespuesta.mensaje = "no existe una accion tutorial con ese id";
                                miRespuesta.code = 500;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.mensaje = "error en el sistema";
                        miRespuesta.code = 500;
                    }
                }
                else
                {
                    miRespuesta.code = StatusCodes.Status400BadRequest;
                    miRespuesta.mensaje = "no se ha dado el id de la acción tutorial";
                }
            }
            else
            {
                miRespuesta.code = StatusCodes.Status400BadRequest;
                miRespuesta.mensaje = "los datos no son enviados no son correctos";
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
                    db.AccionesTutoriales.Remove(db.AccionesTutoriales.Where(w => w.Id == int.Parse(id)).First());
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Accion tutorial eliminada con exito";
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

        [Route("count")]
        [HttpGet]
        public Respuesta Count()
        {
            Respuesta respuesta = new Respuesta();
            respuesta.code = StatusCodes.Status200OK;
            respuesta.mensaje = "Exito";
            using (TUTORIASContext db = new TUTORIASContext())
            {

                respuesta.data = new { count = db.AccionesTutoriales.Count() };
            }

            return respuesta;
        }

    }
}