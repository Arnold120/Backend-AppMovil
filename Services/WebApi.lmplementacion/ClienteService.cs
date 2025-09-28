using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class ClienteService : IClienteService
    {
        private readonly string _connectionString;

        public ClienteService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<Cliente> Registrar(Cliente cliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO Clientes (Nombre, Apellido, Direccion, Telefono, Email, Activo,  FechaRegistro) OUTPUT INSERTED.Cliente_ID VALUES (@Nombre, @Apellido, @Direccion, @Telefono, @Email, @Activo,  @FechaRegistro)", connection);

                command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                command.Parameters.AddWithValue("@Email", cliente.Email);
                command.Parameters.AddWithValue("@Activo", cliente.Activo);
                command.Parameters.AddWithValue("@FechaRegistro", cliente.FechaRegistro);

                await connection.OpenAsync();
                cliente.Cliente_ID = (int)await command.ExecuteScalarAsync();
            }

            return cliente;
        }

        public IEnumerable<Cliente> GetAll()
        {
            var clientes = new List<Cliente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Cliente_ID, Nombre, Apellido, Direccion, Telefono, Email, Activo, FechaRegistro FROM Clientes";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            Cliente_ID = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Direccion = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Activo = reader.GetBoolean(6),
                            FechaRegistro = reader.GetDateTime(7),
                        });
                    }
                }
            }
            return clientes;
        }

        public Cliente GetById(int id)
        {
            Cliente? cliente = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Cliente_ID, Nombre, Apellido, Direccion, Telefono, Email, Activo, FechaRegistro FROM Clientes WHERE Cliente_ID = @Cliente_ID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Cliente_ID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cliente = new Cliente
                        {
                            Cliente_ID = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            Direccion = reader.GetString(3),
                            Telefono = reader.GetString(4),
                            Email = reader.GetString(5),
                            Activo = reader.GetBoolean(6),
                            FechaRegistro = reader.GetDateTime(7),
                        };
                    }
                }
            }
            return cliente;
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Clientes WHERE Cliente_ID = @Cliente_ID", connection);
                command.Parameters.AddWithValue("@Cliente_ID", id);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new KeyNotFoundException("Cliente no encontrado.");
            }
        }

        public void Update(int id, Cliente cliente)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido, Direccion = @Direccion, Telefono = @Telefono, Email = @Email, Activo = @Activo, FechaRegistro = @FechaRegistro WHERE Cliente_ID = @Cliente_ID", connection);

                command.Parameters.AddWithValue("@Cliente_ID", id);
                command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@Apellido", cliente.Apellido);
                command.Parameters.AddWithValue("@Direccion", cliente.Direccion);
                command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
                command.Parameters.AddWithValue("@Email", cliente.Email);
                command.Parameters.AddWithValue("@Activo", cliente.Activo);
                command.Parameters.AddWithValue("@FechaRegistro", cliente.FechaRegistro);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new KeyNotFoundException("Cliente no encontrado.");
            }
        }
    }
}