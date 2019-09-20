using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TecAPI.Models.Escolares;
using TecAPI.Models.Request;
using TecAPI.Models.Tec;
using TecAPI.Models.Tutorias;
using TecAPI.Response;
 
namespace TecAPI.Controllers
{


    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {

        public static int mostrarCreditos(string _numeroControl)
        {
            int creditos = 0;
            using (EscolaresContext db2 = new EscolaresContext())
            {
                creditos = db2.Creditos.Where(r => r.NumeroDeControl == _numeroControl).Count();
            }
            return creditos;
        }


        [AllowAnonymous]
        [HttpGet]
        public Respuesta mostrarTodos(string cant, string pag, string correo)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    
                   
                    var result = db.Estudiantes
                        .Include(i => i.Grupo)
                        .Include(i => i.Usuario)
                        .Select(s => new
                        {
                            id = s.Id,
                            usuario = new
                            {
                                nombre = s.Usuario.Nombre,
                                apellidoMaterno = s.Usuario.ApellidoMaterno,
                                apellidoPaterno = s.Usuario.ApellidoPaterno,
                                nombreCompleto = s.Usuario.NombreCompleto,
                                email = s.Usuario.Email,
                                tipo = "E",
                                genero = s.Usuario.Genero
                            },
                            grupo = new
                            {
                                id = s.GrupoId,
                                salon = s.Grupo.Salon,
                                tutor = new
                                {
                                    usuario = new
                                    {
                                        nombreCompleto = s.Grupo.Personal.Usuario.NombreCompleto
                                    }
                                }
                            },
                            numeroDeControl = s.NumeroDeControl,
                            sesiones = (s.EstudiantesSesiones.Count() + s.SesionesIniciales),
                            canalizaciones = s.Canalizaciones.Count(),
                            creditos = mostrarCreditos(s.NumeroDeControl),
                            semestre = s.Semestre

                        });
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
                        if(result.Count() > 0)
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


                }
            }
            catch(Exception ex)
            {
                miRespuesta.code = StatusCodes.Status500InternalServerError;
                miRespuesta.mensaje = "error";
                miRespuesta.data = ex;
             }
            return miRespuesta;

        }

        [Route("{numeroDeControl}")]
        [AllowAnonymous]
        [HttpGet]
        public Respuesta mostrarAlumno(string numeroDeControl)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    int creditos = 0;
                    using (EscolaresContext db2 = new EscolaresContext())
                    {
                        creditos = db2.Creditos.Where(r => r.NumeroDeControl == numeroDeControl).Count();
                    }

                    var result = db.Estudiantes
                        .Include(i => i.Grupo)
                        .Include(i => i.Usuario)
                        .Select(s => new
                        {
                            id = s.Id,
                            usuario = new
                            {
                                nombre = s.Usuario.Nombre,
                                apellidoMaterno = s.Usuario.ApellidoMaterno,
                                apellidoPaterno = s.Usuario.ApellidoPaterno,
                                nombreCompleto = s.Usuario.NombreCompleto,
                                email = s.Usuario.Email,
                                tipo = "E",
                                genero = s.Usuario.Genero
                            },
                            grupo = new
                            {
                                id = s.GrupoId,
                                salon = s.Grupo.Salon,
                                tutor = new
                                {
                                    usuario = new
                                    {
                                        nombreCompleto = s.Grupo.Personal.Usuario.NombreCompleto
                                    }
                                }
                            },
                            numeroDeControl = s.NumeroDeControl,
                            sesiones = (s.EstudiantesSesiones.Count() + s.SesionesIniciales),
                            canalizaciones = s.Canalizaciones.Count(),
                            creditos = creditos,
                            semestre = s.Semestre

                        }).Where(r => r.numeroDeControl == numeroDeControl);

                    if (result.Count() > 0)
                    {
                        miRespuesta.data = result.First();
                        miRespuesta.code = StatusCodes.Status200OK;
                    }
                    else
                    {
                        miRespuesta.mensaje = "no existe un estudiante con los datos solicitados";
                        miRespuesta.code = StatusCodes.Status500InternalServerError;

                    }

                }
            }
            catch (Exception ex)
            {
                miRespuesta.mensaje = "error interno";
                miRespuesta.code = StatusCodes.Status500InternalServerError;
                miRespuesta.data = ex;
            }
            return miRespuesta;

        }


        [AllowAnonymous]
        [HttpPut]
        public Respuesta modificar([FromBody] Estudiantes miEstudiante)
        {

            Respuesta miRespuesta = new Respuesta();

            if (miEstudiante != null)
            {
                if(!String.IsNullOrEmpty(miEstudiante.NumeroDeControl))
                {
                    try
                    {
                        using (TUTORIASContext db = new TUTORIASContext())
                        {

                            var result = db.Estudiantes.Where(r => r.NumeroDeControl == miEstudiante.NumeroDeControl);
                            if (result.Count() > 0)
                            {
                                try
                                {

                                    List<string> acciones = new List<string>();
                                    List<string> errores = new List<string>();
                                    if(miEstudiante.GrupoId != null)
                                    {
                                        if (db.Grupos.Where(r => r.Id == miEstudiante.GrupoId).Count() > 0)
                                        {
                                            result.First().GrupoId = miEstudiante.GrupoId;
                                            acciones.Add("se ha cambiado el grupo con exito");

                                        }
                                        else
                                        {
                                            errores.Add("no existe ningun grupo con el id dado");
                                        }
                                    }
                                    if (miEstudiante.CarreraId != 0)
                                    {
                                        if(db.Carreras.Where(r=> r.Id == miEstudiante.CarreraId).Count() >0)
                                        {
                                            result.First().CarreraId = miEstudiante.CarreraId;
                                            acciones.Add("se ha cambiado la carrera con exito");
                                        }
                                        else
                                        {
                                            errores.Add("no existe ninguna carrera con el id dado");

                                        }
                                    }
                                    if(miEstudiante.SesionesIniciales != 0)
                                    {   
                                            result.First().SesionesIniciales = miEstudiante.SesionesIniciales;
                                            acciones.Add("se han establecido las sesiones iniciales con exito");
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

        [AllowAnonymous]
        [HttpPost]
        public Respuesta Registrar([FromBody] Estudiantes estudiante)
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
                miRespuesta.code = StatusCodes.Status400BadRequest;
            }
            else
            {
                if (TECDB.ExisteEstudiante(estudiante.NumeroDeControl, estudiante.Curp))
                {
                    try
                    {

                        TecAPI.Models.Tutorias.Usuarios miUsuario = TECDB.TraerDatosAlumno(estudiante.NumeroDeControl);
                        miUsuario.Email = estudiante.Usuario.Email;
                        miUsuario.Clave = estudiante.Usuario.Clave;
                        miUsuario.Tipo = "E";
                        //estudiante.CarreraId = TECDB.MostrarCarreraId(estudiante.NumeroDeControl);
                        estudiante.Usuario = miUsuario;
                        using (TUTORIASContext db = new TUTORIASContext())
                        {
                            try
                            {
                                estudiante.CarreraId = db.Carreras
                                    .Where(w => w.Carcve == short.Parse(TECDB.TraerCarrera(estudiante.NumeroDeControl))).First().Id;
                                db.Estudiantes.Add(estudiante);
                                db.SaveChanges();
                                miRespuesta.mensaje = "Se ha insertado correctamente";
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.data = mostrarAlumno(estudiante.NumeroDeControl).data;
                            }
                            catch (Exception ex)
                            {
                                miRespuesta.mensaje = $"{ex.ToString()}";
                                miRespuesta.code = StatusCodes.Status500InternalServerError;
                            }
                        }



                    }
                    catch
                    {
                        miRespuesta.mensaje = "error interno";
                        miRespuesta.code = 500;
                    }
                }
                else
                {
                    List<string> errores = new List<string>();
                    errores.Add("el curp no coinside con el numero de control");
                    miRespuesta.data = errores;
                    miRespuesta.mensaje = "datos invalidos";
                    miRespuesta.code = StatusCodes.Status400BadRequest;
                }
            }

            return miRespuesta;


            /*
            Respuesta miRespuesta = new Respuesta();


            List<string> errores = new List<string>();
            if ((String.IsNullOrEmpty(estudiante.correo)))
            {
                errores.Add("no se ha recibido el correo");
            }
            if ((String.IsNullOrEmpty(estudiante.numeroDeControl)))
            {
                errores.Add("no se ha recibido el numero de control");
            }
            if ((String.IsNullOrEmpty(estudiante.clave)))
            {
                errores.Add("no se ha recibido la clave");
            }
            if ((String.IsNullOrEmpty(estudiante.curp)))
            {
                errores.Add("no se ha recibido el curp");
            }
            if (errores.Count > 0)
            {
                miRespuesta.mensaje = "no se han recibido ciertos datos";
                miRespuesta.data = errores;
                miRespuesta.code = 400;
                return miRespuesta;
            }
            else
            {
                try
                {
                    using (TUTORIASContext db = new TUTORIASContext())
                    {

                        if (!Nucleo.ComprobarFormatoEmail(estudiante.correo))
                        {
                            miRespuesta.code = 400;
                            miRespuesta.mensaje = "el correo electronico no es valido";
                        }
                        else
                        {
                            var result = db.Usuarios.Where(p => p.Email == estudiante.correo);
                            if (result.Count() > 0)
                            {
                                miRespuesta.code = 409;
                                miRespuesta.mensaje = "ya existe un usuario con ese correo, intenta con otro";
                            }
                            else
                            {
                                result = null;
                                var result2 = db.Estudiantes.Where(p => p.NumeroDeControl == estudiante.numeroDeControl);
                                if (result2.Count() > 0)
                                {
                                    miRespuesta.code = 400;
                                    miRespuesta.mensaje = "el número de control ya existe";
                                }
                                else
                                {
                                    if (estudiante.clave.Length <= 5)
                                    {
                                        miRespuesta.code = 400;
                                        miRespuesta.mensaje = "tu clave debe tener un mínimo de 6 carácteres";
                                    }
                                    else
                                    {
                                        if (estudiante.numeroDeControl.Length >= 8)
                                        {
                                            //Comprobar que exista el numero de control
                                            //Comprobar el curp con el numero de control


                                            if (estudiante.curp == "123")
                                            {
                                                try
                                                {

                                                    Usuarios nuevo = new Usuarios()
                                                    {
                                                        Nombre = "Nombre equis",
                                                        ApellidoMaterno = "Apellido Materno equis",
                                                        ApellidoPaterno = "Apellido paterno equis bro",
                                                        Genero = "h",
                                                        Email = estudiante.correo,
                                                        Tipo = "e",
                                                        Clave = estudiante.clave,
                                                        Estudiantes = new Estudiantes()
                                                        {
                                                            CarreraId = 5,
                                                            NumeroDeControl = estudiante.numeroDeControl
                                                        }
                                                    };
                                                    db.Usuarios.Add(nuevo);
                                                    db.SaveChanges();

                                                    var result3 = db.Estudiantes
                                                        .Include(p => p.Grupo)
                                                        .Include(p => p.Usuario)
                                                        .Include(p => p.EstudiantesCanalizaciones)
                                                        .Where(r => r.NumeroDeControl == estudiante.numeroDeControl)
                                                        .Select(s => new REstudiante(
                                                            s.Usuario.Nombre,
                                                            s.Usuario.ApellidoMaterno,
                                                            s.Usuario.ApellidoPaterno,
                                                            s.NumeroDeControl,
                                                            s.Usuario.Genero,
                                                            s.Usuario.Email,
                                                            new RGrupo(
                                                                s.Grupo.Salon,
                                                                new RPersonal(
                                                                s.Grupo.Personal.Usuario.Nombre,
                                                                s.Grupo.Personal.Usuario.ApellidoMaterno,
                                                                s.Grupo.Personal.Usuario.ApellidoPaterno)
                                                            )
                                                        ));



                                                    miRespuesta.code = 200;
                                                    miRespuesta.mensaje = "Se ha creado tu usuario de manera correcta";
                                                    miRespuesta.data = result3.First();
                                                }
                                                catch
                                                {
                                                    miRespuesta.code = 500;
                                                    miRespuesta.mensaje = "error al guardar el estudiante";
                                                }
                                            }
                                            else
                                            {
                                                miRespuesta.code = 400;
                                                miRespuesta.mensaje = "tu CURP no es valido";
                                            }
                                        }
                                        else
                                        {
                                            miRespuesta.code = 400;
                                            miRespuesta.mensaje = "tu número de control no es valido";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    miRespuesta.code = 500;
                    miRespuesta.mensaje = ex.ToString();
                }
            }
            */
        }


        
        
    }
}