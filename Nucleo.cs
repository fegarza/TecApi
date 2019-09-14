using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TecAPI
{
     

    public static class Nucleo
    {
        


        public static Dictionary<string, string> ROLES = new Dictionary<string, string>()
      {
          {"a", "administrador"},
          {"c", "coordinador"},
          {"j", "jefe"},
          {"t", "tutor"},
          {"e", "estudiante"},
          {"c", "coordinador"},
      };

        public static bool ComprobarFormatoEmail(string sEmailAComprobar)
        {
            String sFormato;
            sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(sEmailAComprobar, sFormato))
            {
                if (Regex.Replace(sEmailAComprobar, sFormato, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



    }
}
