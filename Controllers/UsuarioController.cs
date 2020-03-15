using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;
using Microsoft.EntityFrameworkCore;


namespace TecAPI.Controllers
{
    /// <summary>
    /// Todo lo relacionado a los usuarios
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id">Identificador del usuario</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public Respuesta Show(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            if (id != null)
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Usuarios.Include("Estudiantes").Where(r => r.Id == int.Parse(id));
                        if (result.Count() > 0)
                        {
                            miRespuesta.data = result.First();
                        }
                        else
                        {
                            miRespuesta.mensaje = "no existe ningun usuario con dicho id";
                        }
                    }
                    catch
                    {
                        miRespuesta.mensaje = "id invalido";
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
        /// Mostrar el rango
        /// </summary>
        /// <param name="token">token</param>
        /// <returns>Un modelo de respuesta</returns>
        [Route("type")]
        [HttpGet]
        public Respuesta ShowType(string token)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken tokenS = handler.ReadToken(token) as JwtSecurityToken;
                string x = tokenS.Claims.First(claim => claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Value;
                if (x == "E" || x == "C" || x == "J" || x == "T" || x == "A" || x == "D")
                {
                    miRespuesta.code = 200;
                    miRespuesta.mensaje = "se ha encontrado el rol";
                    miRespuesta.data = x;
                }
                else
                {
                    miRespuesta.code = 404;
                    miRespuesta.mensaje = "rol es invalido";

                }

            }
            catch
            {
                miRespuesta.code = 500;
                miRespuesta.mensaje = "error en el sistema";

            }
            return miRespuesta;
        }


        /// <summary>
        /// Mostrar todos los usuario
        /// </summary>
        /// <param name="cant">cantidad de registros a traer</param>
        /// <param name="pag">pagina en la que se quiere estar</param>
        /// <param name="orderBy">orden a implementar</param>
        /// <returns>un modelo de respuesta</returns>
        [HttpGet]
        //[Authorize(Roles = "ADMINISTRADOR")]
        [AllowAnonymous]
        public Respuesta Index(int cant, int pag, string orderBy)
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    miRespuesta.code = 200;

                    var result = db.Usuarios.Select(s => s.Estudiantes);
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
            catch
            {
                miRespuesta.code = 500;
                miRespuesta.mensaje = "Error en el sistema";
            }
            return miRespuesta;
        }


        /// <summary>
        /// Iniciar sesion
        /// </summary>
        /// <param name="email">Correo electronico</param>
        /// <param name="clave">Clave</param>
        /// <returns>Un toekn validado</returns>
        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public Respuesta Login([FromBody] Usuarios usr)
        {
            string email = usr.Email;
            string clave = usr.Clave;
            Respuesta respuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    //Verificar si existe
                    var busqueda = db.Usuarios.Include(i => i.Estudiantes).Include(i => i.Personales).Where(r => r.Email == email && r.Clave == clave);
                    if (busqueda.Count() > 0)
                    {
                        respuesta.code = StatusCodes.Status200OK;
                        //Tomar datos para el personal
                        if (busqueda.First().Tipo == "P")
                        {
                            string ROL = busqueda.First().Personales.Cargo;
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.Role, ROL),
                                new Claim("rol",  ROL),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            };
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wW7pPV7ngghwWxpNLc7N8SQPhjXcPQEMtHwpfiknpJqkr5aX1kSDsNnndqWLXWkx"));
                            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var expiration = DateTime.UtcNow.AddHours(5);
                            JwtSecurityToken token = new JwtSecurityToken(
                                issuer: "dominio.com",
                                audience: "dominio.com",
                                claims: claims,
                                expires: expiration,
                                signingCredentials: credentials
                            );
                            var tokenS = new JwtSecurityTokenHandler().WriteToken(token);
                            respuesta.data = busqueda.Select(s => new
                            {
                                token = tokenS.ToString(),
                                nombre = s.Nombre,
                                nombreCompleto = s.NombreCompleto,
                                apellidoMaterno = s.ApellidoMaterno,
                                apellidoPaterno = s.ApellidoPaterno,
                                correo = s.Email,
                                genero = s.Genero,
                                personal = new
                                {
                                    id = s.Personales.Id,
                                    cargo = s.Personales.Cargo,
                                    departamentoId = s.Personales.DepartamentoId
                                }
                            }).First();
                            
                        }
                        //Tomar datos para el estudiante
                        else
                        {
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.Role, "E"),
                                new Claim("rol",  "E"),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                 new Claim("nocontrol", busqueda.First().Estudiantes.NumeroDeControl)
                            };
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wW7pPV7ngghwWxpNLc7N8SQPhjXcPQEMtHwpfiknpJqkr5aX1kSDsNnndqWLXWkx"));
                            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var expiration = DateTime.UtcNow.AddHours(5);
                            JwtSecurityToken token = new JwtSecurityToken(
                                issuer: "dominio.com",
                                audience: "dominio.com",
                                claims: claims,
                                expires: expiration,
                                signingCredentials: credentials
                            );
                            var tokenS = new JwtSecurityTokenHandler().WriteToken(token);
                            respuesta.data = busqueda.Select(s => new
                            {
                                token = tokenS.ToString(),
                                nombre = s.Nombre,
                                nombreCompleto = s.NombreCompleto,
                                apellidoMaterno = s.ApellidoMaterno,
                                apellidoPaterno = s.ApellidoPaterno,
                                correo = s.Email,
                                genero = s.Genero,
                                estudiante = new
                                {
                                    id = s.Estudiantes.Id,
                                    numeroDeControl = s.Estudiantes.NumeroDeControl,
                                    grupoId = s.Estudiantes.GrupoId,
                                    semestre = s.Estudiantes.Semestre,
                                    carrera = s.Estudiantes.Carrera
                                }
                            }).First();
                        }
                    }
                    else
                    {
                        respuesta.code = StatusCodes.Status404NotFound;
                        respuesta.mensaje = "No existe una cuenta con esos datos";
                    }



                    /*
                    //Buscamos el usuario con los datos dados
                    var result = db.Usuarios
                       .Where(r => r.Email == email && r.Clave == clave)
                       .Select(s => new
                       {
                           nombre = s.Nombre,
                           nombreCompleto = s.NombreCompleto,
                           apellidoMaterno = s.ApellidoMaterno,
                           apellidoPaterno = s.ApellidoPaterno,
                           correo = s.Email,
                           genero = s.Genero,
                           estudiante = s.Estudiantes
                       });
                    if (result.Count() > 0)
                    {
                        var = result.First();
                        //Claims
                        if (miUsuario.tipo == "E")
                        {
                            var result2 = db.Usuarios
                               .Where(r => r.Email == email && r.Clave == clave)
                               .Select(s => new { identificador = s.Estudiantes.NumeroDeControl });
                            var claims = new[]
                            {
                                new Claim(ClaimTypes.Role, "E"),
                                new Claim("rol",  "E"),
                                new Claim("identificador", result2.First().identificador  ),
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            };
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wW7pPV7ngghwWxpNLc7N8SQPhjXcPQEMtHwpfiknpJqkr5aX1kSDsNnndqWLXWkx"));
                            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var expiration = DateTime.UtcNow.AddHours(5);
                            JwtSecurityToken token = new JwtSecurityToken(
                                issuer: "dominio.com",
                                audience: "dominio.com",
                                claims: claims,
                                expires: expiration,
                                signingCredentials: credentials
                            );
                            var tokenS = new JwtSecurityTokenHandler().WriteToken(token);
                            miUsuario.token = tokenS.ToString();
                            respuesta.code = StatusCodes.Status200OK;
                            respuesta.data = miUsuario;
                            respuesta.mensaje = "exito";
                        }
                        else
                        {
                            var result2 = db.Usuarios
                                .Where(r => r.Email == email && r.Clave == clave)
                                .Select(s => new
                                {
                                    cargo = s.Personales.Cargo,
                                    identificador = s.Personales.Id.ToString()
                                });
                            var claims = new[]
                            {
                                    new Claim(ClaimTypes.Role,  result2.First().cargo ),
                                    new Claim("rol",  result2.First().cargo),
                                    new Claim("identificador", result2.First().identificador  ),
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                            };
                            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wW7pPV7ngghwWxpNLc7N8SQPhjXcPQEMtHwpfiknpJqkr5aX1kSDsNnndqWLXWkx"));
                            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                            var expiration = DateTime.UtcNow.AddHours(5);
                            JwtSecurityToken token = new JwtSecurityToken(
                                issuer: "dominio.com",
                                audience: "dominio.com",
                                claims: claims,
                                expires: expiration,
                                signingCredentials: credentials
                            );
                            var tokenS = new JwtSecurityTokenHandler().WriteToken(token);
                            miUsuario.token = tokenS.ToString();
                            respuesta.code = StatusCodes.Status200OK;
                            respuesta.data = miUsuario;
                            respuesta.mensaje = "exito";
                        }
                       


                    }
                    else
                    {
                        respuesta.code = StatusCodes.Status404NotFound;
                        respuesta.mensaje = "no existe ningun usuario con estos datos";
                    } */
                }
            }
            catch (Exception ex)
            {
                respuesta.code = StatusCodes.Status500InternalServerError;
                respuesta.mensaje = "Error en la base de datos: " + ex;
            }
            return respuesta;
        }


        
        [HttpPut]
        [Authorize(Roles = "A, J, D, C")]
        public Respuesta Update([FromBody] Usuarios usuario)
        {
            Respuesta miRespuesta = new Respuesta();

            if (usuario.Id != 0)
            {
                  try
                    {
                        using (TUTORIASContext db = new TUTORIASContext())
                        {

                            var result = db.Usuarios.Where(r => r.Id == usuario.Id);
                            if (result.Count() > 0)
                            {
                                try
                                {
                                   
                                 
                                   if(!String.IsNullOrEmpty(usuario.Clave))
                                    {
                                    result.First().Clave = usuario.Clave;
                                    }
                                if (!String.IsNullOrEmpty(usuario.Email))
                                {
                                    result.First().Email = usuario.Email;
                                }

                                    db.SaveChanges();
                                miRespuesta.mensaje = "exito";
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
                        miRespuesta.data = ex;
                    }
                
            }
            else
            {
                miRespuesta.code = StatusCodes.Status400BadRequest;
                miRespuesta.mensaje = "los datos no son enviados no son correctos";
            }

            return miRespuesta;

        }
/*
        [HttpGet]
        [Route("{id}/Password")]
        [AllowAnonymous]
        public Respuesta RecuperarPassword(string id)
        {
            Respuesta miRespuesta = new Respuesta();
            MasterMailServer miMail = new MasterMailServer();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    try
                    {
                        var result = db.Usuarios.Include("Estudiantes").Where(r => r.Id == int.Parse(id));
                        if (result.Count() > 0)
                        {
                            List<string> mails = new List<string>();
                            mails.Add(result.First().Email);
                            miMail.sendMail("Recuperacion de la cuenta",$"Tu clave es: {result.First().Clave}", mails);
                            miRespuesta.mensaje = "Enviado con exito";
                        }
                        else
                        {
                            miRespuesta.mensaje = "no existe ningun usuario con dicho id";
                        }
                    }
                    catch
                    {
                        miRespuesta.mensaje = "id invalido";
                    }

                }
            }
            catch
            {
                miRespuesta.code = 500;
                miRespuesta.mensaje = "Error en el sistema";
            }
            return miRespuesta;
        }*/
    }
}