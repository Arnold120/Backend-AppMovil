using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface ICategoriaService
    {
        Categorias Add(Categorias categoria);
        List<Categorias> GetAll();
        Categorias GetByID(int id);
        void Update(Categorias categoria);
        void Delete(int id);
    }
}
