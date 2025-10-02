using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompraController : ControllerBase
    {
        private readonly ICompraService _compraService;

        public CompraController(ICompraService compraService)
        {
            _compraService = compraService;
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var compras = await _compraService.GetAllAsync();
                return Ok(compras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las compras", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID proporcionado no es válido." });

            try
            {
                var compra = await _compraService.GetByIDAsync(id);
                if (compra == null)
                    return NotFound(new { message = "Compra no encontrada" });

                return Ok(compra);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error interno al procesar la solicitud", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("Usuario/{idUsuario:int}")]
        public async Task<IActionResult> GetByUsuario(int idUsuario)
        {
            if (idUsuario <= 0)
                return BadRequest(new { message = "El ID de usuario proporcionado no es válido." });

            try
            {
                var compras = await _compraService.GetByUsuarioAsync(idUsuario);
                return Ok(compras);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener las compras del usuario", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> RegisterCompra([FromBody] ComprasDto compraDto)
        {
            var userIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
                return Unauthorized(new { message = "No se pudo obtener el ID del usuario desde el token." });

            var compra = new Compras
            {
                Usuario_ID = userId,
                Proveedor_ID = compraDto.Proveedor_ID,
                FechaRegistro = DateTime.Now,
                DetallesCompra = compraDto.DetallesCompra.Select(detalle => new DetalleCompra
                {
                    Producto_ID = detalle.Producto_ID,
                    CantidadUnitaria = detalle.CantidadUnitaria,
                    MontoUnitario = detalle.MontoUnitario,
                    IVA = detalle.IVA,
                    Total = (detalle.MontoUnitario * detalle.CantidadUnitaria) + detalle.IVA
                }).ToList()
            };

            compra.CantidadTotal = compra.DetallesCompra.Sum(d => d.CantidadUnitaria);
            compra.SubTotal = compra.DetallesCompra.Sum(d => d.MontoUnitario * d.CantidadUnitaria);
            compra.IVATotal = compra.DetallesCompra.Sum(d => d.IVA);
            compra.Total = compra.SubTotal + compra.IVATotal;
            compra.MontoTotal = compra.SubTotal;

            try
            {
                var compraAgregada = await _compraService.AddCompraConDetallesAsync(compra, userId);
                return Ok(compraAgregada);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Compras compra)
        {
            if (id <= 0 || compra.Compra_ID != id)
                return BadRequest(new { message = "ID de compra inválido" });

            try
            {
                var compraExistente = await _compraService.GetByIDAsync(id);
                if (compraExistente == null)
                    return NotFound(new { message = "Compra no encontrada" });

                var compraActualizada = await _compraService.UpdateAsync(compra);
                return Ok(compraActualizada);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar la compra", error = ex.Message });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID proporcionado no es válido." });

            try
            {
                await _compraService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar la compra", error = ex.Message });
            }
        }
    }
}