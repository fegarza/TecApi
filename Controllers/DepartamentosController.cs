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
    public class DepartamentosController : ControllerBase
    {
        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarTodos()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    var result = db.Departamentos
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
                        miRespuesta.mensaje = "No hay departamentos registrados";
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


        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/AccionesTutoriales")]
        public Respuesta MostrarAccionesTutoriales(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var AccionesTomadas = db.Sesiones
                        .Where(s => s.DepartamentoId == int.Parse(id))
                        .Select(s => s.Id);

                    var sesionesObligatoriasTomadas =
                        db.Sesiones
                        .Include(i => i.AccionTutorial)
                        .Where(s => s.DepartamentoId == int.Parse(id) && s.AccionTutorial.Obligatorio == true);

                    var AccionesSinTomar = db.AccionesTutoriales
                        .Where(w => !AccionesTomadas.Contains(w.Id))
                        .Select(s=> 
                        new {
                            s.Id,
                            titulo = s.Titulo,
                            contenido = s.Contenido,
                            fecha = s.Fecha.ToShortDateString(),
                            personalId = s.PersonalId,
                            obligatorio = s.Obligatorio,
                        });


                    if (AccionesTomadas.Count() > 0 && AccionesSinTomar.Count() > 0)
                    {

                        if (sesionesObligatoriasTomadas.Count() > 0)
                        {
                            //Tomamos la mas reciente
                            var ultimaSesionObligatoriaFecha = sesionesObligatoriasTomadas.Max(m => m.AccionTutorial.Fecha);
                            List<object> resultado = new List<object>();
                            bool encontrado = false;
                            foreach (var x in AccionesSinTomar.Where(w => DateTime.Parse(w.fecha) > ultimaSesionObligatoriaFecha))
                            {
                                if (!encontrado)
                                {
                                    if (x.obligatorio == true)
                                    {
                                        resultado.Add(x);
                                        encontrado = true;
                                    }
                                    else
                                    {
                                        resultado.Add(x);
                                    }
                                }
                                else
                                {
                                    miRespuesta.code = StatusCodes.Status200OK;
                                    miRespuesta.data = resultado;
                                    miRespuesta.mensaje = "exito";
                                }
                            }
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = resultado;
                            miRespuesta.mensaje = "exito";
                        }
                        else
                        {
                            List<object> resultado = new List<object>();
                            bool encontrado = false;
                            foreach (var x in AccionesSinTomar.OrderBy(o => o.fecha))
                            {
                                if (!encontrado)
                                {
                                    if (x.obligatorio == true)
                                    {
                                        resultado.Add(x);
                                        encontrado = true;
                                    }
                                    else
                                    {
                                        resultado.Add(x);
                                    }
                                }
                                else
                                {
                                    miRespuesta.code = StatusCodes.Status200OK;
                                    miRespuesta.data = resultado;
                                    miRespuesta.mensaje = "exito";
                                }
                            }
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = resultado;
                            miRespuesta.mensaje = "exito";
                        }


                    }
                    else
                    {
                        List<object> resultado = new List<object>();
                        bool encontrado = false;
                        foreach (var x in AccionesSinTomar.OrderBy(o => o.fecha))
                        {
                            if (!encontrado)
                            {
                                if (x.obligatorio == true)
                                {
                                    resultado.Add(x);
                                    encontrado = true;
                                }
                                else
                                {
                                    resultado.Add(x);
                                }
                            }
                            else
                            {
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.data = resultado;
                                miRespuesta.mensaje = "exito";
                            }
                        }
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.data = resultado;
                        miRespuesta.mensaje = "exito";
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

        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/Grupos")]
        public Respuesta MostrarGrupos(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Grupos.Where(w => w.Personal.DepartamentoId == byte.Parse(id))
                        .Select(s => new
                        {
                            id = s.Id,
                            salon = s.Salon,
                            tutor = new
                            {
                                id = s.Personal.Id,
                                departamento = s.Personal.Departamento.Titulo,
                                nombre = s.Personal.Usuario.Nombre,
                                apellidoMaterno = s.Personal.Usuario.ApellidoMaterno,
                                apellidoPaterno = s.Personal.Usuario.ApellidoPaterno,
                                nombreCompleto = s.Personal.Usuario.NombreCompleto
                            }
                        }).ToList();
                    miRespuesta.code = StatusCodes.Status200OK;
                    miRespuesta.mensaje = "exito";
                    miRespuesta.data = result;
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


        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/Canalizaciones")]
        public Respuesta MostrarCanalizaciones(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                   var result =  db.Canalizaciones.Where(w => w.Personal.DepartamentoId == byte.Parse(id) && w.Estado.ToLower() == "a")
                        .Select(s => new
                        {
                             descripcion = s.Descripcion,
                             estudiante = new
                             {
                                 id = s.Estudiante.Id,
                                 numeroDeControl = s.Estudiante.NumeroDeControl,
                                 usuario = new
                                 {
                                     id= s.Estudiante.Usuario.Id,
                                     nombreCompleto = s.Estudiante.Usuario.NombreCompleto
                                 }
                             },
                             personal = new {
                                 id = s.Personal.Id,
                                 usuario = new {
                                     id = s.Personal.Usuario.Id,
                                     nombreCompleto = s.Personal.Usuario.NombreCompleto
                                 }
                             },
                             Atencion = new {
                                 id = s.Atencion.AreaId,
                                 titulo = s.Atencion.Titulo
                             }
                        }).ToList();
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "exito";
                        miRespuesta.data = result;
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
    }
}