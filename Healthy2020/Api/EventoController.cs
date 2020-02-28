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
    public class EventoController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public EventoController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        [HttpGet]
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Get()
        {
            try
            {

                return Ok(contexto.Evento);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet] 
        [Route("prueba")]
        public async Task<IActionResult> Getdeprueba(int i,string o,int r)
        {
            try
            {

                return Ok(contexto.Participante);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [Route("Usuario/{tipo}")]
        [Authorize(Policy = "Usuario")]
        public async Task<IActionResult> Get(string tipo)
        {
            try
            {
                var lista = contexto.InscripcionEvento.Where(x => x.Usuario.Id == UsuarioController.soyYo&&x.Estado==1);
                var estado1 = lista.Select(x => x.EventoId).ToArray();

                var lista3 = contexto.InscripcionEvento.Where(x => x.Usuario.Id == UsuarioController.soyYo && x.Estado == 0);
                var estado0 = lista3.Select(x => x.EventoId).ToArray();

                var lista2 = contexto.Participante.Include(a=>a.Actividad).Where(x => x.UsuarioId == UsuarioController.soyYo && x.Estado == 1 && x.Actividad.Estado == 1);
                var participa = lista2.Select(x => x.ActividadId).ToArray();

                var aux4 = contexto.Participante.Include(a => a.Actividad).Where(x => x.UsuarioId == UsuarioController.soyYo && (x.Estado == 1 || x.Actividad.Estado == 1));
                var noparticipa = aux4.Select(x => x.ActividadId).ToArray();


                if (tipo.Equals("mias"))
                {
                    return Ok(contexto.Evento.Where(x => estado1.Contains(x.Id) && x.Estado == 1 && participa.Contains(x.ActividadId)));
                }
                else
                {
                    return Ok(contexto.Evento.Where(x => (!estado1.Contains(x.Id) && estado0.Contains(x.Id) && x.Estado == 1 && participa.Contains(x.ActividadId)) || (!estado1.Contains(x.Id) &&x.Estado == 1 && participa.Contains(x.ActividadId))));

                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Eventos para coordinador
        [Route("Coordinador/{tipo}")]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Coordinador(String tipo)
        {
            try
            {
            
                if (tipo == "mias")
                {
                    return Ok(contexto.Evento.Where(x => x.Actividad.Coordinador.Id == UsuarioController.soyYo));
                }
                else
                {
                    return Ok(contexto.Evento.Include(a=>a.Actividad));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Post([FromBody] Evento entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.Actividad = contexto.Actividad.Single(e => e.Id == entidad.ActividadId);
                    entidad.Estado = 1;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    contexto.Evento.Add(entidad);
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
        public async Task<IActionResult> Put(int id, [FromForm] Evento entidad)
        {
            try
            {
                if (ModelState.IsValid && contexto.Evento.AsNoTracking().SingleOrDefault(x => x.Id == id) != null)
                {
                    entidad.Id = id;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    contexto.Evento.Update(entidad);
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

        [HttpDelete]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var entidad = contexto.Evento.AsNoTracking().FirstOrDefault(e => e.Id == id);
                if (entidad != null)
                {
                    entidad.Estado = 0;
                    contexto.Evento.Update(entidad);
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