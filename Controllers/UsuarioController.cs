using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using TecAPI.Responses;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using TecAPI.Response;

namespace TecAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration configuration;


        public UsuariosController(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }





        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public Respuesta Validar(string id)
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


        [Route("type")]
        [HttpGet]
        public Respuesta TraerTipo(string token)
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


        [HttpGet]
        //[Authorize(Roles = "ADMINISTRADOR")]
        [AllowAnonymous]
        public Respuesta Mostrar()
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    respuesta.code = 200;
                    respuesta.data = db.Usuarios.ToArray();
                }
            }
            catch
            {
                respuesta.code = 500;
                respuesta.mensaje = "Error en el sistema";
            }
            return respuesta;
        }




        [AllowAnonymous]
        [HttpGet]
        [Route("Login")]
        public Respuesta Login(string email, string clave)
        {
            Respuesta respuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    //Buscamos el usuario con los datos dados
                    var result = db.Usuarios
                       .Where(r => r.Email == email && r.Clave == clave)
                       .Select(s => new RUsuario()
                       {
                           nombre = s.Nombre,
                           nombreCompleto = s.NombreCompleto,
                           apellidoMaterno = s.ApellidoMaterno,
                           apellidoPaterno = s.ApellidoPaterno,
                           correo = s.Email,
                           genero = s.Genero,
                           tipo = s.Tipo
                       });
                    if (result.Count() > 0)
                    {

                        RUsuario miUsuario = result.First();
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
                                .Select(s => new {
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
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta.code = StatusCodes.Status500InternalServerError;
                respuesta.mensaje = "Error en la base de datos: " + ex;
            }
            return respuesta;
        }





    }
}