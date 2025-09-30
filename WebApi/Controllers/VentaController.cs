using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VentaController : ControllerBase
    {
        private readonly IVentaService _ventaService;

        public VentaController(IVentaService ventaService)
        {
            _ventaService = ventaService;
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var ventas = await _ventaService.GetAllAsync();
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las ventas", error = ex.Message });
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
                var venta = await _ventaService.GetByIDAsync(id);
                if (venta == null)
                    return NotFound(new { message = "Venta no encontrada" });

                return Ok(venta);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error interno", error = ex.Message });
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
                var ventas = await _ventaService.GetByClienteAsync(idCliente);
                return Ok(ventas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener ventas del cliente", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> RegisterVenta([FromBody] VentaDto ventaDto)
        {
            var userIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { message = "No se pudo obtener el ID del usuario desde el token." });

            var venta = new Venta
            {
                Cliente_ID = ventaDto.Cliente_ID,  
                Usuario_ID = userId,               
                MontoRecibido = ventaDto.MontoRecibido,
                Descuento = ventaDto.Descuento,
                Estado = "Activo",
                FechaVenta = DateTime.Now,         
                DetallesVenta = ventaDto.DetallesVenta.Select(det => new DetalleVenta
                {
                    Producto_ID = det.Producto_ID,
                    Cantidad = det.Cantidad,
                    PrecioUnitario = det.PrecioUnitario,
                    IVA = det.IVA,
                    TipoComprobante = det.TipoComprobante,
                    SubTotal = det.PrecioUnitario * det.Cantidad,
                    Total = (det.PrecioUnitario * det.Cantidad) + det.IVA
                }).ToList()
            };

            venta.CantidadTotal = venta.DetallesVenta.Sum(d => d.Cantidad);
            venta.SubTotal = venta.DetallesVenta.Sum(d => d.SubTotal);
            venta.IVA = venta.DetallesVenta.Sum(d => d.IVA);
            venta.Total = venta.SubTotal + venta.IVA - venta.Descuento;
            venta.MontoDevuelto = venta.MontoRecibido - venta.Total;

            try
            {
                var ventaRegistrada = await _ventaService.AddVentaConDetallesAsync(venta, userId);
                return Ok(ventaRegistrada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] VentaDto ventaDto)
        {
            if (id <= 0)
                return BadRequest(new { message = "ID de venta no válido." });

            try
            {
                var ventaExistente = await _ventaService.GetByIDAsync(id);
                if (ventaExistente == null)
                    return NotFound(new { message = "Venta no encontrada." });

                var userId = int.Parse(User.FindFirstValue(ClaimTypes.Name));

                ventaExistente.Usuario_ID = userId;        
                ventaExistente.Cliente_ID = ventaDto.Cliente_ID; 
                ventaExistente.MontoRecibido = ventaDto.MontoRecibido;
                ventaExistente.Descuento = ventaDto.Descuento;

                ventaExistente.DetallesVenta = ventaDto.DetallesVenta.Select(det => new DetalleVenta
                {
                    Venta_ID = id,                       
                    Producto_ID = det.Producto_ID,       
                    Cantidad = det.Cantidad,
                    PrecioUnitario = det.PrecioUnitario,
                    IVA = det.IVA,
                    TipoComprobante = det.TipoComprobante,
                    SubTotal = det.PrecioUnitario * det.Cantidad,
                    Total = (det.PrecioUnitario * det.Cantidad) + det.IVA
                }).ToList();

                ventaExistente.CantidadTotal = ventaExistente.DetallesVenta.Sum(d => d.Cantidad);
                ventaExistente.SubTotal = ventaExistente.DetallesVenta.Sum(d => d.SubTotal);
                ventaExistente.IVA = ventaExistente.DetallesVenta.Sum(d => d.IVA);
                ventaExistente.Total = ventaExistente.SubTotal + ventaExistente.IVA - ventaExistente.Descuento;
                ventaExistente.MontoDevuelto = ventaExistente.MontoRecibido - ventaExistente.Total;

                await _ventaService.UpdateAsync(ventaExistente);
                return Ok(ventaExistente);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la venta", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID proporcionado no es válido." });

            try
            {
                await _ventaService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la venta", error = ex.Message });
            }
        }

        [HttpGet("precio-fifo/{idProducto}")]
        public async Task<IActionResult> ObtenerPrecioUnitarioFIFO(int idProducto)
        {
            try
            {
                var precio = await _ventaService.ObtenerPrecioUnitarioFIFO(idProducto);
                return Ok(precio);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}