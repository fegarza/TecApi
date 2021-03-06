﻿using System;
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
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]

    public class SesionesIndividualesController : ControllerBase
    {
        /// <summary>
        /// Mostrar todas las sesiones
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        public Respuesta Index(int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.SesionesIndividuales
                      .Select(s =>
                           new
                           {
                               id = s.Id,
                               departamentoId = s.DepartamentoId,
                               fecha = s.Fecha.ToString()
                           }
                    );

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


        /// <summary>
        /// Insertar una sesion
        /// </summary>
        /// <param name="sesion">El objeto sesion en formato JSON</param>
        /// <returns>Un modelo de respuesta</returns>
        [HttpPost]
        public Respuesta Store(SesionesIndividuales sesion)
        {


            sesion.Visible = true;
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

                        var sesionesDelDepartamento = db.SesionesIndividuales.Include(i => i.AccionTutorial).Where(s => s.DepartamentoId == sesion.DepartamentoId).Select(s => s.AccionTutorialId).ToArray();
                        var accionesTutorialesSinTomar = db.AccionesTutoriales.Where(w => !sesionesDelDepartamento.Contains(w.Id) && w.Tipo == "I");
                        if (accionesTutorialesSinTomar.Count() > 0)
                        {
                            var accionPendiente = accionesTutorialesSinTomar.Where(w => w.Obligatorio == true).OrderBy(o => o.Fecha);

                            if (accionPendiente.Count() > 0)
                            {
                                var AccionSeleccionada = db.AccionesTutoriales.Where(w => w.Id == sesion.AccionTutorialId).First();
                                if (AccionSeleccionada.Fecha <= accionPendiente.First().Fecha)
                                {

                                    db.SesionesIndividuales.Add(sesion);
                                    db.SaveChanges();
                                    miRespuesta.code = StatusCodes.Status200OK;
                                    miRespuesta.mensaje = "exito";
                                    miRespuesta.data = sesion;
                                }
                                else
                                {
                                    miRespuesta.code = StatusCodes.Status400BadRequest;
                                    miRespuesta.mensaje = "no se puede insertar puesto que tienes pendiente una accion tutorial obligatoria";
                                    miRespuesta.data = accionPendiente;
                                }
                            }
                            else
                            {
                                db.SesionesIndividuales.Add(sesion);
                                db.SaveChanges();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                                miRespuesta.data = sesion;

                            }




                        }
                        else//no tiene acciones tutoriales sin tomar
                        {
                            miRespuesta.code = StatusCodes.Status400BadRequest;
                            miRespuesta.mensaje = "ya cumplio con todas las acciones tutoriales pendientes";
                            miRespuesta.data = null;
                        }





                    }
                }
                catch (Exception ex)
                {
                    miRespuesta.mensaje = "error";
                    miRespuesta.code = StatusCodes.Status500InternalServerError;
                    miRespuesta.data = ex;
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
                    db.SesionesIndividuales.Remove(db.SesionesIndividuales.Where(w => w.Id == int.Parse(id)).First());
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Sesion eliminada con exito";
                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "La sesion ya tiene pases de listas asignados";
                }
            }
            return respuesta;
        }


        [HttpPut]
        public Respuesta Update([FromBody] SesionesIndividuales sesion)
        {
            Respuesta respuesta = new Respuesta();
            if (sesion.Id != 0)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.SesionesIndividuales.Where(w => w.Id == sesion.Id);
                        result.First().Visible = sesion.Visible;
                        db.SaveChanges();
                        respuesta.code = StatusCodes.Status200OK;
                        respuesta.mensaje = "Sesion editada con exito";
                    }
                    catch (Exception e)
                    {
                        respuesta.data = e;
                        respuesta.code = StatusCodes.Status400BadRequest;
                        respuesta.mensaje = "Error al editar la sesion";
                    }
                }
            }
            else
            {
                respuesta.code = StatusCodes.Status400BadRequest;
                respuesta.mensaje = "No existe tal sesion";
            }


            return respuesta;
        }


    }
}