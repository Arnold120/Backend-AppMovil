using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet]
        public IActionResult GetAll()
        {
            var clientes = _clienteService.GetAll();

            if (!clientes.Any())
            {
                return Ok(new { message = "No hay clientes registrados actualmente." });
            }

            return Ok(clientes);
        }

     //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            var cliente = _clienteService.GetById(id);
            if (cliente == null)
            {
                return NotFound(new { message = "Cliente no encontrado." });
            }
            return Ok(cliente);
        }

        //[Authorize(Roles = "Propietario, Administrador, Empleado")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] Cliente cliente)
        {
            try
            {
                var newCliente = await _clienteService.Registrar(cliente);
                return Ok(newCliente);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Cliente cliente)
        {
            try
            {
                _clienteService.Update(id, cliente);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "El cliente con el ID especificado no existe." });
            }
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _clienteService.Delete(id);
                return Ok(new { message = $"El cliente con ID {id} ha sido eliminado exitosamente." });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"El cliente con ID {id} no existe en el sistema." });
            }
        }
    }
}
