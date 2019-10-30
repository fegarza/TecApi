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
    /// <summary>
    /// Todo lo relacionado con los departamentos
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class DepartamentosController : ControllerBase
    {
        /// <summary>
        /// Mostrar todos los departamentos
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
                    var result = db.Departamentos
                         .Select(s => new
                         {
                             id = s.Id,
                             titulo = s.Titulo
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
        /// Mostrar un departamento en especifico
        /// </summary>
        /// <param name="id">id </param>
        /// <returns>un departamento</returns>
        [Route("{id}")]
        [Authorize(Roles = "A, C, J, D")]
        [HttpGet]
        [AllowAnonymous]
        public Respuesta Show(string id)
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
                         }).Where(w => w.id == int.Parse(id));
                     
                    if (result.Count() > 0)
                    {
                        miRespuesta.mensaje = "exito";
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.data = result.First();
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
        /// Mostrar las acciones tutoriales pertenecientes
        /// </summary>
        /// <param name="id">identificador del departamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/AccionesTutoriales")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexAcciones(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
        
                    //Acciones tutoriales tomadas
                    var AccionesTomadas = db.Sesiones
                        .Where(s => s.DepartamentoId == int.Parse(id))
                        .Select(s => s.AccionTutorialId);

                    if(AccionesTomadas.Count() > 0)
                    {
                        //La ultima obligatoria que ha tenido 
                        var obligatorias = db.Sesiones
                            .Where(s => s.DepartamentoId == int.Parse(id) && s.AccionTutorial.Obligatorio == true)
                            .OrderBy(o => o.AccionTutorial.Fecha);
                        if (obligatorias.Count() > 0)
                        {
                            // La primera obligatoria que se encuentre en orden ascendente y sea despues a la ultima obligatoria que ya tiene (aquella que no se haya tomado)
                            var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Obligatorio == true && w.Fecha > obligatorias.Last().AccionTutorial.Fecha).OrderBy(o => o.Fecha);
                            if (primeraObligatoria.Count() > 0)
                            {
                                miRespuesta.data = db.AccionesTutoriales
                                    .Where(w => w.Fecha <= primeraObligatoria.First()
                                    .Fecha && !AccionesTomadas.Contains(w.Id))
                                    .Select(s=>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToShortDateString(),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                    .ToList();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                              }
                            else
                            {
                                miRespuesta.data = db.AccionesTutoriales
                                    .Where(w => !AccionesTomadas.Contains(w.Id) && w.Fecha > obligatorias.Last().Fecha)
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToShortDateString(),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                    .ToList();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                            }
                        }
                        else
                        {
                            // La primera obligatoria que se encuentre en orden ascendente
                            var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Obligatorio == true).OrderBy(o => o.Fecha);
                            if (primeraObligatoria.Count() > 0)
                            {
                                miRespuesta.data = db.AccionesTutoriales.Where(w => w.Fecha <= primeraObligatoria.First().Fecha)
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToShortDateString(),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                    .ToList();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                            }
                            else
                            {
                                miRespuesta.data = db.AccionesTutoriales.Select(s=>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToShortDateString(),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                    .ToList();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                            }
                        }
                    }
                    else
                    {
                        // La primera obligatoria que se encuentre en orden ascendente
                        var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Obligatorio == true).OrderBy(o => o.Fecha);
                        if(primeraObligatoria.Count() > 0)
                        {
                            miRespuesta.data = db.AccionesTutoriales.Where(w => w.Fecha <= primeraObligatoria.First().Fecha)
                                .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToShortDateString(),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                .ToList();
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.mensaje = "exito";
                        }
                        else
                        {
                            miRespuesta.data = db.AccionesTutoriales
                                .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToShortDateString(),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                .ToList();
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.mensaje = "exito";
                        }

                        
                    }

                  




                    /*


                    var sesionesObligatoriasTomadas =
                        db.Sesiones
                        .Include(i => i.AccionTutorial)
                        .Where(s => s.DepartamentoId == int.Parse(id) && s.AccionTutorial.Obligatorio == true);

                    var AccionesSinTomar = db.AccionesTutoriales
                        .Where(w => !AccionesTomadas.Contains(w.Id))
                        .Select(s =>
                        new
                        {
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

                    */


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
        /// Mostrar todos los grupos pertenecientes
        /// </summary>
        /// <param name="id">identificador del departamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/Grupos")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta Indexgrupos(string id)
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
                            personal = new
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

        /// <summary>
        /// Mostrar todas las canalizaciones pertenecientes
        /// </summary>
        /// <param name="id">identificador del departamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/Canalizaciones")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexCanalizaciones(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Canalizaciones.Where(w => w.Personal.DepartamentoId == byte.Parse(id) && w.Estado.ToLower() == "a")
                         .Select(s => new
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

        /// <summary>
        /// Mostrar todo el personal de un departamento dado
        /// </summary>
        /// <param name="id">identificador del departamento</param>
        /// <returns>El personal de un departamento</returns>
        [HttpGet]
        [Route("{id}/Personales")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexPersonales(string id, int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();

            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Personales
                       .Include(p => p.Grupos)
                       .Include(p => p.Usuario)
                       .Include(p => p.Canalizaciones)
                       .Select(s => new
                       {
                           id = s.Id,
                           usuario = new
                           {
                               nombreCompleto = s.Usuario.NombreCompleto,
                               nombre = s.Usuario.Nombre,
                               apellidoMaterno = s.Usuario.ApellidoMaterno,
                               apellidoPaterno = s.Usuario.ApellidoPaterno,
                               genero = s.Usuario.Genero,
                               email = s.Usuario.Email
                           },
                           cargo = s.Cargo,
                           departamento = new
                           {
                               id = s.DepartamentoId,
                               titulo = s.Departamento.Titulo
                           },
                           tutorados = s.Grupos.Estudiantes.Count(),
                           canalizaciones = s.Canalizaciones.Count(),
                           posts = s.Posts.Count(),
                           grupoId = s.Grupos.Id.ToString()
                       }).Where(w=> w.departamento.id == int.Parse(id));

                    if (!String.IsNullOrEmpty(orderBy))
                    {
                        result = result.OrderBy(orderBy);
                    }
                    if (cant != 0 && pag != 0)
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
                    miRespuesta.data = ex;
                    miRespuesta.mensaje = "error interno";
                }

            }

            return miRespuesta;




        }


        [HttpGet]
        [Route("{id}/Sesiones")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexSesiones(string id, int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();

            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Sesiones
                        .Include( i => i.AccionTutorial)
                        .Select(s => new
                        {
                            fecha = s.Fecha,
                            accionTutorial = new 
                                            { 
                                                id = s.AccionTutorial.Id,
                                                titulo = s.AccionTutorial.Fecha,
                                                contenido = s.AccionTutorial.Contenido,
                                                fecha = s.AccionTutorial.Fecha,
                                                activo = s.AccionTutorial.Activo
                                            },
                            s.DepartamentoId
                        })
                        .Where(w => w.DepartamentoId == int.Parse(id));

                    if (!String.IsNullOrEmpty(orderBy))
                    {
                        result = result.OrderBy(orderBy);
                    }
                    if (cant != 0 && pag != 0)
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
                    miRespuesta.data = ex;
                    miRespuesta.mensaje = "error interno";
                }

            }

            return miRespuesta;




        }

    }
}