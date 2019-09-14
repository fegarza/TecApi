using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using TecAPI.Models.Request;
 
namespace TecAPI.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {



      
        public ValuesController( )
        {
             
        }


        // GET api/values
        [AllowAnonymous]
        [HttpGet]
        public Respuesta Get()
        {
            Respuesta miRespuesta = new Respuesta();
            miRespuesta.mensaje = "Prueba simple xd";
            return miRespuesta;
        }

        
    }
}
