using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Healthy2020.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Healthy2020
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                   .AddCookie(options =>
                   {
                       options.LoginPath = "/Home/Login";
                       options.LogoutPath = "/Home/Logout";
                       options.AccessDeniedPath = "/Home/Restringido";
                   })
                   .AddJwtBearer(options =>
                   {
                       options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                       {
                           ValidateIssuer = true,
                           ValidateAudience = true,
                           ValidateLifetime = true,
                           ValidateIssuerSigningKey = true,
                           ValidIssuer = configuration["TokenAuthentication:Issuer"],
                           ValidAudience = configuration["TokenAuthentication:Audience"],
                           IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(configuration["TokenAuthentication:SecretKey"])),
                       };
                   });
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Administrador", policy => policy.RequireClaim(ClaimTypes.Role, "Administrador"));
                options.AddPolicy("Coordinador", policy => policy.RequireClaim(ClaimTypes.Role, "Coordinador"));
                options.AddPolicy("Usuario", policy => policy.RequireClaim(ClaimTypes.Role, "Usuario"));
            });
            services.AddMvc();
            services.AddDbContext<DataContext>(
                options => options.UseSqlServer(
                    configuration["ConnectionStrings:DefaultConnection"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Habilitar CORS
            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
            // Uso de archivos estáticos (*.html, *.css, *.js, etc.)
            app.UseStaticFiles();
            // Permitir cookies
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                MinimumSameSitePolicy = SameSiteMode.None,
            });
            // Habilitar autenticación
            app.UseAuthentication();
            // App en ambiente de desarrollo?
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();//página amarilla de errores
            }

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "login",
                    template: "login/{**accion}",
                    defaults: new { controller = "Usuario", action = "Login" });
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Usuario}/{action=Login}/{id?}");
            });
        }
    }
}
