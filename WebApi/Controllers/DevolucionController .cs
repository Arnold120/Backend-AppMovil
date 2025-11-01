using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DevolucionController : ControllerBase
    {
        private readonly IDevolucionService _service;

        public DevolucionController(IDevolucionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var devoluciones = await _service.GetAllAsync();
            if (!devoluciones.Any())
                return NotFound(new { message = "No se encontraron devoluciones." });

            return Ok(devoluciones);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetByID(int id)
        {
            try
            {
                var devolucion = await _service.GetByIDAsync(id);
                return Ok(devolucion);
            }
            catch
            {
                return NotFound(new { message = "Devolución no encontrada." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DevolucionDto dto)
        {
            var devolucion = new Devolucion
            {
                Venta_ID = dto.Venta_ID,
                CantidadDevuelta = dto.CantidadDevuelta,
                SubTotalDevuelto = dto.SubTotalDevuelto,
                TotalDevuelto = dto.TotalDevuelto,
                Motivo = dto.Motivo,
                TipoDevolucion = dto.TipoDevolucion
            };

            var result = await _service.AddDevolucionAsync(devolucion);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Devolucion devolucion)
        {
            if (id != devolucion.Devolucion_ID)
                return BadRequest(new { message = "El ID no coincide." });

            await _service.UpdateAsync(devolucion);
            return Ok(new { message = "Devolución actualizada con éxito." });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok(new { message = "Devolución eliminada correctamente." });
        }
    }
}