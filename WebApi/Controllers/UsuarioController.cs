using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [Authorize(Roles = "Administrador, Propietario")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var usuarios = _usuarioService.GetAll();
            return Ok(usuarios);
        }

        [Authorize(Roles = "Administrador, Propietario")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var usuario = _usuarioService.GetById(id);
            if (usuario == null)
                return NotFound(new { message = "Usuario no encontrado." });

            return Ok(usuario);
        }


        [HttpPost("Registro")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto usuarios)
        {
            var usuario = new Usuario
            {
                NombreUsuario = usuarios.NombreUsuario,
            };

            try
            {
                await _usuarioService.Registrar(usuario, usuarios.Contraseña);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("Autenticar")]
        public async Task<IActionResult> Authenticate([FromBody] UsuarioDto Usuario)
        {
            var user = await _usuarioService.Autenticar(Usuario.NombreUsuario, Usuario.Contraseña);

            if (user == null)
                return BadRequest(new { message = "Datos Incorrectos" });

            var token = _usuarioService.GenerateJwtToken(user);
            return Ok(new { Token = token });
        }
    }
}