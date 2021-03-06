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
    public class ParticipanteController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public ParticipanteController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var lista = contexto.Participante.Where(x => x.ActividadId == id && x.Estado == 1);
                var ids = lista.Select(x => x.Usuario.Id).ToArray();
                var users = contexto.Usuario.Where(x => ids.Contains(x.Id));

                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("Todos")]
        [Authorize(Policy ="Coordinador")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var lista = contexto.Participante.Include(a=>a.Actividad).Where(x => x.Actividad.CoordinadorId == UsuarioController.soyYo && x.Estado == 1);
         

                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }



        [HttpGet]
        public async Task<IActionResult> Get(int usuario, int actividad)
        {
            try
            {
                return Ok(contexto.Participante.LastOrDefault(x => x.UsuarioId == usuario && x.ActividadId==actividad&&x.Estado==1).Id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy = "Usuario")]
        public async Task<IActionResult> Post(Participante entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.Usuario = contexto.Usuario.Single(e => e.Mail == User.Identity.Name);
                    entidad.Actividad = contexto.Actividad.Single(e => e.Id == entidad.ActividadId);
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    entidad.Estado = 1;
                    contexto.Participante.Add(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.Actividad.Id }, entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        [Authorize(Policy = "Usuario")]
        public async Task<IActionResult> Delete(Actividad actividad)
        {
            try
            {
                var entidad = contexto.Participante.AsNoTracking().Include(x => x.Actividad).FirstOrDefault(e => e.UsuarioId == UsuarioController.soyYo && e.ActividadId == actividad.Id && e.Estado == 1);
                if (entidad != null)
                {
                    entidad.Estado = 0;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    contexto.Participante.Update(entidad);
                    contexto.SaveChanges();
                    return Ok(entidad.Actividad);
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