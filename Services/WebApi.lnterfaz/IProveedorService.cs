using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IProveedorService
    {
        Proveedor Add(Proveedor proveedor);
        List<Proveedor> GetAll();
        Proveedor GetByID(int id);
        void Update(Proveedor proveedor);
        void Delete(int id);
    }
}