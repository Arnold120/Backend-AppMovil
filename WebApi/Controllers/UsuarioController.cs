using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Security.Claims;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(IUsuarioService usuarioService, ILogger<UsuarioController> logger)
        {
            _usuarioService = usuarioService;
            _logger = logger;
        }

        [Authorize(Roles = "Administrador, Propietario")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var usuarios = _usuarioService.GetAll();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error interno al obtener usuarios." });
            }
        }

        [Authorize(Roles = "Administrador, Propietario")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var usuario = _usuarioService.GetById(id);
                if (usuario == null)
                    return NotFound(new { message = "Usuario no encontrado." });

                return Ok(usuario);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario por ID");
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        [AllowAnonymous]
        [HttpPost("Registro")]
        public async Task<IActionResult> Register([FromBody] UsuarioDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Contraseña))
                return BadRequest(new { message = "Debe proporcionar nombre de usuario y contraseña." });

            var usuario = new Usuario
            {
                NombreUsuario = dto.NombreUsuario
            };

            try
            {
                await _usuarioService.Registrar(usuario, dto.Contraseña);
                return Ok(new { message = "Usuario registrado correctamente." });
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Error SQL durante el registro.");
                return StatusCode(500, new { message = "Error en la base de datos al registrar usuario." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al registrar usuario.");
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("Autenticar")]
        public async Task<IActionResult> Authenticate([FromBody] UsuarioDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.NombreUsuario) || string.IsNullOrWhiteSpace(dto.Contraseña))
                return BadRequest(new { message = "Debe proporcionar usuario y contraseña." });

            try
            {
                var user = await _usuarioService.Autenticar(dto.NombreUsuario, dto.Contraseña);

                if (user == null)
                    return Unauthorized(new { message = "Usuario o contraseña incorrectos." });

                var token = _usuarioService.GenerateJwtToken(user);

                return Ok(new
                {
                    token = token,
                    NombreUsuario = user.NombreUsuario
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al autenticar usuario.");
                return StatusCode(500, new { message = "Error al autenticar usuario." });
            }
        }


        [Authorize(Roles = "Administrador, Propietario")]
        [HttpGet("en-linea")]
        public IActionResult ObtenerUsuariosEnLinea()
        {
            try
            {
                var usuarios = _usuarioService.ObtenerUsuariosEnLinea();
                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios en línea.");
                return StatusCode(500, new { message = "Error al obtener usuarios en línea." });
            }
        }

        [Authorize(Roles = "Administrador, Propietario")]
        [HttpPost("reportar-actividad")]
        public async Task<IActionResult> ReportarActividad()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var resultado = await _usuarioService.ActualizarActividad(usuarioId);

                if (resultado)
                {
                    return Ok(new
                    {
                        message = "Actividad reportada",
                        fecha = DateTime.Now,
                        Usuario_ID = usuarioId
                    });
                }

                return StatusCode(500, new { message = "Error al reportar actividad" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al reportar actividad");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [Authorize(Roles = "Administrador,Propietario")]
        [HttpGet("estado-en-linea/{usuarioId}")]
        public IActionResult VerificarEstadoEnLinea(int usuarioId)
        {
            try
            {
                var usuario = _usuarioService.GetById(usuarioId);
                var estaEnLinea = _usuarioService.EstaEnLinea(usuario);

                return Ok(new
                {
                    usuarioId = usuario.Usuario_ID,
                    nombreUsuario = usuario.NombreUsuario,
                    estaEnLinea = estaEnLinea,
                    ultimaActividad = usuario.UltimaActividad
                });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Usuario no encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar estado en línea");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [Authorize(Roles = "Administrador,Propietario")]
        [HttpGet("estados-en-linea")]
        public IActionResult ObtenerEstadosEnLinea()
        {
            try
            {
                var estados = _usuarioService.ObtenerTodosLosEstadosEnLinea();
                return Ok(estados);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estados en línea");
                return StatusCode(500, new { message = "Error interno del servidor" });
            }
        }

        [HttpPost("CerrarSesion")]
        public async Task<IActionResult> CerrarSesion()
        {
            try
            {
                var usuarioId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var resultado = await _usuarioService.CerrarSesion(usuarioId);

                if (resultado)
                {
                    return Ok(new { message = "Sesión cerrada correctamente" });
                }

                return StatusCode(500, new { message = "Error al cerrar sesión" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión");
                return StatusCode(500, new { message = "Error interno del servidor al cerrar sesión" });
            }
        }
    }
}
