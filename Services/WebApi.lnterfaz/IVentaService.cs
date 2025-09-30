using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IVentaService
    {
        Task<List<Venta>> GetAllAsync();                                  
        Task<Venta> GetByIDAsync(int idVenta);                    
        Task<List<Venta>> GetByClienteAsync(int idCliente);            
        Task DeleteAsync(int idVenta);                            
        Task<Venta> AddVentaConDetallesAsync(Venta venta, int idUsuarioAutenticado); 
        Task<Venta> UpdateAsync(Venta venta);
        Task<decimal> ObtenerPrecioUnitarioFIFO(int idProducto);
    }
}