using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class UsuarioRolService : IUsuarioRolService
    {
        private readonly string _connectionString;

        public UsuarioRolService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public UsuarioRol Add(UsuarioRol usuarioRol)
        {
            if (usuarioRol == null)
                throw new ArgumentNullException(nameof(usuarioRol), "El objeto UsuarioRol no puede ser nulo.");

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                "INSERT INTO UsuarioRol (Usuario_ID, Rol_ID, FechaAsignacion) VALUES (@Usuario_ID, @Rol_ID, @FechaAsignacion)",
                connection))
            {
                command.Parameters.Add("@Usuario_ID", SqlDbType.Int).Value = usuarioRol.Usuario_ID;
                command.Parameters.Add("@Rol_ID", SqlDbType.Int).Value = usuarioRol.Rol_ID;
                command.Parameters.Add("@FechaAsignacion", SqlDbType.DateTime).Value = usuarioRol.FechaAsignacion;

                connection.Open();
                command.ExecuteNonQuery();
            }

            return usuarioRol;
        }

        public List<UsuarioRol> GetAll()
        {
            var lista = new List<UsuarioRol>();

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM UsuarioRol", connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new UsuarioRol
                        {
                            Usuario_ID = reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Rol_ID = reader.GetInt32(reader.GetOrdinal("Rol_ID")),
                            FechaAsignacion = reader.GetDateTime(reader.GetOrdinal("FechaAsignacion"))
                        });
                    }
                }
            }

            return lista;
        }

        public UsuarioRol GetByID(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("SELECT * FROM UsuarioRol WHERE Usuario_ID = @Usuario_ID", connection))
            {
                command.Parameters.Add("@Usuario_ID", SqlDbType.Int).Value = id;
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new UsuarioRol
                        {
                            Usuario_ID = reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Rol_ID = reader.GetInt32(reader.GetOrdinal("Rol_ID")),
                            FechaAsignacion = reader.GetDateTime(reader.GetOrdinal("FechaAsignacion"))
                        };
                    }
                }
            }

            throw new KeyNotFoundException("No se encontró la relación usuario-rol con el ID especificado.");
        }

        public void Update(UsuarioRol usuarioRol)
        {
            if (usuarioRol == null)
                throw new ArgumentNullException(nameof(usuarioRol), "El objeto UsuarioRol no puede ser nulo.");

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(
                "UPDATE UsuarioRol SET Rol_ID = @Rol_ID, FechaAsignacion = @FechaAsignacion WHERE Usuario_ID = @Usuario_ID",
                connection))
            {
                command.Parameters.Add("@Usuario_ID", SqlDbType.Int).Value = usuarioRol.Usuario_ID;
                command.Parameters.Add("@Rol_ID", SqlDbType.Int).Value = usuarioRol.Rol_ID;
                command.Parameters.Add("@FechaAsignacion", SqlDbType.DateTime).Value = usuarioRol.FechaAsignacion;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand("DELETE FROM UsuarioRol WHERE Usuario_ID = @Usuario_ID", connection))
            {
                command.Parameters.Add("@Usuario_ID", SqlDbType.Int).Value = id;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}