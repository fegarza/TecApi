using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecAPI.Models.Request;
using TecAPI.Models.Tec;
using TecAPI.Models.Tutorias;
using TecAPI.Response;

namespace TecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class ReporteController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="periodo"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [Route("{id}/Grupo")]
        [HttpGet]
        [Authorize(Roles = "A, C, J, D, T")]
        public Respuesta GetSemestralPorGrupo(string id, int periodo, int year)
        {
            DateTime fechaInicial;
            DateTime fechaFinal;
            if (periodo == 1)
            {
                fechaInicial = new DateTime(year, 1, 1);
                fechaFinal = new DateTime(year, 7, 1);
            }
            else
            {
                fechaInicial = new DateTime(year, 8, 1);
                fechaFinal = new DateTime(year, 12, 29);
            }




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
                               nombreDepartamento  = s.Personal.Departamento.Titulo,
                               jefeDepartamento = s.Personal.Departamento.Personales
                                                  .Where(w => w.Cargo == "D")
                                                  .Select( m =>  m.Usuario.NombreCompleto )
                                                  .FirstOrDefault(),
                               jefeTutor = s.Personal.Departamento.Personales
                                                  .Where(w => w.Cargo == "J")
                                                  .Select(m => m.Usuario.NombreCompleto)
                                                  .FirstOrDefault(),
                               tutorNombre = s.Personal.Usuario.NombreCompleto,
                               estudiantes = s.Estudiantes
                                              .Select(v => new
                                              {
                                                  usuario = new
                                                  {
                                                      nombreCompleto = v.Usuario.NombreCompleto,
                                                      genero = v.Usuario.Genero
                                                  },
                                                  numeroDeControl = v.NumeroDeControl,
                                                  sesiones = v.EstudiantesSesiones.Where(w => w.Sesion.Fecha >= fechaInicial && w.Sesion.Fecha <= fechaFinal).Count(),
                                                  sesionesIndividuales = v.EstudiantesSesionesIndividuales.Where(w => w.SesionIndividual.Fecha >= fechaInicial && w.SesionIndividual.Fecha <= fechaFinal).Count(),
                                                  sesionesEspeciales = v.SesionesEspeciales.Where(w => w.Fecha >= fechaInicial && w.Fecha <= fechaFinal).Count(),
                                                  sesionesIniciales = v.SesionesIniciales,
                                                  canalizaciones = v.Canalizaciones.Count(),
                                                  canalizacionesLista = v.Canalizaciones.Where(w => w.Fecha >= fechaInicial && w.Fecha <= fechaFinal).Select(h => new
                                                  {
                                                      area = h.Atencion.Area.Titulo
                                                  }).Distinct(),
                                                  semestre = v.Semestre,
                                                  estado = v.Estado
                                              }).ToList(),
                               estudiantesH1 = s.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "H" && (w.Semestre == 1 || w.Semestre == 2)).Count(),
                               estudiantesM1 = s.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "M" && (w.Semestre == 1 || w.Semestre == 2)).Count(),
                               estudiantesH = s.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "H" && w.Semestre > 2).Count(),
                               estudiantesM = s.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "M" && w.Semestre > 2).Count()
                           }
                            ); ;

                        if (result.Count() > 0)
                        {
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.data = result.FirstOrDefault();
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
                        miRespuesta.data = ex.Message;
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="periodo"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [Route("{id}/Departamento")]
        [HttpGet]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta GetSemestralPorDepartamento(string id, int periodo, int year)
        {

            DateTime fechaInicial;
            DateTime fechaFinal;
            if (periodo == 1)
            {
                fechaInicial = new DateTime(year, 1, 1);
                fechaFinal = new DateTime(year, 7, 1);
            }
            else
            {
                fechaInicial = new DateTime(year, 8, 1);
                fechaFinal = new DateTime(year, 12, 29);
            }

            //.Where(w => w.Fecha >= fechaInicial && w.Fecha <= fechaFinal)
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    var result = db.Departamentos
                         .Select(s => new
                         {
                             id = s.Id,
                             titulo = s.Titulo,
                             jefeTutor = s.Personales.Where(w => w.Cargo == "J").Select(c => c.Usuario.NombreCompleto).FirstOrDefault(),
                             jefeDepartamento = s.Personales.Where(w => w.Cargo == "D").Select(c => c.Usuario.NombreCompleto).FirstOrDefault(),

                             tutores = s.Personales.Select(r => new
                             {
                                 id = r.Usuario.Id,
                                 usuario = new
                                 {
                                     nombreCompleto = r.Usuario.NombreCompleto
                                 },
                                 r.Cargo,
                                 estudiantesAtendidos = r.Grupos.EstudiantesSesiones.Where(w => w.Sesion.Fecha >= fechaInicial && w.Sesion.Fecha <= fechaFinal).Count(),
                                 estudiantesAtendidosIndividual = r.Grupos.EstudiantesSesionesIndividuales.Where(w => w.SesionIndividual.Fecha >= fechaInicial && w.SesionIndividual.Fecha <= fechaFinal).Count(),
                                 estudiantes = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E").Count(),
                                 estudiantesH1 = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "H" && (w.Semestre == 1 || w.Semestre == 2)).Count(),
                                 estudiantesM1 = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "M" && (w.Semestre == 1 || w.Semestre == 2)).Count(),
                                 estudiantesH = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "H" && w.Semestre > 2).Count(),
                                 estudiantesM = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "M" && w.Semestre > 2).Count(),
                                 canalizacionesLista = r.Canalizaciones.GroupBy(g => g.Atencion.Titulo).Select(v => new { count = v.Count(), area = v.Key } )
                             }).Where(w => w.Cargo != "A")
                         }).Where(w => w.id == int.Parse(id));

                    if (result.Count() > 0)
                    {
                        miRespuesta.mensaje = "exito";
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.data = result.FirstOrDefault();
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
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="periodo"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [Route("Institucional")]
        [HttpGet]
       // [AllowAnonymous]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta GetSemestralPorInstitucion(string id, int periodo, int year)
        {

            DateTime fechaInicial;
            DateTime fechaFinal;
            if (periodo == 1)
            {
                fechaInicial = new DateTime(year, 1, 1);
                fechaFinal = new DateTime(year, 7, 1);
            }
            else
            {
                fechaInicial = new DateTime(year, 8, 1);
                fechaFinal = new DateTime(year, 12, 29);
            }

            //.Where(w => w.Fecha >= fechaInicial && w.Fecha <= fechaFinal)
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    


                    var dep = db.Departamentos
                         .Select(s => new
                         {
                             titulo = s.Titulo,
                             jefeTutorStr = s.Personales.Where(w => w.Cargo == "J")
                             .Select(c => c.Usuario.NombreCompleto)
                             .FirstOrDefault(),
                             jefeDepartamentoStr = s.Personales.Where(w => w.Cargo == "D")
                             .Select(c => c.Usuario.NombreCompleto)
                             .FirstOrDefault(),

                             personales = s.Personales.Select( r => new {
                                 estudiantesAtendidos = r.Grupos.EstudiantesSesiones.Where(w => w.Sesion.Fecha >= fechaInicial && w.Sesion.Fecha <= fechaFinal).Count(),
                                 estudiantesAtendidosIndividual = r.Grupos.EstudiantesSesionesIndividuales.Where(w => w.SesionIndividual.Fecha >= fechaInicial && w.SesionIndividual.Fecha <= fechaFinal).Count(),
                                 estudiantes = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E").Count(),
                                 estudiantesH1 = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "H" && (w.Semestre == 1 || w.Semestre == 2)).Count(),
                                 estudiantesM1 = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "M" && (w.Semestre == 1 || w.Semestre == 2)).Count(),
                                 estudiantesH = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "H" && w.Semestre > 2).Count(),
                                 estudiantesM = r.Grupos.Estudiantes.Where(w => w.Estado != "B" && w.Estado != "E" && w.Usuario.Genero == "M" && w.Semestre > 2).Count(),
                              }),

                             canalizacionesLista = s.Personales.Select( r=> new {
                                 canalizaciones = r.Canalizaciones.GroupBy(g => g.Atencion.Titulo).Select(v => new { count = v.Count(), area = v.Key })
                             })
                           


                         });




                    


                    if (dep.Count() > 0)
                    {
                        miRespuesta.mensaje = "exito";
                        miRespuesta.code = StatusCodes.Status200OK;
                       
                        String sub = TECDB.TraerNombreDelSubdirector();
                        var c = db.Personales.Where(w => w.Cargo == "C").Select(s => new { titulo = s.Titulo, usuario = s.Usuario } ).FirstOrDefault();


                        var result = new
                        {
                            nombreSubdirector = sub,
                            coordinador = c,
                            departamentos = dep.ToList()
                        };
                        miRespuesta.data = result;
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


    }
}