﻿using System;
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

        [Route("Usuario/{tipo}")]
        [Authorize(Policy = "Usuario")]
        public async Task<IActionResult> Get(string tipo)
        {
            try
            {
                int id = contexto.Usuario.Single(u => u.Mail.Equals(User.Identity.Name)).Id;
                var lista = contexto.InscripcionEvento.Where(x => x.Usuario.Id == id && x.Estado == 1);
                var ids = lista.Select(x => x.Evento.Id).ToArray();

                if (tipo.Equals("mias"))
                {

                    return Ok(contexto.Evento.Where(x => ids.Contains(x.Id) && x.Estado == 1));
                }
                else
                {

                    return Ok(contexto.Evento.Where(x => !ids.Contains(x.Id) && x.Estado == 1));

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
        public async Task<IActionResult> Coordinador()
        {
            try
            {
                int id = contexto.Usuario.Single(u => u.Mail.Equals(User.Identity.Name)).Id;
                return Ok(contexto.Evento.Where(x => x.Actividad.Coordinador.Id == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Post(Evento entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.Actividad = contexto.Actividad.Single(e => e.Id == entidad.ActividadId);
                    entidad.Estado = 1;
                    entidad.FechaUltMod = DateTime.Now;
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
                    entidad.FechaUltMod = DateTime.Now;
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

        [HttpDelete("{id}")]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Delete(int id, int estado)
        {
            try
            {
                var entidad = contexto.Evento.AsNoTracking().FirstOrDefault(e => e.Id == id);
                if (entidad != null)
                {
                    entidad.Estado = estado;
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