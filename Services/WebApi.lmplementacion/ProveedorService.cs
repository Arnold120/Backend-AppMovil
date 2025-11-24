using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class ProveedorService : IProveedorService
    {
        private readonly string _connectionString;

        public ProveedorService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public Proveedor Add(Proveedor proveedor)
        {
            if (proveedor == null)
                throw new ArgumentNullException(nameof(proveedor), "El proveedor no puede ser nulo.");

            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                "INSERT INTO Proveedores (NombreEmpresa, Direccion, Telefono, Email, AceptaDevoluciones, TiempoDevolucion, PorcentajeCobertura) OUTPUT INSERTED.Proveedor_ID VALUES (@NombreEmpresa, @Direccion, @Telefono, @Email, @AceptaDevoluciones, @TiempoDevolucion, @PorcentajeCobertura)",
                connection);

            command.Parameters.AddWithValue("@NombreEmpresa", proveedor.NombreEmpresa ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", proveedor.Direccion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Telefono", proveedor.Telefono ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", proveedor.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AceptaDevoluciones", proveedor.AceptaDevoluciones);
            command.Parameters.AddWithValue("@TiempoDevolucion", proveedor.TiempoDevolucion);
            command.Parameters.AddWithValue("@PorcentajeCobertura", proveedor.PorcentajeCobertura);

            connection.Open();
            proveedor.Proveedor_ID = (int)command.ExecuteScalar();

            return proveedor;
        }

        public List<Proveedor> GetAll()
        {
            var proveedores = new List<Proveedor>();

            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Proveedores", connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                proveedores.Add(new Proveedor
                {
                    Proveedor_ID = reader.GetInt32(reader.GetOrdinal("Proveedor_ID")),
                    NombreEmpresa = reader.GetString(reader.GetOrdinal("NombreEmpresa")),
                    Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    AceptaDevoluciones = reader.GetBoolean(reader.GetOrdinal("AceptaDevoluciones")),
                    TiempoDevolucion = reader.GetInt32(reader.GetOrdinal("TiempoDevolucion")),
                    PorcentajeCobertura = reader.GetDecimal(reader.GetOrdinal("PorcentajeCobertura"))
                });
            }

            return proveedores;
        }

        public Proveedor GetByID(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("SELECT * FROM Proveedores WHERE Proveedor_ID = @Proveedor_ID", connection);
            command.Parameters.AddWithValue("@Proveedor_ID", id);

            connection.Open();

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Proveedor
                {
                    Proveedor_ID = reader.GetInt32(reader.GetOrdinal("Proveedor_ID")),
                    NombreEmpresa = reader.GetString(reader.GetOrdinal("NombreEmpresa")),
                    Direccion = reader.GetString(reader.GetOrdinal("Direccion")),
                    Telefono = reader.GetString(reader.GetOrdinal("Telefono")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    AceptaDevoluciones = reader.GetBoolean(reader.GetOrdinal("AceptaDevoluciones")),
                    TiempoDevolucion = reader.GetInt32(reader.GetOrdinal("TiempoDevolucion")),
                    PorcentajeCobertura = reader.GetDecimal(reader.GetOrdinal("PorcentajeCobertura"))
                };
            }

            return null;
        }

        public void Update(Proveedor proveedor)
        {
            if (proveedor == null)
                throw new ArgumentNullException(nameof(proveedor), "El proveedor no puede ser nulo.");

            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                "UPDATE Proveedores SET NombreEmpresa = @NombreEmpresa, Direccion = @Direccion, Telefono = @Telefono, Email = @Email, AceptaDevoluciones = @AceptaDevoluciones, TiempoDevolucion = @TiempoDevolucion, PorcentajeCobertura = @PorcentajeCobertura WHERE Proveedor_ID = @Proveedor_ID",
                connection);

            command.Parameters.AddWithValue("@Proveedor_ID", proveedor.Proveedor_ID);
            command.Parameters.AddWithValue("@NombreEmpresa", proveedor.NombreEmpresa ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Direccion", proveedor.Direccion ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Telefono", proveedor.Telefono ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", proveedor.Email ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@AceptaDevoluciones", proveedor.AceptaDevoluciones);
            command.Parameters.AddWithValue("@TiempoDevolucion", proveedor.TiempoDevolucion);
            command.Parameters.AddWithValue("@PorcentajeCobertura", proveedor.PorcentajeCobertura);

            connection.Open();
            command.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("DELETE FROM Proveedores WHERE Proveedor_ID = @Proveedor_ID", connection);
            command.Parameters.AddWithValue("@Proveedor_ID", id);

            connection.Open();
            command.ExecuteNonQuery();
        }
    }
}