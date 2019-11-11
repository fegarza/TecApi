using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecAPI.Models.Escolares;
using TecAPI.Models.Request;
using TecAPI.Models.Tec;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{

    /// <summary>
    /// Todo lo relacionado con los estudiantes
    /// </summary>
    [Route("api/[controller]")]
    [Authorize(Roles = "A, C, J, D, T, E")]
    [ApiController]
    public class EstudiantesController : ControllerBase
    {
        /// <summary>
        /// Mostrar creditos de un estudiante
        /// </summary>
        /// <param name="_numeroControl">Identificador del estudiante</param>
        /// <returns>cantidad de creditos</returns>
        public static int IndexCreditos(string _numeroControl)
        {
            int creditos = 0;
            using (EscolaresContext db2 = new EscolaresContext())
            {
                creditos = db2.Creditos.Where(r => r.NumeroDeControl == _numeroControl).Count();
            }
            return creditos;
        }
        public static List<Creditos> MostrarCreditos(string _numeroControl) 
        {
            List<Creditos> creditos = new List<Creditos>();
            using (EscolaresContext db2 = new EscolaresContext())
            {
                creditos = db2.Creditos.Include(i => i.Actividad).Where(r => r.NumeroDeControl == _numeroControl).ToList();
            }
            return creditos;
        }
       
        /// <summary>
        /// Mostrar todos los estudiantes
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
                            sesiones = s.EstudiantesSesiones.Count(),
                            sesionesIniciales = s.SesionesIniciales,
                            canalizaciones = s.Canalizaciones.Count(),
                            cantidadDeCreditos = IndexCreditos(s.NumeroDeControl),
                            semestre = s.Semestre,
                            FotoLink = s.FotoLink,
                            estado = s.Estado

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
                miRespuesta.mensaje = "error";
                miRespuesta.data = ex;
            }
            return miRespuesta;

        }


        /// <summary>
        /// Mostrar un alumno en especifico
        /// </summary>
        /// <param name="numeroDeControl">Identificador del alumno</param>
        /// <returns>un modelo de respuesta</returns>
        [Route("{numeroDeControl}")]
        [HttpGet]
        [AllowAnonymous]
        public Respuesta Show(string numeroDeControl)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    List<Creditos> creditos;
                    using (EscolaresContext db2 = new EscolaresContext())
                    {
                        creditos = db2.Creditos.Include(i => i.Actividad).Where(r => r.NumeroDeControl == numeroDeControl).ToList();
                    }

                    var result = db.Estudiantes
                            .Include(i => i.Grupo)
                            .Include(i => i.Grupo.Personal)
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
                                    personal = new
                                    {
                                        id = (s.Grupo.Personal != null ? s.Grupo.PersonalId : 0),
                                        departamentoId = (s.Grupo.Personal != null ? s.Grupo.Personal.DepartamentoId : 0),
                                        usuario = new
                                        {
                                            nombreCompleto = s.Grupo.Personal.Usuario.NombreCompleto
                                        }
                                    }
                                },
                                numeroDeControl = s.NumeroDeControl,
                                sesiones = s.EstudiantesSesiones.Count(),
                                sesionesIniciales = s.SesionesIniciales,
                                SesionesIndividuales = s.SesionesIndividuales.Count(),
                                canalizaciones = s.Canalizaciones.Count(),
                                cantidadDeCreditos = creditos.Count(),
                                creditos = creditos,
                                semestre = s.Semestre,
                                FotoLink = s.FotoLink,
                                estado = s.Estado
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


        /// <summary>
        
        [Route("{numeroDeControl}/datos")]
        [HttpGet]
        [AllowAnonymous]
        public Respuesta ShowDatos(string numeroDeControl)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {


                    var result = db.EstudiantesDatos.Include(i => i.Estudiante).Where(w => w.Estudiante.NumeroDeControl == numeroDeControl);
                             

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


        [Route("{numeroDeControl}/Canalizaciones")]
        [HttpGet]
        [AllowAnonymous]
        public Respuesta ShowCanalizaciones(string numeroDeControl)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {


                    var result = db.Canalizaciones
                        .Include(i => i.Atencion)
                        .Include(i => i.Personal)
                        .Include(i => i.Personal.Usuario)
                        .Where(w => w.Estudiante.NumeroDeControl == numeroDeControl);


                    if (result.Count() > 0)
                    {
                        miRespuesta.data = result.ToList();
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

        /// <summary>
        /// Actualizar un estudiante
        /// </summary>
        /// <param name="miEstudiante">Objeto del estudiante en formato JSON</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpPut]
        [Authorize(Roles = "A, C, J, D")]
        public Respuesta Update([FromBody] Estudiantes miEstudiante)
        {
            Respuesta miRespuesta = new Respuesta();
            if (miEstudiante != null)
            {
                if (!String.IsNullOrEmpty(miEstudiante.NumeroDeControl))
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
                                    if (miEstudiante.GrupoId != null)
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
                                        if (db.Carreras.Where(r => r.Id == miEstudiante.CarreraId).Count() > 0)
                                        {
                                            result.First().CarreraId = miEstudiante.CarreraId;
                                            acciones.Add("se ha cambiado la carrera con exito");
                                        }
                                        else
                                        {
                                            errores.Add("no existe ninguna carrera con el id dado");

                                        }
                                    }
                                    if (miEstudiante.SesionesIniciales != 0)
                                    {
                                        result.First().SesionesIniciales = miEstudiante.SesionesIniciales;
                                        acciones.Add("se han establecido las sesiones iniciales con exito");
                                    }
                                    if (!String.IsNullOrEmpty(miEstudiante.FotoLink))
                                    {
                                        result.First().FotoLink = miEstudiante.FotoLink;
                                        acciones.Add("se ha cambiado la foto de perfil con exito");
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


        /// <summary>
        /// Registrar un nuevo estudiante
        /// </summary>
        /// <param name="estudiante">Objeto del estudiante en formato JSON</param>
        /// <returns>un modelo de respuesta</returns>
        [AllowAnonymous]
        [HttpPost]
        public Respuesta Store([FromBody] Estudiantes estudiante)
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
                                estudiante.EstudiantesDatos = new EstudiantesDatos();
                                db.Estudiantes.Add(estudiante);
                                db.SaveChanges();
                                miRespuesta.mensaje = "Se ha insertado correctamente";
                                miRespuesta.code = StatusCodes.Status200OK;
                                miRespuesta.data = Show(estudiante.NumeroDeControl).data;
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





        [Route("datos")]
        [HttpPut]
        public Respuesta Update([FromBody] EstudiantesDatos datos)
        {
            Respuesta respuesta = new Respuesta();
            if (datos.EstudianteId != 0)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.EstudiantesDatos.Where(w => w.EstudianteId== datos.EstudianteId).First();
                        result.BecadoPor = datos.BecadoPor = datos.BecadoPor;
                        result.CalleDomicilio = datos.CalleDomicilio;
                        result.CependenciaEconomica = datos.CependenciaEconomica;
                        result.CiudadNacimiento = datos.CiudadNacimiento;
                        result.CodigoPostalDomicilio = datos.CodigoPostalDomicilio;
                        result.ColoniaDomicilio = datos.ColoniaDomicilio;
                        result.DificultadDormir = datos.DificultadDormir;
                        result.DoloresCabezaVomito = datos.DoloresCabezaVomito;
                        result.DoloresVientre = datos.DoloresVientre;
                        result.Empresa = datos.Empresa;
                        result.EstadoCivil = datos.EstadoCivil;
                        result.EstadoNacimiento = datos.EstadoNacimiento;
                        result.EstudiosMadre = datos.EstudiosMadre;
                        result.EstudiosPadre = datos.EstudiosPadre;
                        result.FatigaAgotamiento = datos.FatigaAgotamiento;
                        result.FechaModificacion = datos.FechaModificacion;
                        result.FechaNacimiento = datos.FechaNacimiento;
                        result.Horario = datos.Horario;
                        result.Incontinencia = datos.Incontinencia;
                        result.LugarTrabajoFamiliar = datos.LugarTrabajoFamiliar;
                        result.LugarTrabajoMadre = datos.LugarTrabajoMadre;
                        result.LugarTrabajoPadre = datos.LugarTrabajoPadre;
                        result.MadreVive = datos.MadreVive;
                        result.ManosPiesHinchados = datos.ManosPiesHinchados;
                        result.MiedosIntensos = datos.MiedosIntensos;
                        result.Nss = datos.Nss;
                        result.NumDomicilio = datos.NumDomicilio;
                        result.NumeroHijos = datos.NumeroHijos;
                        result.ObservacionesHigiene = datos.ObservacionesHigiene;
                        result.PadreVive = datos.PadreVive;
                        result.PerdidaEquilibrio = datos.PerdidaEquilibrio;
                        result.PerdidaVistaOido = datos.PerdidaVistaOido;
                        result.PesadillasTerroresNocturnos = datos.PesadillasTerroresNocturnos;
                        result.PesadillasTerroresNocturnosAque = datos.PesadillasTerroresNocturnosAque;
                        result.PrescripcionLenguaje = datos.PrescripcionLenguaje;
                        result.PrescripcionMotriz = datos.PrescripcionMotriz;
                        result.PrescripcionOido = datos.PrescripcionOido;
                        result.PrescripcionSistemaCirculatorio = datos.PrescripcionSistemaCirculatorio;
                        result.PrescripcionSistemaDigestivo = datos.PrescripcionSistemaDigestivo;
                        result.PrescripcionSistemaNervioso = datos.PrescripcionSistemaNervioso;
                        result.PrescripcionSistemaOseo = datos.PrescripcionSistemaOseo;
                        result.PrescripcionSistemaOtro = datos.PrescripcionSistemaOtro;
                        result.PrescripcionSistemaRespiratorio = datos.PrescripcionSistemaRespiratorio;
                        result.PrescripcionVista = datos.PrescripcionVista;
                        result.Tartamudeos = datos.Tartamudeos;
                        result.TelefonoDomicilio = datos.TelefonoDomicilio;
                        result.TelefonoMovil = datos.TelefonoMovil;
                        result.TelefonoTrabajoFamiliar = datos.TelefonoTrabajoFamiliar;
                        result.TelefonoTrabajoMadre = datos.TelefonoTrabajoMadre;
                        result.TelefonoTrabajoPadre = datos.TelefonoTrabajoPadre;
                        result.TieneBeca = datos.TieneBeca;
                        result.Trabaja = datos.Trabaja;
                        result.TratamientoPsicologicoPsiquiatrico = datos.TratamientoPsicologicoPsiquiatrico;
                        result.TratamientoPsicologicoPsiquiatricoExplicacion = datos.TratamientoPsicologicoPsiquiatricoExplicacion;

                        db.SaveChanges();
                        respuesta.code = StatusCodes.Status200OK;
                        respuesta.mensaje = "Archivo editado con exito";
                    }
                    catch (Exception e)
                    {
                        respuesta.data = e;
                        respuesta.code = StatusCodes.Status400BadRequest;
                        respuesta.mensaje = "Error al editar el archivo";
                    }
                }
            }
            else
            {
                respuesta.code = StatusCodes.Status400BadRequest;
                respuesta.mensaje = "No existe tal archivo";
            }


            return respuesta;
        }

    }
}