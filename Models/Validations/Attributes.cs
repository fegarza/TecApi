using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TecAPI.Models.Tec;

namespace TecAPI.Models.Tutorias
{

    
    public class Unico : ValidationAttribute
    {
        string tipo;
        public Unico( string tipo)
        {
            this.tipo = tipo;
        }
        
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                value = " ";
            }
            bool resp = false; 

            using (TUTORIASContext db = new TUTORIASContext())
            {
                switch (tipo)
                {
                    case "email":
                        return (db.Usuarios.Where(r => r.Email == value.ToString()).Count() == 0);
                    
                    case "numeroDeControl":
                        return (db.Estudiantes.Where(r => r.NumeroDeControl == value.ToString()).Count() == 0);
                    case "clave":
                        return (db.Personales.Where(r => r.Cve == value.ToString()).Count() == 0);
                    case "grupoPersonal":
                        return (db.Grupos.Where(r => r.PersonalId == int.Parse(value.ToString())).Count() == 0);

                }

            }

            return resp;

                
        }
    }
    public class Existe : ValidationAttribute
    {
        string tipo;
        public Existe(string tipo)
        {
            this.tipo = tipo;
        }

        public override bool IsValid(object value)
        {
            if(value == null)
            {
                value = " ";
            }

            bool resp = false;
            try
            {
                using (TUTORIASContext db = new TUTORIASContext())
                {
                    switch (tipo)
                    {

                        case "numeroDeControl":
                            resp = (TECDB.ExisteNumeroDeControl(value.ToString()));
                            break;
                        case "clave":
                            resp = (TECDB.ExisteClave(value.ToString()));
                            break;
                        case "grupoPersonal":
                            resp = (db.Personales.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;
                        case "departamento":
                            resp = (db.Departamentos.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;
                        case "atencion":
                            resp = (db.Atenciones.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;
                        case "estudiante":
                            resp = (db.Estudiantes.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;
                        case "personal":
                            resp = (db.Personales.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;
                        case "accionTutorial":
                            resp = (db.AccionesTutoriales.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;
                        case "titulo":
                            resp = (db.Titulos.Where(r => r.Id == int.Parse(value.ToString())).Count() > 0);
                            break;

                    }
                }
            }
            catch(Exception e)
            {
                System.Console.Write("ERROR -> ");
                System.Console.Write(e.ToString());
            }
            return resp;

        }


    }

    public class Numero : ValidationAttribute
    {
       
        public override bool IsValid(object value)
        {
            try
            {
                int.Parse(value.ToString());
                return true;
            }
            catch
            {
                return false;
            }
              
        }


    }
    public class NotNull : ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            if (String.IsNullOrEmpty(value.ToString()) || value == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


    }
}
