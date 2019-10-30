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
using TecAPI.Response;

namespace TecAPI.Controllers
{

    /// <summary>
    /// Todo lo relacionado a los grupos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class GruposController : ControllerBase
    {
        /// <summary>
        /// Mostrar todos los grupos
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta Index(int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();

            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Grupos
                        .Include(i => i.Estudiantes)
                        .Include(i => i.Personal)
                      .Select(s =>
                           new
                           {
                               id = s.Id,
                               salon = s.Salon,
                               personal = new
                               {
                                   id = s.Personal.Id,
                                   departamento = s.Personal.Departamento.Titulo,
                                   usuario = new
                                   {
                                       nombreCompleto = s.Personal.Usuario.NombreCompleto
                                   }
                               },
                               estudiantes = s.Estudiantes
                                              .Select(v => new
                                              {
                                                  usuario = new
                                                  {
                                                      nombreCompleto = v.Usuario.NombreCompleto,
                                                      nombre = v.Usuario.Nombre,
                                                      apellidoMaterno = v.Usuario.ApellidoMaterno,
                                                      apellidoPaterno = v.Usuario.ApellidoMaterno,
                                                      correo = v.Usuario.Email,
                                                      genero = v.Usuario.Genero
                                                  },
                                                  numeroDeControl = v.NumeroDeControl,
                                                  sesiones = v.EstudiantesSesiones.Count(),
                                                  canalizaciones = v.Canalizaciones.Count(),
                                                  creditos = EstudiantesController.IndexCreditos(v.NumeroDeControl),
                                                  semeste = v.Semestre
                                              }).ToList()
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
        /// Mostrar un grupo en especifico
        /// </summary>
        /// <param name="id">Identificador del grupo</param>
        /// <returns>Modelo de respuesta</returns>
        [Route("{id}")]
        [HttpGet]
        public Respuesta Show(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            if (id != null)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Grupos
                            .Where(r => r.Id == int.Parse(id))
                            .Include(i => i.Estudiantes)
                            .Include(i => i.Personal)
                            .Select(s =>
                           new
                           {
                               id = s.Id,
                               salon = s.Salon,
                               personal = new
                               {
                                   id = s.Personal.Id,
                                   departamento = s.Personal.Departamento.Titulo,
                                   usuario = new {
                                       nombreCompleto = s.Personal.Usuario.NombreCompleto
                                   }
                               },
                               estudiantes = s.Estudiantes
                                              .Select(v => new
                                              {
                                                  usuario = new
                                                  {
                                                      nombreCompleto = v.Usuario.NombreCompleto,
                                                      nombre = v.Usuario.Nombre,
                                                      apellidoMaterno = v.Usuario.ApellidoMaterno,
                                                      apellidoPaterno = v.Usuario.ApellidoMaterno,
                                                      correo = v.Usuario.Email,
                                                      genero = v.Usuario.Genero
                                                  },
                                                  numeroDeControl = v.NumeroDeControl,
                                                  sesiones = v.EstudiantesSesiones.Count(),
                                                  canalizaciones = v.Canalizaciones.Count(),
                                                  creditos = EstudiantesController.IndexCreditos(v.NumeroDeControl),
                                                  semeste = v.Semestre,
                                              }).ToList()
                           }
                            );

                        if (result.Count() > 0)
                        {
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = result.First();
                        }
                        else
                        {
                            miRespuesta.code = StatusCodes.Status404NotFound;
                            miRespuesta.mensaje = "no existe ningun grupo con dicho id";
                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.mensaje = "error interno";
                    }

                }
            }
            else
            {
                miRespuesta.code = StatusCodes.Status409Conflict;
                miRespuesta.mensaje = "no se ha dado el id";
            }



            return miRespuesta;
        }

        /// <summary>
        /// Insertar un grupo
        /// </summary>
        /// <param name="grupo">Objeto del grupo en formato JSON</param>
        /// <returns>Un modelo de respuesta</returns>
        [HttpPost]
        public Respuesta Store([FromBody] Grupos grupo)
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
                        db.Grupos.Add(grupo);
                        db.SaveChanges();
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "Insertado correctamente";
                        miRespuesta.data = Show(db.Grupos.Where(w => w.PersonalId == grupo.PersonalId).First().Id.ToString()).data;
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

        /// <summary>
        /// Mostrar las sesiones de un grupo en especifico
        /// </summary>
        /// <param name="id">Identificador del grupo</param>
        /// <returns>Modelo de respuesta</returns>
        [Route("{id}/Sesiones")]
        [HttpGet]
        public Respuesta ShowSesiones(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            if (id != null)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var personal = db.Grupos.Include(i => i.Personal).Where(w => w.Id == int.Parse(id)).First().Personal;
                        var result = db.Sesiones.Where(w => w.DepartamentoId == personal.DepartamentoId)
                            .Select(s => new
                            {
                                id = s.Id,
                                fecha = s.Fecha.ToShortDateString(),
                                AccionTutorial = new
                                {
                                    id = s.AccionTutorial.Id,
                                    titulo = s.AccionTutorial.Titulo,
                                    contenido = s.AccionTutorial.Contenido
                                },
                                Asistencias = s.EstudiantesSesiones.Select(d => new { estudianteId = d.EstudianteId })
                            }
                            );

                        if (result.Count() > 0)
                        {
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = result.ToList();
                        }
                        else
                        {
                            miRespuesta.code = StatusCodes.Status404NotFound;
                            miRespuesta.mensaje = "no existe ningun grupo con dicho id";
                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.mensaje = "error interno";
                    }

                }
            }
            else
            {
                miRespuesta.code = StatusCodes.Status409Conflict;
                miRespuesta.mensaje = "no se ha dado el id";
            }



            return miRespuesta;
        }

