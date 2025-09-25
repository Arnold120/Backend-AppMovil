using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IMarcaService
    {
        Marcas Add(Marcas marca);
        void Update(Marcas marca);
        void Delete(int id);
        List<Marcas> GetAll();
        Marcas GetByID(int id);
    }

}
