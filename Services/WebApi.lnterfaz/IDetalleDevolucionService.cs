using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IDetalleDevolucionService
    {
        Task<DetalleDevolucion> AddAsync(DetalleDevolucion detalle);
        Task<List<DetalleDevolucion>> GetAllAsync();
        Task<List<DetalleDevolucion>> GetByDevolucionIDAsync(int devolucionId);
        Task DeleteAsync(int id);
    }
}