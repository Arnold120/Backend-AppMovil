using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleCompraController : ControllerBase
    {
        private readonly IDetalleCompraService _detalleCompraService;

        public DetalleCompraController(IDetalleCompraService detalleCompraService)
        {
            _detalleCompraService = detalleCompraService;
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var detallesCompras = await _detalleCompraService.GetAllAsync();
                return Ok(detallesCompras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los detalles de compra", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("{idDetalleCompra:int}")]
        public async Task<IActionResult> GetById(int idDetalleCompra)
        {
            try
            {
                var detalleCompra = await _detalleCompraService.GetByIDAsync(idDetalleCompra);
                if (detalleCompra == null)
                    return NotFound(new { message = "Detalle de compra no encontrado." });

                return Ok(detalleCompra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el detalle de compra", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("Compra/{idCompra:int}")]
        public async Task<IActionResult> GetByCompra(int idCompra)
        {
            try
            {
                var detalles = await _detalleCompraService.GetByCompraIDAsync(idCompra);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los detalles por compra", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DetalleCompra detalleCompra)
        {
            try
            {
                var nuevoDetalleCompra = await _detalleCompraService.AddAsync(detalleCompra);
                return CreatedAtAction(nameof(GetById), new { idDetalleCompra = nuevoDetalleCompra.DetalleCompra_ID }, nuevoDetalleCompra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agregar el detalle de compra", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{idDetalleCompra:int}")]
        public async Task<IActionResult> Update(int idDetalleCompra, [FromBody] DetalleCompra detalleCompra)
        {
            if (idDetalleCompra != detalleCompra.DetalleCompra_ID)
                return BadRequest(new { message = "El ID del detalle de compra no coincide." });

            try
            {
                await _detalleCompraService.UpdateAsync(detalleCompra);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el detalle de compra", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{idDetalleCompra:int}")]
        public async Task<IActionResult> Delete(int idDetalleCompra)
        {
            try
            {
                await _detalleCompraService.DeleteAsync(idDetalleCompra);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el detalle de compra", error = ex.Message });
            }
        }
    }
}