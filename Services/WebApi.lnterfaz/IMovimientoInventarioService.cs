using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IMovimientoInventarioService
    {
        Task<MovimientoInventario> RegistrarMovimiento(MovimientoInventario movimiento);
        IEnumerable<MovimientoInventario> GetAll();
        MovimientoInventario GetById(int id);
        Task<List<MovimientoInventario>> GetByProductoId(int idProducto);
        Task<int> ObtenerStockActual(int idProducto);
        void Delete(int id);
        void Update(int id, MovimientoInventario movimiento);
        Task<decimal> ObtenerUltimoPrecioEntradaAsync(int idProducto);
    }
}