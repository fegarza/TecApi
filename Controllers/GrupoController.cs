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
    [Route("api/[controller]")]
    [ApiController]
    public class GruposController : ControllerBase
    {

        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarGrupo(string id)
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
                               tutor = new
                               {
                                   id = s.Personal.Id,
                                   departamento = s.Personal.Departamento.Titulo,
                                   nombre = s.Personal.Usuario.Nombre,
                                   apellidoMaterno = s.Personal.Usuario.ApellidoMaterno,
                                   apellidoPaterno = s.Personal.Usuario.ApellidoPaterno,
                                   nombreCompleto = s.Personal.Usuario.NombreCompleto
                               },
                               estudiantes = s.Estudiantes
                                              .Select(v => new
                                              {
                                                  nombreCompleto = v.Usuario.NombreCompleto,
                                                  nombre = v.Usuario.Nombre,
                                                  apellidoMaterno = v.Usuario.ApellidoMaterno,
                                                  apellidoPaterno = v.Usuario.ApellidoMaterno,
                                                  numeroDeControl = v.NumeroDeControl,
                                                  correo = v.Usuario.Email,
                                                  sesiones = v.EstudiantesSesiones.Count(),
                                                  canalizaciones = v.Canalizaciones.Count(),
                                                  creditos = EstudiantesController.mostrarCreditos(v.NumeroDeControl),
                                                  semeste = v.Semestre,
                                                  genero = v.Usuario.Genero
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



        [Route("{id}/Sesiones")]
        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarSesiones(string id)
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
                                Asistencias = s.EstudiantesSesiones.Select(d=> new{ estudianteId = d.EstudianteId})
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

        [Route("{id}/Sesiones/{sesionId}")]
        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarAsistencias(string id, string sesionId)
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
                                nombreCompleto = d.Usuario.NombreCompleto,
                                nombre = d.Usuario.Nombre,
                                apellidoMaterno = d.Usuario.ApellidoMaterno,
                                apellidoPaterno = d.Usuario.ApellidoPaterno,
                                numeroDeControl = d.NumeroDeControl,
                                presente =  d.EstudiantesSesiones.Where( w=> w.EstudianteId == d.Id && w.SesionId == int.Parse(sesionId)).Count() > 0 ? 1 : 0
                            })
                        }); 
                      
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

        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarGrupos(string cant, string pag)
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
                               tutor = new
                               {
                                   id = s.Personal.Id,
                                   departamento = s.Personal.Departamento.Titulo,
                                   usuario = new
                                   {
                                       nombre = s.Personal.Usuario.Nombre,
                                       apellidoMaterno = s.Personal.Usuario.ApellidoMaterno,
                                       apellidoPaterno = s.Personal.Usuario.ApellidoPaterno,
                                       nombreCompleto = s.Personal.Usuario.NombreCompleto
                                   }
                               },
                               estudiantes = s.Estudiantes
                                              .Select(v => new
                                              {
                                                  nombreCompleto = v.Usuario.NombreCompleto,
                                                  nombre = v.Usuario.Nombre,
                                                  apellidoMaterno = v.Usuario.ApellidoMaterno,
                                                  apellidoPaterno = v.Usuario.ApellidoMaterno,
                                                  numeroDeControl = v.NumeroDeControl,
                                                  correo = v.Usuario.Email,
                                                  sesiones = v.EstudiantesSesiones.Count(),
                                                  canalizaciones = v.Canalizaciones.Count(),
                                                  creditos = EstudiantesController.mostrarCreditos(v.NumeroDeControl),
                                                  semeste = v.Semestre,
                                                  genero = v.Usuario.Genero
                                              }).ToList()
                           }
                            );



                    //

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
        public Respuesta InsertarNuevo([FromBody] Grupos grupo)
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
                        miRespuesta.data = MostrarGrupo(db.Grupos.Where(w => w.PersonalId == grupo.PersonalId).First().Id.ToString()).data;
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
        /*

        [AllowAnonymous]
        [HttpPost]
        [Route("{id}/Sesiones")]
        public Respuesta AgregarSesion([FromBody] Sesiones sesion)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                List<string> errores = new List<string>();
                if (String.IsNullOrEmpty(sesion.GrupoId.ToString()))
                {
                    errores.Add("el grupo no se ha dado (grupoId)");
                }
                if (String.IsNullOrEmpty(sesion.Tipo))
                {
                    errores.Add("el grupo no se ha dado el tipo de sesion (tipo)");
                }
                else
                {
                    if (sesion.Tipo.ToLower() != "i" && sesion.Tipo.ToLower() != "g")
                    {
                        errores.Add("el tipo de sesion no es valida (debe ser 'i' o 'g')");
                    }
                }
                if (db.Grupos.Where(w => w.Id == sesion.GrupoId).Count() == 0)
                {
                    errores.Add("el grupoId dado no existe");
                }
                if (errores.Count() > 0)
                {
                    miRespuesta.data = errores;
                    miRespuesta.code = StatusCodes.Status400BadRequest;
                    miRespuesta.mensaje = "no existe el grupo dado";
                }
                else
                {
                    try
                    {
                        db.Sesiones.Add(new Sesiones()
                        {
                            GrupoId = sesion.GrupoId,
                            Fecha = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd hh:mm tt")),
                            Tipo = sesion.Tipo
                        });
                        db.SaveChanges();
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "se ha insertado la sesion con exito";
                    }
                    catch(Exception ex)
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.data = ex;
                        miRespuesta.mensaje = "error interno";
                    }
                }
                return miRespuesta;
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("{id}/Sesiones")]
        public Respuesta MostrarSesiones(string id)
        {
            Respuesta miRespuesta = new Respuesta();

            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    miRespuesta.data = db.Sesiones.Where(w => w.GrupoId == int.Parse(id)).ToList();
                    miRespuesta.code = StatusCodes.Status200OK;
                    miRespuesta.mensaje = "exito";
                }
            }
            catch(Exception ex)
            {
                miRespuesta.code = StatusCodes.Status500InternalServerError;
                miRespuesta.mensaje = "error en el sistema";
                miRespuesta.data = ex;
            }



            return miRespuesta;
        }

        */
    }
}