using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IRolService
    {
        Task<Rol> CrearRolAsync(string nombreRol, string descripcion);

        Task<List<Rol>> ObtenerTodosLosRolesAsync();

        Task<bool> EliminarRolAsync(string nombreRol);
    }
}