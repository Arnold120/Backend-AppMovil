using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IFacturaService
    {
        Task<List<Factura>> GetAllAsync();
        Task<Factura> GetByIDAsync(int idFactura);
        Task<Factura> GetByVentaIDAsync(int idVenta);
        Task<List<Factura>> GetByClienteAsync(int idCliente);
        Task<Factura> AddFacturaConDetallesAsync(Factura factura, int idUsuarioAutenticado);
        Task<Factura> UpdateAsync(Factura factura);
        Task AnularFacturaAsync(int idFactura);
        Task<string> GenerarNumeroFacturaAsync();
    }
}