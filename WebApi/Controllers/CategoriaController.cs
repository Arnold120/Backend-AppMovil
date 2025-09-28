using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;


namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriaController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriaController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var categorias = _categoriaService.GetAll();

            if (categorias == null || !categorias.Any())
            {
                return NotFound(new { message = "No se encontraron categorias." });
            }

            return Ok(categorias);
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public IActionResult Create([FromBody] Categorias categoria)
        {
            if (categoria == null)
            {
                return BadRequest(new { message = "La categor�a no puede ser nula." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Los datos proporcionados no son v�lidos." });
            }

            try
            {
                var createdCategoria = _categoriaService.Add(categoria);
                return CreatedAtAction(nameof(GetByID), new { id = createdCategoria.Categoria_ID }, createdCategoria);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        ////[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID proporcionado no es v�lido. Debe ser mayor que 0." });
            }

            try
            {
                var categoria = _categoriaService.GetByID(id);
                if (categoria == null)
                {
                    return NotFound(new { message = "Categor�a no encontrada." });
                }
                return Ok(categoria);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor.", error = ex.Message });
            }
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Categorias categoria)
        {
            if (id <= 0 || categoria == null || id != categoria.Categoria_ID)
            {
                return BadRequest(new { message = "El ID es inv�lido o los datos de la categor�a no coinciden." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Los datos proporcionados no son v�lidos." });
            }

            try
            {
                var existingCategoria = _categoriaService.GetByID(id);
                if (existingCategoria == null)
                {
                    return NotFound(new { message = "Categor�a no encontrada." });
                }

                _categoriaService.Update(categoria);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID proporcionado no es v�lido. Debe ser mayor que 0." });
            }

            try
            {
                var existingCategoria = _categoriaService.GetByID(id);
                if (existingCategoria == null)
                {
                    return NotFound(new { message = "Categor�a no encontrada." });
                }

                _categoriaService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }
    }
}