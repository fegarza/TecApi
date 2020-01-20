using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
    /// <summary>
    /// Todo lo relacionado a los personales
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "A, C, J, D, T")]
    public class PersonalesController : ControllerBase
    {

        /// <summary>
        /// Mostrar un personal en especifico
        /// </summary>
        /// <param name="id">Identificador del personal</param>
        /// <returns>Un modelo de respuesta</returns>
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
                        var result = db.Personales
                           .Include(p => p.Grupos)
                           .Include(p => p.Usuario)
                           .Include(p => p.Canalizaciones)
                           .Where(w => w.Id == int.Parse(id))
                           .Select(s => new
                           {
                               id = s.Id,
                               usuario = new
                               {
                                   id = s.Usuario.Id,
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
                               grupo = s.Grupos,
                               grupoId = s.Grupos.Id.ToString()
                           });

                        if (result.Count() > 0)
                        {
                            miRespuesta.code = 202;
                            miRespuesta.data = result.First();
                        }
                        else
                        {
                            miRespuesta.code = 404;
                            miRespuesta.mensaje = "no existe ningun personal con dicho id";
                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.code = 500;
                        miRespuesta.mensaje = "error interno";
                    }

                }
            }
            else
            {
                miRespuesta.code = 404;
                miRespuesta.mensaje = "no se ha dado el id";
            }



            return miRespuesta;
        }

        /// <summary>
        /// Mostrar un personal en especifico
        /// </summary>
        /// <param name="id">Identificador del personal</param>
        /// <returns>Un modelo de respuesta</returns>
        [Route("{id}/grupo")]
        [HttpGet]
        public Respuesta ShowGrupo(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            if (id != null)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Personales
                           .Include(p => p.Grupos)
                           .Where(w => w.Id == int.Parse(id))
                           .Select(s => new
                           {
                               grupoId = s.Grupos.Id.ToString()
                           });

                        if (result.Count() > 0)
                        {
                            GruposController miGrupo = new GruposController();
                            miRespuesta = miGrupo.Show(result.First().grupoId);
                        }
                        else
                        {
                            miRespuesta.code = 404;
                            miRespuesta.mensaje = "no existe ningun personal con dicho id";
                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.code = 500;
                        miRespuesta.mensaje = "error interno";
                    }
                } 
            }
            else
            {
                miRespuesta.code = 404;
                miRespuesta.mensaje = "no se ha dado el id";
            }



            return miRespuesta;
        }


        /// <summary>
        /// Mostrar todos los personales del sistema
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        [Authorize(Roles = "A, J, D, C")]

        public Respuesta Index(int cant, int pag, string orderBy)
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
                    miRespuesta.data = ex;
                    miRespuesta.mensaje = "error interno";
                }

            }




            return miRespuesta;
        }

        /// <summary>
        /// Mostrar todo el personal del tecnologico
        /// </summary>
        /// <returns>un modelo de respuesta</returns>
        [Route("Tec")]
        [HttpGet]
        [Authorize(Roles = "A, J, D, C")]
        public Respuesta Index()
        {
            Respuesta miRespuesta = new Respuesta();
            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {
                    miRespuesta.code = StatusCodes.Status200OK;

                    miRespuesta.data = TECDB.MostrarPersonales();
                    miRespuesta.mensaje = "exito";

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

        /// <summary>
        /// Insertar un personal
        /// </summary>
        /// <param name="personal">El objeto del personal en formato JSON</param>
        /// <returns>Un modelo de respuesta</returns>
        [HttpPost]
        [Authorize(Roles = "A, J, D, C")]
        public Respuesta Store([FromBody] Personales personal)
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

                    TecAPI.Models.Tutorias.Usuarios miUsuario = TECDB.TraerDatosPersonal(personal.Cve);
                    miUsuario.Email = personal.Usuario.Email;
                    miUsuario.Clave = personal.Usuario.Clave;
                    miUsuario.Tipo = "P";
                    personal.Usuario = miUsuario;
                    personal.Grupos = new Grupos() { };
                    using (TUTORIASContext db = new TUTORIASContext())
                    {
                        try
                        {
                            db.Personales.Add(personal);
                            db.SaveChanges();
                            miRespuesta.mensaje = "Se ha insertado correctamente";
                            miRespuesta.code = 200;
                            miRespuesta.data = Show(db.Personales.Where(w => w.Cve == personal.Cve).First().Id.ToString()).data;
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

        /// <summary>
        /// Actualizar un personal
        /// </summary>
        /// <param name="personal">El objeto del personal en formato JSON</param>
        /// <returns>Un modelo de respuesta</returns>
        [HttpPut]
        [Authorize(Roles = "A, J, D, C")]
        public Respuesta Update([FromBody] Personales personal)
        {
            Respuesta miRespuesta = new Respuesta();

            if (personal != null)
            {
                if (!String.IsNullOrEmpty(personal.Id.ToString()))
                {
                    try
                    {
                        using (TUTORIASContext db = new TUTORIASContext())
                        {

                            var result = db.Personales.Where(r => r.Id == personal.Id);
                            if (result.Count() > 0)
                            {
                                try
                                {
                                    List<string> acciones = new List<string>();
                                    List<string> errores = new List<string>();
                                    //Modificar el titulo
                                    if (personal.TituloId != 0)
                                    {
                                        if (db.Titulos.Where(r => r.Id == personal.TituloId).Count() > 0)
                                        {
                                            result.First().TituloId = personal.TituloId;
                                            acciones.Add("se ha cambiado el titulo con exito");

                                        }
                                        else
                                        {
                                            errores.Add("no existe ningun titulo con el id dado");
                                        }
                                    }
                                    //Modificar cargo
                                    if (personal.Cargo != null)
                                    {
                                        if (personal.Cargo.ToLower() == "d" || personal.Cargo.ToLower() == "j" || personal.Cargo.ToLower() == "t" || personal.Cargo.ToLower() == "c")
                                        {
                                            result.First().Cargo = personal.Cargo;
                                            acciones.Add("se ha cambiado el cargo con exito");
                                        }
                                        else
                                        {
                                            errores.Add("no existe el cargo dado");

                                        }
                                    }

                                    if (personal.DepartamentoId != 0)
                                    {
                                        if (db.Departamentos.Where(r => r.Id == personal.DepartamentoId).Count() > 0)
                                        {
                                            result.First().DepartamentoId = personal.DepartamentoId;
                                            acciones.Add("se ha cambiado el departamento con exito");

                                        }
                                        else
                                        {
                                            errores.Add("no existe ningun departamento con el id dado");
                                        }
                                    }

                                    db.SaveChanges();
                                    miRespuesta.mensaje = "exito";
                                    miRespuesta.data = new { acciones, errores };
                                    miRespuesta.code = StatusCodes.Status200OK;
                                }
                                catch (System.Data.DataException dex)
                                {
                                    if (dex.InnerException is SqlException)
                                        miRespuesta.mensaje = dex.Message;
                                    miRespuesta.code = StatusCodes.Status400BadRequest;

                                }
                                catch (SqlException sqlex)
                                {
                                    miRespuesta.mensaje = sqlex.Message;
                                    miRespuesta.code = StatusCodes.Status400BadRequest;

                                }
                                catch (Exception e)
                                {
                                    Console.Write("\n----------------ERROR---------------\n");
                                    Console.Write(e);
                                    miRespuesta.code = StatusCodes.Status400BadRequest;
                                    miRespuesta.mensaje = "Error al editar personal";
                                   
                                    miRespuesta.data = e;
                                    // non-SQL exception handling
                                }
                            }
                            else
                            {
                                miRespuesta.mensaje = "no existe un personal con ese id";
                                miRespuesta.code = 500;

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.mensaje = "error en el sistema";
                        miRespuesta.code = 500;
                        miRespuesta.data = ex;
                    }
                }
                else
                {
                    miRespuesta.code = StatusCodes.Status400BadRequest;
                    miRespuesta.mensaje = "no se ha dado el numero de control";
                }
            }
            else
            {
                miRespuesta.code = StatusCodes.Status400BadRequest;
                miRespuesta.mensaje = "los datos no son enviados no son correctos";
            }

            return miRespuesta;

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
                
                respuesta.data = new {count =  db.Personales.Count()};
            }
                
            return respuesta;
        }
    }
}