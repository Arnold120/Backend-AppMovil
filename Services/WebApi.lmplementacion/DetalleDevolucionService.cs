using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class DetalleDevolucionService : IDetalleDevolucionService
    {
        private readonly string _connectionString;

        public DetalleDevolucionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<DetalleDevolucion> AddAsync(DetalleDevolucion detalle)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Detalles_Devoluciones
                (Devolucion_ID, DetalleVenta_ID, Producto_ID, Cantidad, PrecioUnitario, IVADevuelto, SubtotalDevuelto, EstadoProducto)
                OUTPUT INSERTED.DetalleDevolucion_ID
                VALUES (@Devolucion_ID, @DetalleVenta_ID, @Producto_ID, @Cantidad, @PrecioUnitario, @IVADevuelto, @SubtotalDevuelto, @EstadoProducto);
            ";

            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Devolucion_ID", detalle.Devolucion_ID);
            cmd.Parameters.AddWithValue("@DetalleVenta_ID", detalle.DetalleVenta_ID);
            cmd.Parameters.AddWithValue("@Producto_ID", detalle.Producto_ID);
            cmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
            cmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
            cmd.Parameters.AddWithValue("@IVADevuelto", (object?)detalle.IVADevuelto ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SubtotalDevuelto", detalle.SubtotalDevuelto);
            cmd.Parameters.AddWithValue("@EstadoProducto", detalle.EstadoProducto);

            detalle.DetalleDevolucion_ID = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            return detalle;
        }

        public async Task<List<DetalleDevolucion>> GetAllAsync()
        {
            var list = new List<DetalleDevolucion>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Detalles_Devoluciones";
            var cmd = new SqlCommand(query, connection);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new DetalleDevolucion
                {
                    DetalleDevolucion_ID = reader.GetInt32(0),
                    Devolucion_ID = reader.GetInt32(1),
                    DetalleVenta_ID = reader.GetInt32(2),
                    Producto_ID = reader.GetInt32(3),
                    Cantidad = reader.GetInt32(4),
                    PrecioUnitario = reader.GetDecimal(5),
                    IVADevuelto = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                    SubtotalDevuelto = reader.GetDecimal(7),
                    EstadoProducto = reader.GetString(8)
                });
            }

            return list;
        }

        public async Task<List<DetalleDevolucion>> GetByDevolucionIDAsync(int devolucionId)
        {
            var list = new List<DetalleDevolucion>();
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Detalles_Devoluciones WHERE Devolucion_ID = @id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", devolucionId);
            using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                list.Add(new DetalleDevolucion
                {
                    DetalleDevolucion_ID = reader.GetInt32(0),
                    Devolucion_ID = reader.GetInt32(1),
                    DetalleVenta_ID = reader.GetInt32(2),
                    Producto_ID = reader.GetInt32(3),
                    Cantidad = reader.GetInt32(4),
                    PrecioUnitario = reader.GetDecimal(5),
                    IVADevuelto = reader.IsDBNull(6) ? null : reader.GetDecimal(6),
                    SubtotalDevuelto = reader.GetDecimal(7),
                    EstadoProducto = reader.GetString(8)
                });
            }

            return list;
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Detalles_Devoluciones WHERE DetalleDevolucion_ID = @id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            if (await cmd.ExecuteNonQueryAsync() == 0)
                throw new KeyNotFoundException("Detalle de devolución no encontrado.");
        }
    }
}