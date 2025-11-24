using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetalleDevolucionController : ControllerBase
    {
        private readonly IDetalleDevolucionService _service;

        public DetalleDevolucionController(IDetalleDevolucionService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DetalleDevolucionDto dto)
        {
            var detalle = new DetalleDevolucion
            {
                Devolucion_ID = dto.Devolucion_ID,
                DetalleVenta_ID = dto.DetalleVenta_ID,
                Producto_ID = dto.Producto_ID,
                Cantidad = dto.Cantidad,
                PrecioUnitario = dto.PrecioUnitario,
                IVADevuelto = dto.IVADevuelto,
                SubtotalDevuelto = dto.SubtotalDevuelto,
                EstadoProducto = dto.EstadoProducto
            };

            var result = await _service.AddAsync(detalle);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{devolucionId}")]
        public async Task<IActionResult> GetByDevolucionID(int devolucionId)
        {
            var result = await _service.GetByDevolucionIDAsync(devolucionId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Detalle eliminado correctamente." });
        }
    }
}