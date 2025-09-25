using Microsoft.Extensions.Configuration;
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
                var command = new SqlCommand("INSERT INTO Clientes (Nombre, Apellido, FechaRegistro) OUTPUT INSERTED.IDCliente VALUES (@Nombre, @Apellido, GETDATE())", connection);

                command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@Apellido", cliente.Apellido);

                await connection.OpenAsync();
                cliente.IDCliente = (int)await command.ExecuteScalarAsync();
            }

            return cliente;
        }

        public IEnumerable<Cliente> GetAll()
        {
            var clientes = new List<Cliente>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT IDCliente, Nombre, Apellido, FechaRegistro FROM Clientes";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            IDCliente = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            FechaRegistro = reader.GetDateTime(3),
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
                var query = "SELECT IDCliente, Nombre, Apellido, FechaRegistro FROM Clientes WHERE IDCliente = @IDCliente";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@IDCliente", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cliente = new Cliente
                        {
                            IDCliente = reader.GetInt32(0),
                            Nombre = reader.GetString(1),
                            Apellido = reader.GetString(2),
                            FechaRegistro = reader.GetDateTime(3),
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
                var command = new SqlCommand("DELETE FROM Clientes WHERE IDCliente = @IDCliente", connection);
                command.Parameters.AddWithValue("@IDCliente", id);

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
                var command = new SqlCommand("UPDATE Clientes SET Nombre = @Nombre, Apellido = @Apellido WHERE IDCliente = @IDCliente", connection);

                command.Parameters.AddWithValue("@IDCliente", id);
                command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
                command.Parameters.AddWithValue("@Apellido", cliente.Apellido);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new KeyNotFoundException("Cliente no encontrado.");
            }
        }
    }
}