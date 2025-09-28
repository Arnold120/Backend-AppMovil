using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly IMarcaService _marcaService;

        public MarcaController(IMarcaService marcaService)
        {
            _marcaService = marcaService ?? throw new ArgumentNullException(nameof(marcaService));
        }

        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet]
        public ActionResult<List<Marcas>> GetAll()
        {
            try
            {
                var marcas = _marcaService.GetAll();
                if (marcas == null || marcas.Count == 0)
                {
                    return NotFound(new { message = "No hay marcas disponibles." });
                }
                return Ok(marcas);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error interno del servidor." });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("{id}")]
        public IActionResult GetByID(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { message = "El ID proporcionado no es v�lido. Debe ser mayor que 0." });
            }

            var marca = _marcaService.GetByID(id);
            if (marca == null)
            {
                return NotFound(new { message = $"No se encontr� la marca con ID {id}." });
            }
            return Ok(marca);
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public IActionResult Add([FromBody] Marcas marca)
        {
            if (marca == null)
            {
                return BadRequest(new { message = "El cuerpo de la solicitud no puede estar vac�o." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos invalidos." });
            }

            try
            {
                var newMarca = _marcaService.Add(marca);
                return CreatedAtAction(nameof(GetByID), new { id = newMarca.Marca_ID }, newMarca);
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error al agregar la marca." });
            }
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Marcas marca)
        {
            if (id <= 0 || marca == null || id != marca.Marca_ID)
            {
                return BadRequest(new { message = "El ID proporcionado es incorrecto o el cuerpo de la solicitud est� vac�o." });
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "La solicitud contiene datos inv�lidos." });
            }

            try
            {
                var existingMarca = _marcaService.GetByID(id);
                if (existingMarca == null)
                {
                    return NotFound(new { message = $"No se encontro la marca con ID {id}." });
                }

                _marcaService.Update(marca);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error al actualizar la marca." });
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
                var existingMarca = _marcaService.GetByID(id);
                if (existingMarca == null)
                {
                    return NotFound(new { message = $"No se encontr� la marca con ID {id}." });
                }

                _marcaService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {

                return StatusCode(500, new { message = "Error interno del servidor." });
            }

        }
    }
}
