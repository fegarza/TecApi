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
    public class PersonalesController : ControllerBase
    {

        [Route("{id}")]
        [AllowAnonymous]
        [HttpGet]
        public Respuesta mostrarPersonal(string id)
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

        [AllowAnonymous]
        [HttpGet]
        public Respuesta mostrarTodos(string cant, string pag)
        {
            Respuesta miRespuesta = new Respuesta();





            using (TUTORIASContext db = new TUTORIASContext())
            {
                try
                {



                    if (cant != null & pag != null)
                    {
                        try
                        {
                            int cantidad = int.Parse(cant);
                            int pagina = int.Parse(pag);
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
                           }).Skip((cantidad * pagina) - cantidad).Take(cantidad).ToList();


                            if (result.Count() > 0)
                            {
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.mensaje = "exito";
                                miRespuesta.data = result;
                            }
                            else
                            {
                                miRespuesta.code = StatusCodes.Status404NotFound;
                                miRespuesta.mensaje = "no hay registros";
                                miRespuesta.data = result;
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
                        if (result.Count() > 0)
                        {
                            miRespuesta.code = StatusCodes.Status200OK;
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


        [Route("Tec")]
        [AllowAnonymous]
        [HttpGet]
        public Respuesta mostrarTec()
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




        [AllowAnonymous]
        [HttpPost]
        public Respuesta Registrar([FromBody] Personales personal)
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
                            miRespuesta.data = mostrarPersonal(db.Personales.Where(w => w.Cve == personal.Cve).First().Id.ToString()).data;
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


        [AllowAnonymous]
        [HttpPut]
        public Respuesta modificar([FromBody] Personales personal)
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
                                catch
                                {
                                    miRespuesta.mensaje = "error al establecer datos al estudiante";
                                    miRespuesta.code = StatusCodes.Status400BadRequest;
                                }
                            }
                            else
                            {
                                miRespuesta.mensaje = "no existe un estudiante con ese numero de control";
                                miRespuesta.code = 500;

                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        miRespuesta.mensaje = "error en el sistema";
                        miRespuesta.code = 500;
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

    }
}