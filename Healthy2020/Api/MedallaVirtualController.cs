using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Healthy2020.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Healthy2020.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MedallaVirtualController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;

        public MedallaVirtualController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        //Obtiene todas las medallas del usuario con el ID
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(contexto.MedallaVirtual.SingleOrDefault(x => x.UsuarioId == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        //Crea una nueva medalla
        [HttpPost]
        public async Task<IActionResult> Post(MedallaVirtual entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    entidad.Usuario = contexto.Usuario.Single(e => e.Mail == User.Identity.Name);
                    contexto.MedallaVirtual.Add(entidad);
                    contexto.SaveChanges();
                    return CreatedAtAction(nameof(Get), new { id = entidad.UsuarioId }, entidad);
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