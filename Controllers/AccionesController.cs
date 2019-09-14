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
    [ApiController]
    public class AccionesController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarTodos(string cant, string pag)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.AccionesTutoriales
                      .Select(s =>
                           new
                           {
                                id = s.Id,
                                personalId = s.PersonalId,
                                titulo = s.Titulo,
                                contenido = s.Contenido,
                                fecha = s.Fecha.ToShortDateString(),
                                obligatorio = s.Obligatorio
                           }
                    );
                    if (cant != null & pag != null)
                    {
                        try
                        {
                            int cantidad = int.Parse(cant);
                            int pagina = int.Parse(pag);
                            var resut2 = result.Skip((cantidad * pagina) - cantidad).Take(cantidad).ToList();
                            if (resut2.Count() > 0)
                            {
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                                miRespuesta.data = resut2;
                            }
                            else
                            {
                                miRespuesta.code = StatusCodes.Status404NotFound;
                                miRespuesta.mensaje = "no hay registros";
                                miRespuesta.data = resut2;
                            }

                        }
                        catch
                        {
                            miRespuesta.code = StatusCodes.Status400BadRequest;
                            miRespuesta.mensaje = "error con el numero de pag y numero de cantidad";
                        }
                    }
                    else
                    {
                        if (result.Count() > 0)
                        {
                            miRespuesta.code = 200;
                            miRespuesta.data = result.ToList();
                            miRespuesta.mensaje = "exito";
                        }
                        else
                        {
                            miRespuesta.code = StatusCodes.Status404NotFound;
                            miRespuesta.data = result.ToList();
                            miRespuesta.mensaje = "no hay registros";
                        }

                    }
                    //






                }
                catch (Exception ex)
                {
                    miRespuesta.code = StatusCodes.Status500InternalServerError;
                    miRespuesta.mensaje = "error interno";
                }

            }



            return miRespuesta;
        }


        [AllowAnonymous]
        [HttpPost]
        public Respuesta Registrar([FromBody] AccionesTutoriales accion)
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
                miRespuesta.code = 400;
            }
            else
            {
                try
                {
                    using (TUTORIASContext db = new TUTORIASContext())
                    {
                        try
                        {
                            db.AccionesTutoriales.Add(accion);
                            db.SaveChanges();
                            miRespuesta.mensaje = "Se ha insertado correctamente";
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = accion;
                        }
                        catch (Exception ex)
                        {
                            miRespuesta.mensaje = $"{ex.ToString()}";
                            miRespuesta.code = 500;
                        }
                    }
                }
                catch (Exception ex)
                {
                    miRespuesta.mensaje = $"{ex.ToString()}";
                    miRespuesta.code = 500;
                }
            }

            return miRespuesta;


        }
    }
}