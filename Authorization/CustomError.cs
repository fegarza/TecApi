using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecAPI.Authorization
{
    public class CustomError
    {
        public string Error { get; }

        public CustomError(string message)
        {
            Error = message;
        }
    }
}
