using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IUsuarioService
    {
        Task<Usuario> Autenticar(string username, string password);
        Task<Usuario> Registrar(Usuario usuario, string password);
        IEnumerable<Usuario> GetAll();
        Usuario GetById(int id);
        string GenerateJwtToken(Usuario usuario);
    }
}