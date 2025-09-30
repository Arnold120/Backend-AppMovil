using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IDetalleVentaService
    {
        Task<DetalleVenta> AddAsync(DetalleVenta detalleVenta);       
        Task<List<DetalleVenta>> GetAllAsync();                     
        Task<DetalleVenta?> GetByIDAsync(int idDetalleVenta);     
        Task<List<DetalleVenta>> GetByVentaIDAsync(int idVenta);      
        Task UpdateAsync(DetalleVenta detalleVenta);           
        Task DeleteAsync(int idDetalleVenta);                     
    }
}