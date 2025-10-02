using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedorController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        //[Authorize(Roles = "Propietario, Administrador")]
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var proveedores = _proveedorService.GetAll();
                return Ok(proveedores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al obtener los proveedores", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpGet("{id:int}")]
        public IActionResult GetById(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID proporcionado no es válido. Debe ser mayor que 0." });

            try
            {
                var proveedor = _proveedorService.GetByID(id);

                if (proveedor == null)
                    return NotFound(new { message = "Proveedor no encontrado" });

                return Ok(proveedor);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Ocurrió un error interno al procesar la solicitud", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPost]
        public IActionResult Add([FromBody] Proveedor proveedor)
        {
            if (proveedor == null)
                return BadRequest(new { message = "El proveedor es nulo" });

            try
            {
                var nuevoProveedor = _proveedorService.Add(proveedor);
                return CreatedAtAction(nameof(GetById), new { id = nuevoProveedor.Proveedor_ID }, nuevoProveedor);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al agregar el proveedor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpPut("{id:int}")]
        public IActionResult Update(int id, [FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (proveedor == null || proveedor.Proveedor_ID != id)
                return BadRequest(new { message = "Datos del proveedor no válidos" });

            try
            {
                var existingProveedor = _proveedorService.GetByID(id);
                if (existingProveedor == null)
                    return NotFound(new { message = "Proveedor no encontrado" });

                _proveedorService.Update(proveedor);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al actualizar el proveedor", error = ex.Message });
            }
        }

        [Authorize(Roles = "Propietario, Administrador")]
        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "El ID proporcionado no es válido." });

            try
            {
                var existingProveedor = _proveedorService.GetByID(id);
                if (existingProveedor == null)
                    return NotFound(new { message = "Proveedor no encontrado" });

                _proveedorService.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al eliminar el proveedor", error = ex.Message });
            }
        }
    }
}