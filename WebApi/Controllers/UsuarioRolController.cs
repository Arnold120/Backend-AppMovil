using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    //[Authorize(Roles = "Propietario")]
    public class UsuarioRolController : ControllerBase
    {
        private readonly IUsuarioRolService _usuarioRolService;

        public UsuarioRolController(IUsuarioRolService usuarioRolService)
        {
            _usuarioRolService = usuarioRolService;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var lista = _usuarioRolService.GetAll();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las relaciones usuario-rol", error = ex.Message });
            }
        }

        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID proporcionado no es válido. Debe ser mayor que 0." });
            }

            try
            {
                var entidad = _usuarioRolService.GetByID(id);

                if (entidad == null)
                {
                    return NotFound(new { message = "Relación usuario-rol no encontrada" });
                }

                return Ok(entidad);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener la relación usuario-rol", error = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult Add([FromBody] UsuarioRol usuarioRol)
        {
            if (usuarioRol == null)
            {
                return BadRequest(new { message = "La relación usuario-rol es nula" });
            }

            try
            {
                var nuevaRelacion = _usuarioRolService.Add(usuarioRol);
                return CreatedAtAction(nameof(GetById), new { id = nuevaRelacion.Usuario_ID }, nuevaRelacion);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agregar la relación usuario-rol", error = ex.Message });
            }
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] UsuarioRol usuarioRol)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (usuarioRol == null || usuarioRol.Usuario_ID != id)
            {
                return BadRequest(new { message = "Datos de la relación no válidos" });
            }

            try
            {
                var existente = _usuarioRolService.GetByID(id);
                if (existente == null)
                {
                    return NotFound(new { message = "Relación usuario-rol no encontrada" });
                }

                _usuarioRolService.Update(usuarioRol);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la relación usuario-rol", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID proporcionado no es válido." });
            }

            try
            {
                var existente = _usuarioRolService.GetByID(id);
                if (existente == null)
                {
                    return NotFound(new { message = "Relación usuario-rol no encontrada" });
                }

                _usuarioRolService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la relación usuario-rol", error = ex.Message });
            }
        }
    }
}