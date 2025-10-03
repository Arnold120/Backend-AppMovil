using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaService _facturaService;

        public FacturaController(IFacturaService facturaService)
        {
            _facturaService = facturaService;
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var facturas = await _facturaService.GetAllAsync();
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las facturas", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID proporcionado no es válido." });

            try
            {
                var factura = await _facturaService.GetByIDAsync(id);
                if (factura == null)
                    return NotFound(new { message = "Factura no encontrada" });

                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error interno", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("venta/{idVenta:int}")]
        public async Task<IActionResult> GetByVenta(int idVenta)
        {
            if (idVenta <= 0)
                return BadRequest(new { message = "ID de venta no válido." });

            try
            {
                var factura = await _facturaService.GetByVentaIDAsync(idVenta);
                return Ok(factura);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener factura de la venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("cliente/{idCliente:int}")]
        public async Task<IActionResult> GetByCliente(int idCliente)
        {
            if (idCliente <= 0)
                return BadRequest(new { message = "ID de cliente no válido." });

            try
            {
                var facturas = await _facturaService.GetByClienteAsync(idCliente);
                return Ok(facturas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener facturas del cliente", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> CreateFactura([FromBody] FacturaDto facturaDto)
        {
            var userIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { message = "No se pudo obtener el ID del usuario desde el token." });

            var factura = new Factura
            {
                Venta_ID = facturaDto.Venta_ID,
                Cliente_ID = facturaDto.Cliente_ID,
                Serie = facturaDto.Serie,
                Correlativo = facturaDto.Correlativo,
                FechaVencimiento = facturaDto.FechaVencimiento,
                Descuento = facturaDto.Descuento,
                Moneda = facturaDto.Moneda,
                MetodoPago = facturaDto.MetodoPago,
                TipoPago = facturaDto.TipoPago,
                Estado = "Emitida",
                DetallesFactura = facturaDto.DetallesFactura.Select(det => new DetalleFactura
                {
                    DetalleVenta_ID = det.DetalleVenta_ID,
                    Producto_ID = det.Producto_ID,
                    Cantidad = det.Cantidad,
                    PrecioUnitario = det.PrecioUnitario,
                    Descuento = det.Descuento,
                    Subtotal = det.PrecioUnitario * det.Cantidad,
                    IVA = (det.PrecioUnitario * det.Cantidad) * 0.15m,
                    Total = (det.PrecioUnitario * det.Cantidad) * 1.15m - det.Descuento
                }).ToList()
            };

            factura.SubTotal = factura.DetallesFactura.Sum(d => d.Subtotal);
            factura.IVA = factura.DetallesFactura.Sum(d => d.IVA);
            factura.TotalFactura = factura.DetallesFactura.Sum(d => d.Total);

            try
            {
                var facturaCreada = await _facturaService.AddFacturaConDetallesAsync(factura, userId);
                return Ok(facturaCreada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Factura factura)
        {
            if (id != factura.Factura_ID)
                return BadRequest(new { message = "El ID de la factura no coincide." });

            try
            {
                var facturaActualizada = await _facturaService.UpdateAsync(factura);
                return Ok(facturaActualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("anular/{id:int}")]
        public async Task<IActionResult> AnularFactura(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID de factura no válido." });

            try
            {
                await _facturaService.AnularFacturaAsync(id);
                return Ok(new { message = "Factura anulada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al anular la factura", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("generar-numero")]
        public async Task<IActionResult> GenerarNumeroFactura()
        {
            try
            {
                var numeroFactura = await _facturaService.GenerarNumeroFacturaAsync();
                return Ok(new { numeroFactura });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al generar número de factura", error = ex.Message });
            }
        }
    }
}