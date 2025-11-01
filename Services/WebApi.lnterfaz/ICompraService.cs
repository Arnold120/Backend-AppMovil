using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface ICompraService
    {
        Task<List<Compras>> GetAllAsync();
        Task<Compras> GetByIDAsync(int id);
        Task<List<Compras>> GetByUsuarioAsync(int idUsuario);
        Task DeleteAsync(int id);
        Task<Compras> AddCompraConDetallesAsync(Compras compra, int idUsuarioAutenticado);
        Task<Compras> UpdateAsync(Compras compra);
    }
}