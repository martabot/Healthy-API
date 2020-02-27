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
    public class InscripcionEventoController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public InscripcionEventoController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var lista = contexto.InscripcionEvento.Where(x => x.EventoId == id && x.Estado == 1);
                var ids = lista.Select(x => x.Usuario.Id).ToArray();

                return Ok(contexto.Usuario.Where(x => ids.Contains(x.Id)));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get(int usuario, int evento)
        {
            try
            {
                return Ok(contexto.InscripcionEvento.LastOrDefault(x => x.UsuarioId == usuario && x.EventoId == evento && x.Estado == 1).Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Usuario")]
        public async Task<IActionResult> Post(InscripcionEvento entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.UsuarioId = UsuarioController.soyYo;
                    entidad.Evento = contexto.Evento.Single(e => e.Id == entidad.EventoId);
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    entidad.Estado = 1;
                    contexto.InscripcionEvento.Add(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.Evento.Id }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, int estado)
        {
            try
            {
                var entidad = contexto.InscripcionEvento.AsNoTracking().FirstOrDefault(e => e.Id == id);
                if (entidad != null)
                {
                    entidad.Estado = estado;
                    contexto.InscripcionEvento.Update(entidad);
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