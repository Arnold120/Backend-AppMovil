using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly IDetalleFacturaService _detalleFacturaService;

        public DetalleFacturaController(IDetalleFacturaService detalleFacturaService)
        {
            _detalleFacturaService = detalleFacturaService;
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var detalles = await _detalleFacturaService.GetAllAsync();
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los detalles de factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("{idDetalleFactura:int}")]
        public async Task<IActionResult> GetById(int idDetalleFactura)
        {
            try
            {
                var detalle = await _detalleFacturaService.GetByIDAsync(idDetalleFactura);
                if (detalle == null)
                    return NotFound(new { message = "Detalle de factura no encontrado." });

                return Ok(detalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el detalle de factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("factura/{idFactura:int}")]
        public async Task<IActionResult> GetByFactura(int idFactura)
        {
            try
            {
                var detalles = await _detalleFacturaService.GetByFacturaIDAsync(idFactura);
                return Ok(detalles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los detalles por factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] DetalleFactura detalleFactura)
        {
            try
            {
                var nuevoDetalle = await _detalleFacturaService.AddAsync(detalleFactura);
                return CreatedAtAction(nameof(GetById), new { idDetalleFactura = nuevoDetalle.DetalleFactura_ID }, nuevoDetalle);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agregar el detalle de factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{idDetalleFactura:int}")]
        public async Task<IActionResult> Update(int idDetalleFactura, [FromBody] DetalleFactura detalleFactura)
        {
            if (idDetalleFactura != detalleFactura.DetalleFactura_ID)
                return BadRequest(new { message = "El ID del detalle de factura no coincide." });

            try
            {
                await _detalleFacturaService.UpdateAsync(detalleFactura);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el detalle de factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{idDetalleFactura:int}")]
        public async Task<IActionResult> Delete(int idDetalleFactura)
        {
            try
            {
                await _detalleFacturaService.DeleteAsync(idDetalleFactura);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el detalle de factura", error = ex.Message });
            }
        }
    }
}