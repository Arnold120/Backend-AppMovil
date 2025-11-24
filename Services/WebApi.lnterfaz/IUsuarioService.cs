using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IUsuarioService
    {
        Task<Usuario> Registrar(Usuario usuario, string password);
        Task<Usuario?> Autenticar(string nombreUsuario, string password);
        Task<bool> CerrarSesion(int usuarioId);
        IEnumerable<Usuario> GetAll();
        Usuario GetById(int id);
        string ObtenerRolDelUsuario(int idUsuario);
        string GenerateJwtToken(Usuario usuario);
        IEnumerable<Usuario> ObtenerUsuariosEnLinea();
        bool EstaEnLinea(Usuario usuario);
        Task<bool> ActualizarActividad(int usuarioId);
        (bool estaEnLinea, DateTime? ultimaActividad) VerificarEstadoEnLinea(int usuarioId);
        IEnumerable<object> ObtenerTodosLosEstadosEnLinea();
    }
}
