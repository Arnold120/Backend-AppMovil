using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class DetalleCompraService : IDetalleCompraService
    {
        private readonly string _connectionString;

        public DetalleCompraService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<DetalleCompra> AddAsync(DetalleCompra detalleCompra)
        {
            if (detalleCompra == null)
                throw new ArgumentNullException(nameof(detalleCompra));

            const string query = @"
                INSERT INTO DetallesCompra 
                    (Compra_ID, Producto_ID, CantidadUnitaria, MontoUnitario, IVA, , Total) 
                OUTPUT INSERTED.DetalleCompra_ID
                VALUES (@Compra_ID, @Producto_ID, @CantidadUnitaria, @MontoUnitario, @IVA, @Total)";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@Compra_ID", SqlDbType.Int).Value = detalleCompra.Compra_ID;
            command.Parameters.Add("@Producto_ID", SqlDbType.Int).Value = detalleCompra.Producto_ID;
            command.Parameters.Add("@CantidadUnitaria", SqlDbType.Int).Value = detalleCompra.CantidadUnitaria;
            command.Parameters.Add("@MontoUnitario", SqlDbType.Decimal).Value = detalleCompra.MontoUnitario;
            command.Parameters.Add("@IVA", SqlDbType.Decimal).Value = detalleCompra.IVA;
            command.Parameters.Add("@Total", SqlDbType.Decimal).Value = detalleCompra.Total;

            await connection.OpenAsync();
            var idDetalle = (int)await command.ExecuteScalarAsync();
            detalleCompra.DetalleCompra_ID = idDetalle;

            return detalleCompra;
        }

        public async Task<List<DetalleCompra>> GetAllAsync()
        {
            var lista = new List<DetalleCompra>();

            const string query = "SELECT * FROM DetallesCompra";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(MapDetalleCompra(reader));
            }

            return lista;
        }

        public async Task<DetalleCompra?> GetByIDAsync(int idDetalleCompra)
        {
            const string query = "SELECT * FROM DetallesCompra WHERE DetalleCompra_ID = @DetalleCompra_ID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@DetalleCompra_ID", SqlDbType.Int).Value = idDetalleCompra;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapDetalleCompra(reader);
            }

            return null;
        }

        public async Task<List<DetalleCompra>> GetByCompraIDAsync(int idCompra)
        {
            var lista = new List<DetalleCompra>();

            const string query = "SELECT * FROM DetallesCompra WHERE Compra_ID = @Compra_ID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@Compra_ID", SqlDbType.Int).Value = idCompra;

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(MapDetalleCompra(reader));
            }

            return lista;
        }

        public async Task UpdateAsync(DetalleCompra detalleCompra)
        {
            if (detalleCompra == null)
                throw new ArgumentNullException(nameof(detalleCompra));

            const string query = @"
                UPDATE DetallesCompra SET 
                    Compra_ID = @Compra_ID,
                    Producto_ID = @Producto_ID,
                    CantidadUnitaria = @CantidadUnitaria,
                    MontoUnitario = @MontoUnitario,
                    IVA = @IVA,
                     = @,
                    Total = @Total
                WHERE DetalleCompra_ID = @DetalleCompra_ID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);

            command.Parameters.Add("@DetalleCompra_ID", SqlDbType.Int).Value = detalleCompra.DetalleCompra_ID;
            command.Parameters.Add("@Compra_ID", SqlDbType.Int).Value = detalleCompra.Compra_ID;
            command.Parameters.Add("@Producto_ID", SqlDbType.Int).Value = detalleCompra.Producto_ID;
            command.Parameters.Add("@CantidadUnitaria", SqlDbType.Int).Value = detalleCompra.CantidadUnitaria;
            command.Parameters.Add("@MontoUnitario", SqlDbType.Decimal).Value = detalleCompra.MontoUnitario;
            command.Parameters.Add("@IVA", SqlDbType.Decimal).Value = detalleCompra.IVA;
            command.Parameters.Add("@Total", SqlDbType.Decimal).Value = detalleCompra.Total;

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"No se encontró detalle de compra con ID {detalleCompra.DetalleCompra_ID}");
        }

        public async Task DeleteAsync(int idDetalleCompra)
        {
            const string query = "DELETE FROM DetallesCompra WHERE DetalleCompra_ID = @DetalleCompra_ID";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.Add("@DetalleCompra_ID", SqlDbType.Int).Value = idDetalleCompra;

            await connection.OpenAsync();
            var rowsAffected = await command.ExecuteNonQueryAsync();

            if (rowsAffected == 0)
                throw new KeyNotFoundException($"No se encontró detalle de compra con ID {idDetalleCompra}");
        }

        private DetalleCompra MapDetalleCompra(SqlDataReader reader)
        {
            return new DetalleCompra
            {
                DetalleCompra_ID = reader.GetInt32(reader.GetOrdinal("DetalleCompra_ID")),
                Compra_ID = reader.GetInt32(reader.GetOrdinal("Compra_ID")),
                Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                CantidadUnitaria = reader.GetInt32(reader.GetOrdinal("CantidadUnitaria")),
                MontoUnitario = reader.GetDecimal(reader.GetOrdinal("MontoUnitario")),
                IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                Total = reader.GetDecimal(reader.GetOrdinal("Total"))
            };
        }
    }
}