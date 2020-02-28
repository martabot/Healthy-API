using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Healthy2020.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Healthy2020.Api
{
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuarioController : Controller
    {
        private readonly DataContext contexto;
        private readonly IConfiguration config;
        public static int soyYo;

        public UsuarioController(DataContext contexto, IConfiguration config)
        {
            this.contexto = contexto;
            this.config = config;
        }

        // GET: api/<controller>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(Usuario entidad)
        {
            try
            {
                string hashed = convertirPass(entidad.Password);
                var usuario = contexto.Usuario.FirstOrDefault(x => x.Mail == entidad.Mail);
                if (usuario == null || !usuario.Password.Equals(hashed))
                {
                    return BadRequest("Datos invalidos");
                }
                else
                {
                    var key = new SymmetricSecurityKey(System.Text.Encoding.ASCII.GetBytes(config["TokenAuthentication:SecretKey"]));
                    var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, usuario.Mail),
                        new Claim(ClaimTypes.Role, usuario.Rol),
                    };

                    var token = new JwtSecurityToken(
                        issuer: config["TokenAuthentication:Issuer"],
                        audience: config["TokenAuthentication:Audience"],
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(120),
                        signingCredentials: credenciales
                    );
                    soyYo = usuario.Id;
                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var usuario = User.Identity.Name;

                return Ok(contexto.Usuario.FirstOrDefault(x => x.Mail == usuario));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("ValidarMail")]
        public async Task<IActionResult> ValidarMail(String mail)
        {
            try
            {
                return Ok(contexto.Usuario.Single(x=>x.Mail==mail));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpGet]
        [Route("Administradores")]
        [Authorize(Policy = "Administrador")]
        public async Task<IActionResult> Admins()
        {
            try
            {
                return Ok(contexto.Usuario);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("Coordinadores")]
        [Authorize(Policy = "Coordinador")]
        public async Task<IActionResult> Coords()
        {
            try
            {
                return Ok(contexto.Usuario.Count());
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                return Ok(contexto.Usuario.SingleOrDefault(x => x.Id == id));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        [Authorize(Policy ="Administrador")]
        public async Task<IActionResult> Post([FromBody] Usuario entidad)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    entidad.Password = convertirPass(entidad.Password);
                    entidad.AdminUltMod = contexto.Usuario.FirstOrDefault(x => x.Mail == User.Identity.Name);
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    entidad.FumaUltMod = DateTime.Now.ToString();
                    entidad.EstadoCuenta = 1;
                    entidad.Conducta = 10;
                    contexto.Usuario.Add(entidad);
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
        public async Task<IActionResult> Put(int id, Usuario entidad)
        {
            try
            {
                if (ModelState.IsValid && contexto.Usuario.AsNoTracking().SingleOrDefault(x => x.Id == id) != null)
                {
                    entidad.Id = id;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    entidad.EstadoCuenta = 1;
                    contexto.Usuario.Update(entidad);
                    contexto.SaveChanges();
                    return Ok(entidad);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("CambiarPass/{id}")]
        public async Task<IActionResult> CambiarPass(int id,string pass1, string pass2)
        {
            try
            {
                var entidad = contexto.Usuario.AsNoTracking().Single(x => x.Id == id);

                if (convertirPass(pass1).Equals(entidad.Password))
                {
                    entidad.Password = convertirPass(pass2);
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    contexto.Usuario.Update(entidad);
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

        public string convertirPass(string pass)
        {
            string nueva= Convert.ToBase64String(KeyDerivation.Pbkdf2(
                                                    password: pass,
                                                    salt: System.Text.Encoding.ASCII.GetBytes(config["salt"]),
                                                    prf: KeyDerivationPrf.HMACSHA1,
                                                    iterationCount: 1000,
                                                    numBytesRequested: 256 / 8));
            return nueva;
        }


        [HttpDelete("{id}")]
        [Authorize(Policy ="Administrador")]
        public async Task<IActionResult> Delete(int id,int estado)
        {
            try
            {
                var entidad = contexto.Usuario.FirstOrDefault(e => e.Id == id);
                if (entidad != null)
                {
                    entidad.AdminUltModId = contexto.Usuario.FirstOrDefault(x=>x.Mail==User.Identity.Name).Id;
                    entidad.FechaUltMod = DateTime.Now.ToString();
                    entidad.EstadoCuenta = estado;
                    contexto.Usuario.Update(entidad);
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