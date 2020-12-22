using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using System;
using TecAPI.Authorization;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Microsoft.OpenApi.Models;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using System.Reflection;

namespace TecAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo{ Title = "My API", Version = "v1" });
            });
            var filePath = Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "TecAPI.xml");
            Console.WriteLine(filePath.ToString());
           
            services.AddSwaggerGen(c =>
            {
                  c.IncludeXmlComments(filePath);
            });
           

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2).AddJsonOptions(options => {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            }); ;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("wW7pPV7ngghwWxpNLc7N8SQPhjXcPQEMtHwpfiknpJqkr5aX1kSDsNnndqWLXWkx"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer( options => 
                options.TokenValidationParameters = new TokenValidationParameters {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "dominio.com",
                    ValidAudience = "dominio.com",
                   IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                });
            
            services.AddMvc(options =>
            {
                // All endpoints need authorization using our custom authorization filter
                options.Filters.Add(new CustomAuthorizeFilter(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build()));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {




            app.UseCors(
               options =>  options.WithOrigins( new string[]{
                "http://localhost:4200",
                "http://tecapi.ddns.net",
                "http://localhost"
               }).AllowAnyMethod().AllowAnyHeader()
                         

           ) ;
            
            
           app.UseSwagger();
            
            app.UseSwaggerUI(s => {
                 s.SwaggerEndpoint("/swagger/v1/swagger.json", "MySite");
             });
           


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

 
            app.UseMvc();

            // global cors policy
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

 
        }
    }
}
