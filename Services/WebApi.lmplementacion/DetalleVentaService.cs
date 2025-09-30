using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class DetalleVentaService : IDetalleVentaService
    {
        private readonly string _connectionString;

        public DetalleVentaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        public async Task<DetalleVenta> AddAsync(DetalleVenta detalleVenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Detalles_Ventas (
                        Venta_ID, Producto_ID, Cantidad, PrecioUnitario,
                        SubTotal, IVA, Total, TipoComprobante
                    ) 
                    OUTPUT INSERTED.DetalleVenta_ID
                    VALUES (
                        @Venta_ID, @Producto_ID, @Cantidad, @PrecioUnitario,
                        @SubTotal, @IVA, @Total, @TipoComprobante
                    )",
                    connection);

                command.Parameters.AddWithValue("@Venta_ID", detalleVenta.Venta_ID);
                command.Parameters.AddWithValue("@Producto_ID", detalleVenta.Producto_ID);
                command.Parameters.AddWithValue("@Cantidad", detalleVenta.Cantidad);
                command.Parameters.AddWithValue("@PrecioUnitario", detalleVenta.PrecioUnitario);
                command.Parameters.AddWithValue("@SubTotal", detalleVenta.SubTotal);
                command.Parameters.AddWithValue("@IVA", detalleVenta.IVA);
                command.Parameters.AddWithValue("@Total", detalleVenta.Total);
                command.Parameters.AddWithValue("@TipoComprobante", detalleVenta.TipoComprobante);

                await connection.OpenAsync();
                detalleVenta.DetalleVenta_ID = Convert.ToInt32(await command.ExecuteScalarAsync());
                return detalleVenta;
            }
        }

        public async Task<List<DetalleVenta>> GetAllAsync()
        {
            var detalles = new List<DetalleVenta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Detalles_Ventas", connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(new DetalleVenta
                        {
                            DetalleVenta_ID = reader.GetInt32(reader.GetOrdinal("DetalleVenta_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            TipoComprobante = reader.GetString(reader.GetOrdinal("TipoComprobante"))
                        });
                    }
                }
            }

            return detalles;
        }

        public async Task<DetalleVenta?> GetByIDAsync(int idDetalleVenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Detalles_Ventas WHERE DetalleVenta_ID = @DetalleVenta_ID",
                    connection);
                command.Parameters.AddWithValue("@DetalleVenta_ID", idDetalleVenta);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new DetalleVenta
                        {
                            DetalleVenta_ID = reader.GetInt32(reader.GetOrdinal("DetalleVenta_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            TipoComprobante = reader.GetString(reader.GetOrdinal("TipoComprobante"))
                        };
                    }
                }
            }

            return null;
        }

        public async Task<List<DetalleVenta>> GetByVentaIDAsync(int idVenta)
        {
            var detalles = new List<DetalleVenta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Detalles_Ventas WHERE Venta_ID = @Venta_ID",
                    connection);
                command.Parameters.AddWithValue("@Venta_ID", idVenta);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(new DetalleVenta
                        {
                            DetalleVenta_ID = reader.GetInt32(reader.GetOrdinal("DetalleVenta_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            TipoComprobante = reader.GetString(reader.GetOrdinal("TipoComprobante"))
                        });
                    }
                }
            }

            return detalles;
        }

        public async Task UpdateAsync(DetalleVenta detalleVenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    UPDATE Detalles_Ventas SET 
                        Venta_ID = @Venta_ID,
                        Producto_ID = @Producto_ID,
                        Cantidad = @Cantidad,
                        PrecioUnitario = @PrecioUnitario,
                        SubTotal = @SubTotal,
                        IVA = @IVA,
                        Total = @Total,
                        TipoComprobante = @TipoComprobante
                    WHERE DetalleVenta_ID = @DetalleVenta_ID",
                    connection);

                command.Parameters.AddWithValue("@DetalleVenta_ID", detalleVenta.DetalleVenta_ID);
                command.Parameters.AddWithValue("@Venta_ID", detalleVenta.Venta_ID);
                command.Parameters.AddWithValue("@Producto_ID", detalleVenta.Producto_ID);
                command.Parameters.AddWithValue("@Cantidad", detalleVenta.Cantidad);
                command.Parameters.AddWithValue("@PrecioUnitario", detalleVenta.PrecioUnitario);
                command.Parameters.AddWithValue("@SubTotal", detalleVenta.SubTotal);
                command.Parameters.AddWithValue("@IVA", detalleVenta.IVA);
                command.Parameters.AddWithValue("@Total", detalleVenta.Total);
                command.Parameters.AddWithValue("@TipoComprobante", detalleVenta.TipoComprobante);

                await connection.OpenAsync();
                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                    throw new KeyNotFoundException("Detalle de venta no encontrado.");
            }
        }

        public async Task DeleteAsync(int idDetalleVenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "DELETE FROM Detalles_Ventas WHERE DetalleVenta_ID = @DetalleVenta_ID",
                    connection);
                command.Parameters.AddWithValue("@DetalleVenta_ID", idDetalleVenta);

                await connection.OpenAsync();
                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                    throw new KeyNotFoundException("Detalle de venta no encontrado.");
            }
        }
    }
}