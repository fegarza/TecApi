﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecAPI.Models.Request;
using TecAPI.Models.Tutorias;

namespace TecAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AtencionesController : ControllerBase
    {

        [AllowAnonymous]
        [HttpGet]
        public Respuesta MostrarTodos()
        {
            Respuesta miRespuesta = new Respuesta();
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {

                    var result = db.Atenciones
                         .Select(s => new
                         {
                             id = s.Id,
                             areaId = s.AreaId,
                             titulo = s.Titulo
                         })
                         .ToList();
                    if (result.Count > 0)
                    {
                        miRespuesta.data = result;
                        miRespuesta.code = StatusCodes.Status200OK;
                        miRespuesta.mensaje = "exito";
                    }
                    else
                    {
                        miRespuesta.code = StatusCodes.Status500InternalServerError;
                        miRespuesta.mensaje = "No hay atenciones registrados";
                    }
                }
            }
            catch (Exception ex)
            {
                miRespuesta.code = StatusCodes.Status500InternalServerError;
                miRespuesta.mensaje = "error interno";
            }

            return miRespuesta;
        }

    }

}