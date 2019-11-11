using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{



     
  
    [Route("api/[controller]")]
    [Authorize(Roles = "A, C, J, D, T")]
    [ApiController]
    public class AsesoriasController : ControllerBase
    {
        /// <summary>
        /// Mostrar todas las asesorias
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        public Respuesta Index(int cant, int pag, string orderBy, [FromQuery]Asesorias asesoria)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Asesorias.Include(i => i.AsesoriaHorario).Select(s => new { 
                        s.Id,
                        s.Asesor,
                        s.AsesoriaHorario,
                        s.Aula,
                        s.DepartamentosAsesorias,
                        s.General
                    });
                    if (asesoria != null)
                    {
                        if (!String.IsNullOrEmpty(asesoria.Aula))
                        {
                            result = result.Where(w => w.Aula == asesoria.Aula);
                        }
                    }

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
        public Respuesta Store([FromBody]Asesorias _asesoria)
        {
            Respuesta respuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    db.Asesorias.Add(_asesoria);
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Asesoria insertada con exito";
                }
                catch(Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al intentar insertar la asesoria";

                }
            }
            return respuesta;
        }

        [HttpPut]
        public Respuesta Update([FromBody] Asesorias _asesoria)
        {
            Respuesta respuesta = new Respuesta();
            if(_asesoria.Id != 0)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Asesorias.Where(w => w.Id == _asesoria.Id).First();
                        result.Asesor = _asesoria.Aula;
                        result.AsesoriaHorario = _asesoria.AsesoriaHorario;
                        result.DepartamentosAsesorias = _asesoria.DepartamentosAsesorias;
                        result.General = _asesoria.General;
                        result.Titulo = _asesoria.Titulo;
                        db.SaveChanges();
                        respuesta.code = StatusCodes.Status200OK;
                        respuesta.mensaje = "Asesoria actualizada con exito";
                    }
                    catch (Exception e)
                    {
                        respuesta.data = e;
                        respuesta.code = StatusCodes.Status400BadRequest;
                        respuesta.mensaje = "Error al intentar actualizar la asesoria";

                    }
                }
            }
            else
            {
                respuesta.code = StatusCodes.Status400BadRequest;
                respuesta.mensaje = "Error al actualizar la asesoria";
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
                    db.AsesoriaHorario.RemoveRange(db.AsesoriaHorario.Where(w => w.AsesoriaId == int.Parse(id)));
                    db.DepartamentosAsesorias.RemoveRange(db.DepartamentosAsesorias.Where(w => w.AsesoriaId == int.Parse(id)));
                    db.Asesorias.Remove(db.Asesorias.Where(w => w.Id == int.Parse(id)).First());
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Asesoria eliminada con exito";
                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al eliminar la asesoria";
                }
            }
            return respuesta;
        }
    }
}