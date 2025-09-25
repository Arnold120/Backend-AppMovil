using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class RolService : IRolService
    {
        private readonly string _connectionString;

        public RolService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
            }
        }

        public async Task<Rol> CrearRolAsync(string nombreRol, string descripcion)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryExistente = "SELECT TOP 1 * FROM Rol WHERE NombreRol = @NombreRol";
                using (var commandExistente = new SqlCommand(queryExistente, connection))
                {
                    commandExistente.Parameters.AddWithValue("@NombreRol", nombreRol);

                    using (var reader = await commandExistente.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            throw new InvalidOperationException("El rol ya existe.");
                        }
                    }
                }

                var queryInsert = @"
                    INSERT INTO Rol (NombreRol, Descripcion, Activo, FechaRegistro)
                    VALUES (@NombreRol, @Descripcion, 1, GETDATE())";
                using (var commandInsert = new SqlCommand(queryInsert, connection))
                {
                    commandInsert.Parameters.AddWithValue("@NombreRol", nombreRol);
                    commandInsert.Parameters.AddWithValue("@Descripcion", descripcion);

                    int affectedRows = await commandInsert.ExecuteNonQueryAsync();
                    if (affectedRows == 0)
                    {
                        throw new Exception("Error al crear el rol.");
                    }
                }

                var querySelect = "SELECT * FROM Rol WHERE NombreRol = @NombreRol";
                using (var commandSelect = new SqlCommand(querySelect, connection))
                {
                    commandSelect.Parameters.AddWithValue("@NombreRol", nombreRol);

                    using (var reader = await commandSelect.ExecuteReaderAsync())
                    {
                        if (reader.HasRows)
                        {
                            await reader.ReadAsync();
                            return new Rol
                            {
                                Rol_ID = reader.GetInt32(reader.GetOrdinal("Rol_ID")),
                                NombreRol = reader.GetString(reader.GetOrdinal("NombreRol")),
                                Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                            };
                        }
                    }
                }

                return null;
            }
        }

        public async Task<List<Rol>> ObtenerTodosLosRolesAsync()
        {
            var roles = new List<Rol>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT * FROM Rol";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(new Rol
                            {
                                Rol_ID = reader.GetInt32(reader.GetOrdinal("Rol_ID")),
                                NombreRol = reader.GetString(reader.GetOrdinal("NombreRol")),
                                Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                                Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                                FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                            });
                        }
                    }
                }
            }

            return roles;
        }

        public async Task<bool> EliminarRolAsync(string nombreRol)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var queryDelete = "DELETE FROM Rol WHERE NombreRol = @NombreRol";
                using (var commandDelete = new SqlCommand(queryDelete, connection))
                {
                    commandDelete.Parameters.AddWithValue("@NombreRol", nombreRol);

                    int rowsAffected = await commandDelete.ExecuteNonQueryAsync();

                    return rowsAffected > 0;
                }
            }
        }

        public async Task<bool> VerificarPermisoAsync(string nombreUsuario, string metodo, string endpoint)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                using (var command = new SqlCommand("VerificarPermiso", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@nombreUsuario", nombreUsuario);
                    command.Parameters.AddWithValue("@metodo", metodo);
                    command.Parameters.AddWithValue("@endpoint", endpoint);

                    var outputParameter = new SqlParameter("@permisoConcedido", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(outputParameter);
                    await command.ExecuteNonQueryAsync();

                    return (bool)outputParameter.Value;
                }
            }
        }
    }
}