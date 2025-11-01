using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IDevolucionService
    {
        Task<Devolucion> AddDevolucionAsync(Devolucion devolucion);
        Task<List<Devolucion>> GetAllAsync();
        Task<Devolucion> GetByIDAsync(int id);
        Task UpdateAsync(Devolucion devolucion);
        Task DeleteAsync(int id);
    }
}