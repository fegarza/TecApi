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
    public class ArchivosController : ControllerBase
    {
        /// <summary>
        /// Mostrar todas las asesorias
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        [AllowAnonymous]
        public Respuesta Index(int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Archivos.Select(s => new {
                      s.Id,
                      s.Link,
                      s.Titulo,
                      s.Fecha,
                         s.Descripcion
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
                catch (Exception ex)
                {
                    miRespuesta.code = StatusCodes.Status500InternalServerError;
                    miRespuesta.mensaje = "error interno";
                    miRespuesta.data = ex;
                }

            }
            return miRespuesta;
        }

        [HttpPost]
        public Respuesta Store([FromBody]Archivos _archivo)
        {
            Respuesta respuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    db.Archivos.Add(_archivo);
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Archivo insertado con exito";
                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al intentar insertar el archivo";

                }
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
                    db.Archivos.Remove(db.Archivos.Where(w => w.Id == int.Parse(id)).First());
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Archivo eliminado con exito";
                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al eliminar el archivo";
                }
            }
            return respuesta;
        }

        [HttpPut]
        public Respuesta Update([FromBody] Archivos archivo)
        {
            Respuesta respuesta = new Respuesta();
            if (archivo.Id != 0)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var  result = db.Archivos.Where(w => w.Id == archivo.Id);
                        result.First().Link = archivo.Link;
                        result.First().Fecha = archivo.Fecha;
                        result.First().Titulo = archivo.Titulo;
                        result.First().Descripcion = archivo.Descripcion;
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

        [Route("count")]
        [HttpGet]
        public Respuesta Count()
        {
            Respuesta respuesta = new Respuesta();
            respuesta.code = StatusCodes.Status200OK;
            respuesta.mensaje = "Exito";
            using (TUTORIASContext db = new TUTORIASContext())
            {

                respuesta.data = new { count = db.Archivos.Count() };
            }

            return respuesta;
        }
    }
}