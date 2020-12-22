using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TecAPI
{
     

    public static class Nucleo
    {
         public static bool EnviarCorreo(List<string> _correos, string _asunto, string _contenido)
        {
            MailMessage email = new MailMessage();
            foreach (string x in _correos)
            {
                email.To.Add(new MailAddress(x));
            }
          
            email.From = new MailAddress("fegarza@outlook.es");
            email.Subject = _asunto;
            email.Body = _contenido;
            email.IsBodyHtml = true;
            email.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.Host = "example.com";
            smtp.Port = 2525;
            smtp.EnableSsl = false;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("fegarza@outlook.es", "ok");
            try
            {
                smtp.Send(email);
                email.Dispose();
                return true;
            }
            catch
            {
                return false;
            }

         
        }

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

        public static IOrderedQueryable<TSource> OrderBy<TSource>(
     this IQueryable<TSource> query, string propertyName)
        {
            var entityType = typeof(TSource);

            //Create x=>x.PropName
            var propertyInfo = entityType.GetProperty(propertyName);
            ParameterExpression arg = Expression.Parameter(entityType, "x");
            MemberExpression property = Expression.Property(arg, propertyName);
            var selector = Expression.Lambda(property, new ParameterExpression[] { arg });

            //Get System.Linq.Queryable.OrderBy() method.
            var enumarableType = typeof(System.Linq.Queryable);
            var method = enumarableType.GetMethods()
                 .Where(m => m.Name == "OrderBy" && m.IsGenericMethodDefinition)
                 .Where(m =>
                 {
                     var parameters = m.GetParameters().ToList();
                     //Put more restriction here to ensure selecting the right overload                
                     return parameters.Count == 2;//overload that has 2 parameters
                 }).Single();
            //The linq's OrderBy<TSource, TKey> has two generic types, which provided here
            MethodInfo genericMethod = method
                 .MakeGenericMethod(entityType, propertyInfo.PropertyType);

            /*Call query.OrderBy(selector), with query and selector: x=> x.PropName
              Note that we pass the selector as Expression to the method and we don't compile it.
              By doing so EF can extract "order by" columns and generate SQL for it.*/
            var newQuery = (IOrderedQueryable<TSource>)genericMethod
                 .Invoke(genericMethod, new object[] { query, selector });
            return newQuery;
        }

    }


    public  class MasterMailServer
    {
        //Atributos
        private SmtpClient smtpClient;
        protected string senderMail { get; set; }
        protected string password { get; set; }
        protected string host { get; set; }
        protected int port { get; set; }
        protected bool ssl { get; set; }

        public MasterMailServer()
        {
            senderMail = "felipegs213@gmail.com";
            password = "72719902";
            host = "smtp.gmail.com";
            port = 587;
            ssl = true;
            initializeSmtpClient();
        }

        //Inicializar propiedades del cliente SMTP
        protected void initializeSmtpClient()
        {

            smtpClient = new SmtpClient();
            smtpClient.Credentials = new NetworkCredential(senderMail, password);
            smtpClient.Host = host;
            smtpClient.Port = port;
            smtpClient.EnableSsl = ssl;
        }
        public void sendMail(string subject, string body, List<string> recipientMail)
        {
            var mailMessage = new MailMessage();
            try
            {
                mailMessage.From = new MailAddress(senderMail);
                foreach (string mail in recipientMail)
                {
                    mailMessage.To.Add(mail);
                }
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.Priority = MailPriority.Normal;
                smtpClient.Send(mailMessage);//Enviar mensaje
            }
            catch (Exception ex) {

                Console.WriteLine("-------------------------------------------------------");
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                mailMessage.Dispose();
                smtpClient.Dispose();
            }
        }
    }
    
}
