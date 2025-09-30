using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleVentaController : ControllerBase
    {
        private readonly IDetalleVentaService _detalleVentaService;

        public DetalleVentaController(IDetalleVentaService detalleVentaService)
        {
            _detalleVentaService = detalleVentaService;
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var detalles = await _detalleVentaService.GetAllAsync();
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los detalles de venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("{idDetalleVenta:int}")]
        public async Task<IActionResult> GetById(int idDetalleVenta)
        {
            try
            {
                var detalle = await _detalleVentaService.GetByIDAsync(idDetalleVenta);
                if (detalle == null)
                    return NotFound(new { message = "Detalle de venta no encontrado." });

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el detalle de venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("Venta/{idVenta:int}")]
        public async Task<IActionResult> GetByVenta(int idVenta)
        {
            try
            {
                var detalles = await _detalleVentaService.GetByVentaIDAsync(idVenta);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los detalles por venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DetalleVenta detalleVenta)
        {
            try
            {
                var nuevoDetalle = await _detalleVentaService.AddAsync(detalleVenta);
                return CreatedAtAction(nameof(GetById), new { idDetalleVenta = nuevoDetalle.DetalleVenta_ID }, nuevoDetalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agregar el detalle de venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{idDetalleVenta:int}")]
        public async Task<IActionResult> Update(int idDetalleVenta, [FromBody] DetalleVenta detalleVenta)
        {
            if (idDetalleVenta != detalleVenta.DetalleVenta_ID) 
                return BadRequest(new { message = "El ID del detalle de venta no coincide." });

            try
            {
                await _detalleVentaService.UpdateAsync(detalleVenta);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el detalle de venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{idDetalleVenta:int}")]
        public async Task<IActionResult> Delete(int idDetalleVenta)
        {
            try
            {
                await _detalleVentaService.DeleteAsync(idDetalleVenta);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el detalle de venta", error = ex.Message });
            }
        }
    }
}