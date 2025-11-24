using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IProductoService
    {
        Task<Producto> Registrar(Producto producto);
        IEnumerable<Producto> GetAll();
        IEnumerable<Producto> GetAll2();
        Producto GetById(int id);
        List<Producto> GetByNombre(string nombreProducto);
        void Delete(int id);
        void Update(int id, Producto producto);
    }
}