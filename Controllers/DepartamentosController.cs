using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                             titulo = s.Titulo,
                             tutorados = db.Estudiantes.Include(i => i.Grupo.Personal).Where(w => w.Grupo.Personal.DepartamentoId == s.Id).Count(),
                             tutores = s.Personales.Count()
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
        [Route("{id}/AccionesTutoriales/Grupales")]
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
                    
                    if (AccionesTomadas.Count() > 0)
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
                                    .Where(w => w.Activo == true && w.Tipo == "G" && w.Fecha <= primeraObligatoria.First().Fecha && !AccionesTomadas.Contains(w.Id)) 
                                    .Select(s=>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
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
                                    .Where(w => w.Activo == true && w.Tipo == "G" && !AccionesTomadas.Contains(w.Id) && w.Fecha > obligatorias.Last().Fecha)
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
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
                            Console.WriteLine("-------");
                             Console.WriteLine("ENTRA AQUI");
                             Console.WriteLine("-----");
                            // La primera obligatoria que se encuentre en orden ascendente
                            var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Obligatorio == true).OrderBy(o => o.Fecha);
                            if (primeraObligatoria.Count() > 0)
                            {
                                Console.WriteLine("-------");
                                Console.WriteLine("ENTRA AQUI  x2");
                                Console.WriteLine("-----");
                                miRespuesta.data = db.AccionesTutoriales.Where(w =>w.Activo == true && w.Tipo == "G" && w.Fecha <= primeraObligatoria.First().Fecha && !AccionesTomadas.Contains(w.Id))
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                    .ToList();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                            }
                            else
                            {
                                Console.WriteLine("-------");
                                Console.WriteLine("ENTRA AQUI x3");
                                Console.WriteLine("-----");
                                miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "G" && !AccionesTomadas.Contains(w.Id)).Select(s=>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
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
                        var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Obligatorio == true && w.Tipo == "G").OrderBy(o => o.Fecha);
                        if(primeraObligatoria.Count() > 0)
                        {
                            miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "G" && w.Fecha <= primeraObligatoria.First().Fecha)
                                .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                .ToList();
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.mensaje = "exito";
                        }
                        else
                        {
                            miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "G")
                                .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                .ToList();
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.mensaje = "exito";
                        }

                        
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
        /// Mostrar las acciones tutoriales pertenecientes
        /// </summary>
        /// <param name="id">identificador del departamento</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/AccionesTutoriales/Individuales")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexAccionesIndividuales(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {

                    //Acciones tutoriales tomadas
                    var AccionesTomadas = db.SesionesIndividuales
                        .Where(s => s.DepartamentoId == int.Parse(id))
                        .Select(s => s.AccionTutorialId);

                    if (AccionesTomadas.Count() > 0)
                    {
                        //La ultima obligatoria que ha tenido 
                        var obligatorias = db.SesionesIndividuales
                            .Where(s => s.DepartamentoId == int.Parse(id) && s.AccionTutorial.Obligatorio == true)
                            .OrderBy(o => o.AccionTutorial.Fecha);
                        if (obligatorias.Count() > 0)
                        {
                            // La primera obligatoria que se encuentre en orden ascendente y sea despues a la ultima obligatoria que ya tiene (aquella que no se haya tomado)
                            var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Tipo == "I" && w.Obligatorio == true && w.Fecha > obligatorias.Last().AccionTutorial.Fecha).OrderBy(o => o.Fecha);
                            if (primeraObligatoria.Count() > 0)
                            {
                                miRespuesta.data = db.AccionesTutoriales
                                    .Where(w => w.Activo == true && w.Tipo == "I" && w.Fecha <= primeraObligatoria.First().Fecha && !AccionesTomadas.Contains(w.Id))
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
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
                                    .Where(w => w.Activo == true && w.Tipo == "I" && !AccionesTomadas.Contains(w.Id) && w.Fecha > obligatorias.Last().Fecha)
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
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
                                miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "I" && w.Fecha <= primeraObligatoria.First().Fecha && !AccionesTomadas.Contains(w.Id))
                                    .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                    .ToList();
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                            }
                            else
                            {

                                miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "I" && !AccionesTomadas.Contains(w.Id)).Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
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
                        var primeraObligatoria = db.AccionesTutoriales.Where(w => w.Obligatorio == true && w.Tipo == "I").OrderBy(o => o.Fecha);
                        if (primeraObligatoria.Count() > 0)
                        {
                            miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "I" && w.Fecha <= primeraObligatoria.First().Fecha)
                                .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                .ToList();
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.mensaje = "exito";
                        }
                        else
                        {
                            miRespuesta.data = db.AccionesTutoriales.Where(w => w.Activo == true && w.Tipo == "I")
                                .Select(s =>
                                    new {
                                        id = s.Id,
                                        titulo = s.Titulo,
                                        fecha = s.Fecha.ToString("MM/dd/yyyy"),
                                        obligatorio = s.Obligatorio,
                                        contenido = s.Contenido
                                    })
                                .ToList();
                            miRespuesta.code = StatusCodes.Status200OK;
                            miRespuesta.mensaje = "exito";
                        }


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

        [HttpGet]
        [Route("{id}/Sesiones")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexSesiones(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.Sesiones.Include(i => i.AccionTutorial).OrderByDescending(o => o.Fecha).Where(w => w.DepartamentoId == int.Parse(id));
                    miRespuesta.code = StatusCodes.Status200OK;
                    miRespuesta.mensaje = "exito";
                    miRespuesta.data = result.ToList();
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


        [Route("{id}/Sesiones/count")]
        [Authorize(Roles = "A, C, J, D")]
        [HttpGet]
        public Respuesta CountSesiones(string id)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.code = StatusCodes.Status200OK;
            respuesta.mensaje = "Exito";
            using (TUTORIASContext db = new TUTORIASContext())
            {

                respuesta.data = new { count = db.Sesiones.Where(w => w.DepartamentoId == int.Parse(id)).Count() };
            }

            return respuesta;
        }


        [HttpGet]
        [Route("{id}/SesionesIndividuales")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexSesionesIndividuales(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.SesionesIndividuales.Include(i => i.AccionTutorial).OrderByDescending(o => o.Fecha).Where(w => w.DepartamentoId == int.Parse(id));
                    miRespuesta.code = StatusCodes.Status200OK;
                    miRespuesta.mensaje = "exito";
                    miRespuesta.data = result.ToList();
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


        [Route("{id}/SesionesIndividuales/count")]
        [Authorize(Roles = "A, C, J, D")]
        [HttpGet]
        public Respuesta CountSesionesIndividuales(string id)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.code = StatusCodes.Status200OK;
            respuesta.mensaje = "Exito";
            using (TUTORIASContext db = new TUTORIASContext())
            {

                respuesta.data = new { count = db.SesionesIndividuales.Where(w => w.DepartamentoId == int.Parse(id)).Count() };
            }

            return respuesta;
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
                             estado = s.Estado,
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
        [Route("{id}/Canalizaciones/count")]
        [Authorize(Roles = "A, C, J, D")]
        [HttpGet]
        public Respuesta CountCanalizaciones(string id)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.code = StatusCodes.Status200OK;
            respuesta.mensaje = "Exito";
            using (TUTORIASContext db = new TUTORIASContext())
            {

                respuesta.data = new { count = db.Canalizaciones.Include(i => i.Personal).Where(w => w.Personal.DepartamentoId == int.Parse(id)).Count() };
            }

            return respuesta;
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

        [Route("{id}/Personales/count")]
        [Authorize(Roles = "A, C, J, D")]
        [HttpGet]
        public Respuesta CountPersonales(string id)
        {
            Respuesta respuesta = new Respuesta();
            respuesta.code = StatusCodes.Status200OK;
            respuesta.mensaje = "Exito";
            using (TUTORIASContext db = new TUTORIASContext())
            {

                respuesta.data = new { count = db.Personales.Where(w => w.DepartamentoId == int.Parse(id)).Count() };
            }

            return respuesta;
        }

        [HttpPost]
        [Route("Asesorias")]
        public Respuesta Store([FromBody] DepartamentosAsesorias departamentoAsesorias)
        {
            Respuesta respuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    db.DepartamentosAsesorias.Add(departamentoAsesorias);
                    db.SaveChanges();
                    respuesta.code = StatusCodes.Status200OK;
                    respuesta.mensaje = "Se ha creado la relacion con la asesoria";

                }
                catch (Exception e)
                {
                    respuesta.data = e;
                    respuesta.code = StatusCodes.Status400BadRequest;
                    respuesta.mensaje = "Error al agregar la relacion con la asesoria";
                }
            }
            return respuesta;
        }

        /// <summary>
        /// Mostrar todo el personal de un departamento dado
        /// </summary>
        /// <param name="id">identificador del departamento</param>
        /// <returns>El personal de un departamento</returns>
        [HttpGet]
        [Route("{id}/Asesorias")]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta IndexAsesorias(string id, int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();

            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    var result = db.DepartamentosAsesorias.Where(w => w.DepartamentoId == int.Parse(id) || w.Asesoria.General == true);

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