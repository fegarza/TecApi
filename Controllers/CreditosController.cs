using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TecAPI.Models.Escolares;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreditosController : ControllerBase
    {
        [HttpGet]
        public Respuesta Index()
        {
            Respuesta miRespuesta = new Respuesta();

            try
            {
                using (EscolaresContext db = new EscolaresContext())
                {
                    var result = db.Secciones.Include(i => i.Actividades).Select(s => s).ToList();
                    miRespuesta.code = StatusCodes.Status200OK;
                    miRespuesta.data = result;
                    miRespuesta.mensaje = "Exito";
                    return miRespuesta;
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

    }
}