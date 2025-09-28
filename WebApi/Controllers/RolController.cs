using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class RolController : ControllerBase
    {
        private readonly IRolService _rolService;

        public RolController(IRolService rolService)
        {
            _rolService = rolService;
        }

        // activar una vez agg el rol de propietario
        //[Authorize(Roles = "Propietario")]
        [HttpPost("CrearRol")]
        public async Task<IActionResult> CrearRol([FromBody] CrearRolDto request)
        {
            try
            {
                var rol = await _rolService.CrearRolAsync(request.NombreRol, request.Descripcion);
                return Ok(new { mensaje = "Rol creado correctamente", rol });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error interno del servidor", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario")]
        [HttpGet("ObtenerRoles")]
        public async Task<IActionResult> ObtenerRoles()
        {
            try
            {
                var roles = await _rolService.ObtenerTodosLosRolesAsync();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener los roles", detalle = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario")]
        [HttpDelete("EliminarRol")]
        public async Task<IActionResult> EliminarRol([FromBody] EliminarRolDto request)
        {
            try
            {
                var eliminado = await _rolService.EliminarRolAsync(request.NombreRol);
                if (eliminado)
                {
                    return Ok(new { mensaje = "Rol eliminado correctamente" });
                }
                else
                {
                    return NotFound(new { mensaje = "Rol no encontrado" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al eliminar el rol", detalle = ex.Message });
            }
        }
    }
}