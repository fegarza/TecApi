using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TecAPI.Models.Escolares;
using TecAPI.Models.Request;
using TecAPI.Models.Tec;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {



      /// <summary>
      /// 
      /// </summary>
        public ValuesController( )
        {
             
        }


        /// <summary>
        /// Esto solo es una prueba
        /// </summary>
        /// <returns>retorna una prueba </returns>
        [AllowAnonymous]
        [HttpGet]
        public Respuesta Get()
        {
            Respuesta miRespuesta = new Respuesta();

            miRespuesta.mensaje = "Version 2021-01-07";

            List<object> errores = new List<object>();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    var x = db.Titulos;
                }
            }
            catch(Exception ex)
            {
                errores.Add(ex);
            }
            try
            {
                using (EscolaresContext db = new EscolaresContext())
                {
                    var x = db.Actividades;
                }
            }
            catch (Exception ex)
            {
                errores.Add(ex);
            }
            errores.Add(TECDB.ComprobarConexion());
            
            
            
            miRespuesta.data = errores;
            return miRespuesta;
        }

        
    }
}
