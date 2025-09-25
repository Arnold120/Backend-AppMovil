using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IUsuarioRolService
    {
        UsuarioRol Add(UsuarioRol usuarioRol);
        List<UsuarioRol> GetAll();
        UsuarioRol GetByID(int id);
        void Update(UsuarioRol usuarioRol);
        void Delete(int id);
    }
}