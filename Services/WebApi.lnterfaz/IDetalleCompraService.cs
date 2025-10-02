using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IDetalleCompraService
    {
        Task<DetalleCompra> AddAsync(DetalleCompra detalleCompra);
        Task<List<DetalleCompra>> GetAllAsync();
        Task<DetalleCompra?> GetByIDAsync(int idDetalleCompra);
        Task<List<DetalleCompra>> GetByCompraIDAsync(int idCompra);
        Task UpdateAsync(DetalleCompra detalleCompra);
        Task DeleteAsync(int idDetalleCompra);
    }
}