        /// <summary>
        /// Mostrar una sesion en especifico de un grupo en especifico
        /// </summary>
        /// <param name="id">Identificador del grupo</param>
        /// <param name="sesionId">Identificador de la sesion</param>
        /// <returns>Modelo de respuesta</returns>
        [Route("{id}/Sesiones/{sesionId}")]
        [HttpGet]
        public Respuesta ShowAsistencias(string id, string sesionId)
        {
            Respuesta miRespuesta = new Respuesta();
            if (id != null && sesionId != null)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Grupos.Where(w => w.Id == int.Parse(id)).Select(s =>
                        new
                        {
                            asistencias = s.Estudiantes.Select(d => new
                            {
                                id = d.Id,
                                grupoId = d.GrupoId,
                                usuario = new
                                {
                                    nombreCompleto = d.Usuario.NombreCompleto,
                                    nombre = d.Usuario.Nombre,
                                    apellidoMaterno = d.Usuario.ApellidoMaterno,
                                    apellidoPaterno = d.Usuario.ApellidoPaterno,
                                },
                                numeroDeControl = d.NumeroDeControl,
                                presente = d.EstudiantesSesiones.Where(w => w.EstudianteId == d.Id && w.SesionId == int.Parse(sesionId)).Count() > 0 ? 1 : 0
                            })
                        });

                        if (result.Count() > 0)
                        {
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = result.First().asistencias;
                        }
                        else
                        {
                            miRespuesta.code = StatusCodes.Status404NotFound;
                            miRespuesta.mensaje = "no existe ningun grupo con dicho id";
                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.mensaje = "error interno";
                    }

                }
            }
            else
            {
                miRespuesta.code = StatusCodes.Status409Conflict;
                miRespuesta.mensaje = "no se ha dado el id";
            }



            return miRespuesta;
        }

        /// <summary>
        /// Insertar la lista de asistencias en una sesion
        /// </summary>
        /// <param name="id">id del grupo</param>
        /// <param name="sesionId">id de la sesion</param>
        /// <param name="estudiantes">arreglo de estudiantes</param>
        /// <returns>Un modelo de respuesta</returns>
        [Route("{id}/Sesiones/{sesionId}")]
        [HttpPost]
        public Respuesta StoreSesion(string id, string sesionId, [FromBody] List<Estudiantes> estudiantes)
        {
            Respuesta miRespuesta = new Respuesta();

            if (id != null && sesionId != null)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    var grupo = db.Grupos.Where(w => w.Id == int.Parse(id)).Select(s => new { departamentoId = s.Personal.DepartamentoId });

                    if (grupo.Count() > 0)
                    {
                        //Existe el grupo
                        if (db.Sesiones.Where(w => w.Id == int.Parse(sesionId) && w.DepartamentoId == grupo.First().departamentoId).Count() > 0)
                        {
                            //Quitar todas las asistencias 
                            db.EstudiantesSesiones.RemoveRange(db.EstudiantesSesiones.Where(w => w.SesionId == int.Parse(sesionId) && w.GrupoId == int.Parse(id)));
                            db.SaveChanges();
                            //Empezar la toma de asistencias
                            List<string> errores = new List<string>();
                            foreach (Estudiantes e in estudiantes)
                            {
                                try
                                {
                                    //Estudiante existe y pertenece al grupo
                                    if (e.GrupoId == int.Parse(id) && db.Estudiantes.Where(w => w.Id == e.Id).Count() > 0)
                                    {
                                        //Colocar asistencia
                                        db.EstudiantesSesiones.Add(new EstudiantesSesiones() { EstudianteId = e.Id, GrupoId = int.Parse(id), SesionId = int.Parse(sesionId) });
                                        db.SaveChanges();
                                    }
                                }
                                catch
                                {
                                    errores.Add("Error con el estudiante con ID: " + e.Id);
                                }
                            }
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = errores;
                            if (errores.Count() > 0)
                            {
                                miRespuesta.mensaje = "se ejecuto pero con errores";
                            }
                            else
                            {
                                miRespuesta.mensaje = "exito";
                            }

                        }
                        else
                        {
                            //No la combinacion de sesion y grupo
                            miRespuesta.code = StatusCodes.Status409Conflict;
                            miRespuesta.mensaje = "el grupo no tiene esa sesion";
                        }
                    }
                    else
                    {
                        //No existe el grupo
                        miRespuesta.code = StatusCodes.Status409Conflict;
                        miRespuesta.mensaje = "el grupo dado no existe";
                    }
                }
            }
            else
            {
                miRespuesta.code = StatusCodes.Status409Conflict;
                miRespuesta.mensaje = "no se ha dado el id";
            }
            return miRespuesta;
        }        
         
    }
}