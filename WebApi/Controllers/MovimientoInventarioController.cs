using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientoInventarioController : ControllerBase
    {
        private readonly IMovimientoInventarioService _movimientoService;

        public MovimientoInventarioController(IMovimientoInventarioService movimientoService)
        {
            _movimientoService = movimientoService;
        }

        [Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var movimientos = _movimientoService.GetAll();

            if (!movimientos.Any())
            {
                return Ok(new { message = "No hay movimientos registrados actualmente." });
            }

            return Ok(movimientos);
        }

        [Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var movimiento = _movimientoService.GetById(id);
            if (movimiento == null)
            {
                return NotFound(new { message = "Movimiento no encontrado." });
            }

            return Ok(movimiento);
        }

        [Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("producto/{idProducto:int}")]
        public async Task<IActionResult> GetByProductoId(int idProducto)
        {
            if (idProducto <= 0)
            {
                return BadRequest(new { message = "ID de producto inválido." });
            }

            try
            {
                var movimientos = await _movimientoService.GetByProductoId(idProducto);
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener movimientos del producto.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("stock/{idProducto:int}")]
        public async Task<IActionResult> GetStockActual(int idProducto)
        {
            try
            {
                var stock = await _movimientoService.ObtenerStockActual(idProducto);
                return Ok(new { IDProducto = idProducto, StockActual = stock });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el stock actual.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] MovimientoInventario movimiento)
        {
            try
            {
                if (movimiento == null)
                {
                    return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });
                }

                var resultado = await _movimientoService.RegistrarMovimiento(movimiento);

                if (resultado == null)
                {
                    return BadRequest(new { message = "No se pudo registrar el movimiento." });
                }

                return Ok(new { message = "Movimiento registrado correctamente.", data = resultado });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al registrar el movimiento.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MovimientoInventario movimiento)
        {
            try
            {
                if (movimiento == null)
                {
                    return BadRequest(new { message = "El cuerpo de la solicitud está vacío." });
                }

                var existing = _movimientoService.GetById(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Movimiento no encontrado." });
                }

                _movimientoService.Update(id, movimiento);
                return Ok(new { message = "Movimiento actualizado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al actualizar el movimiento.", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var existing = _movimientoService.GetById(id);
                if (existing == null)
                {
                    return NotFound(new { message = "Movimiento no encontrado." });
                }

                _movimientoService.Delete(id);
                return Ok(new { message = "Movimiento eliminado" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error al eliminar el movimiento.", error = ex.Message });
            }
        }


        [Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("ultimo-precio/{idProducto:int}")]
        public async Task<IActionResult> ObtenerUltimoPrecioEntrada(int idProducto)
        {
            try
            {
                var precio = await _movimientoService.ObtenerUltimoPrecioEntradaAsync(idProducto);

                if (precio == null)
                {
                    return NotFound(new { message = "No se encontró precio de entrada para el producto." });
                }

                return Ok(new { precioUnitario = precio });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener el precio de entrada.", error = ex.Message });
            }
        }

    }
}