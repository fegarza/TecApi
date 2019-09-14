using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TecAPI.Models.Request;

namespace TecAPI.Authorization
{
    public class CustomUnauthorizedResult : JsonResult
    {
        public CustomUnauthorizedResult(string message)
            : base(new Respuesta(401 ,message))
        {
            StatusCode = StatusCodes.Status401Unauthorized;
        }
    }
}