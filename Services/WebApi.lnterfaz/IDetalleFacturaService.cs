using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IDetalleFacturaService
    {
        Task<DetalleFactura> AddAsync(DetalleFactura detalleFactura);
        Task<List<DetalleFactura>> GetAllAsync();
        Task<DetalleFactura?> GetByIDAsync(int idDetalleFactura);
        Task<List<DetalleFactura>> GetByFacturaIDAsync(int idFactura);
        Task UpdateAsync(DetalleFactura detalleFactura);
        Task DeleteAsync(int idDetalleFactura);
    }
}