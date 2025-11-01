using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;

        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }


        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var productos = _productoService.GetAll();

            if (!productos.Any())
            {
                return Ok(new { message = "No hay productos registrados actualmente." });
            }

            return Ok(productos);
        }


        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var producto = _productoService.GetById(id);
            if (producto == null)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }
            return Ok(producto);
        }

        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("{nombreProducto}")]
        public IActionResult GetByNombre(string nombreProducto)
        {
            var productos = _productoService.GetByNombre(nombreProducto);

            // Verificar si la lista está vacía
            if (productos == null || productos.Count == 0)
            {
                return NotFound(new { message = "Producto no encontrado." });
            }

            return Ok(productos);
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Producto producto)
        {
            try
            {
                var newProducto = await _productoService.Registrar(producto);
                return Ok(new { message = "Producto agregado correctamente.", producto = newProducto });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Producto producto)
        {
            try
            {
                var existingProducto = _productoService.GetById(id);
                if (existingProducto == null)
                {
                    return NotFound(new { message = "Producto no encontrado para actualizar." });
                }

                _productoService.Update(id, producto);
                return Ok(new { message = "Producto modificado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var producto = _productoService.GetById(id);
                if (producto == null)
                {
                    return NotFound(new { message = "Producto no encontrado para eliminar." });
                }

                _productoService.Delete(id);
                return Ok(new { message = "Producto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}