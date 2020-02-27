using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Healthy2020.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Healthy2020.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ActividadController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public ActividadController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        [HttpGet]
        [Route("Admin")]
        [Authorize(Policy ="Administrador")]
        public async Task<IActionResult> Get()
        {
            try
            {
                return Ok(contexto.Actividad);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet(Name = "Get usuario")]
        [Route("Usuario/{tipo}")]
        [Authorize(Policy ="Usuario")]
        public async Task<IActionResult> Get(string tipo)
        {
            try
            {
                    var lista = contexto.Participante.Where(x => x.Usuario.Id == UsuarioController.soyYo && x.Estado == 1);
                    var ids = lista.Select(x => x.Actividad.Id).ToArray();

                if (tipo.Equals("mias"))
                {

                    return Ok(contexto.Actividad.Where(x => ids.Contains(x.Id) && x.Estado == 1));
                }
                else
                {

                    return Ok(contexto.Actividad.Where(x => !ids.Contains(x.Id) && x.Estado == 1));

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Actividades para coordinador
        [HttpGet(Name = "Get coordinador")]
        [Route("Coordinador/{tipo}")]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Coordinador(string tipo)
        {
            try
            {
                if (tipo.Equals("mias"))
                {
                    return Ok(contexto.Actividad.Where(x => x.Coordinador.Id == UsuarioController.soyYo));
                }
                else
                {

                    return Ok(contexto.Actividad.Where(x => x.Coordinador.Id != UsuarioController.soyYo));

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Authorize(Policy ="Coordinador")]
        public async Task<IActionResult> Post([FromBody] Actividad entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.Coordinador = contexto.Usuario.Single(e => e.Mail == User.Identity.Name);
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    entidad.Estado = 1;
                    contexto.Actividad.Add(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.Id }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Put(int id, [FromBody] Actividad entidad)
        {
            try
            {
                if (ModelState.IsValid && contexto.Actividad.AsNoTracking().SingleOrDefault(x => x.Id == id) != null)
                {
                    entidad.Id = id;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    contexto.Actividad.Update(entidad);
                    contexto.SaveChanges();
                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Delete(int id,int estado)
        {
            try
            {
                var entidad = contexto.Actividad.AsNoTracking().FirstOrDefault(e => e.Id == id);
                if (entidad != null)
                {
                    entidad.Estado = estado;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    contexto.Actividad.Update(entidad);
                    contexto.SaveChanges();
                    return Ok();
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}