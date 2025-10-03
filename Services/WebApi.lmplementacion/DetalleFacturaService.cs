using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class DetalleFacturaService : IDetalleFacturaService
    {
        private readonly string _connectionString;

        public DetalleFacturaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        public async Task<DetalleFactura> AddAsync(DetalleFactura detalleFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Detalles_Factura (
                        Factura_ID, Venta_ID, DetalleVenta_ID, Producto_ID,
                        Cantidad, PrecioUnitario, Subtotal, IVA, Descuento, Total
                    ) 
                    OUTPUT INSERTED.DetalleFactura_ID
                    VALUES (
                        @Factura_ID, @Venta_ID, @DetalleVenta_ID, @Producto_ID,
                        @Cantidad, @PrecioUnitario, @Subtotal, @IVA, @Descuento, @Total
                    )",
                    connection);

                command.Parameters.AddWithValue("@Factura_ID", detalleFactura.Factura_ID);
                command.Parameters.AddWithValue("@Venta_ID", detalleFactura.Venta_ID);
                command.Parameters.AddWithValue("@DetalleVenta_ID", detalleFactura.DetalleVenta_ID);
                command.Parameters.AddWithValue("@Producto_ID", detalleFactura.Producto_ID);
                command.Parameters.AddWithValue("@Cantidad", detalleFactura.Cantidad);
                command.Parameters.AddWithValue("@PrecioUnitario", detalleFactura.PrecioUnitario);
                command.Parameters.AddWithValue("@Subtotal", detalleFactura.Subtotal);
                command.Parameters.AddWithValue("@IVA", detalleFactura.IVA);
                command.Parameters.AddWithValue("@Descuento", detalleFactura.Descuento);
                command.Parameters.AddWithValue("@Total", detalleFactura.Total);

                await connection.OpenAsync();
                detalleFactura.DetalleFactura_ID = Convert.ToInt32(await command.ExecuteScalarAsync());
                return detalleFactura;
            }
        }

        public async Task<List<DetalleFactura>> GetAllAsync()
        {
            var detalles = new List<DetalleFactura>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Detalles_Factura", connection);
                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(new DetalleFactura
                        {
                            DetalleFactura_ID = reader.GetInt32(reader.GetOrdinal("DetalleFactura_ID")),
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            DetalleVenta_ID = reader.GetInt32(reader.GetOrdinal("DetalleVenta_ID")),
                            Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                        });
                    }
                }
            }

            return detalles;
        }

        public async Task<DetalleFactura?> GetByIDAsync(int idDetalleFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Detalles_Factura WHERE DetalleFactura_ID = @DetalleFactura_ID",
                    connection);
                command.Parameters.AddWithValue("@DetalleFactura_ID", idDetalleFactura);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new DetalleFactura
                        {
                            DetalleFactura_ID = reader.GetInt32(reader.GetOrdinal("DetalleFactura_ID")),
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            DetalleVenta_ID = reader.GetInt32(reader.GetOrdinal("DetalleVenta_ID")),
                            Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                        };
                    }
                }
            }

            return null;
        }

        public async Task<List<DetalleFactura>> GetByFacturaIDAsync(int idFactura)
        {
            var detalles = new List<DetalleFactura>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Detalles_Factura WHERE Factura_ID = @Factura_ID",
                    connection);
                command.Parameters.AddWithValue("@Factura_ID", idFactura);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        detalles.Add(new DetalleFactura
                        {
                            DetalleFactura_ID = reader.GetInt32(reader.GetOrdinal("DetalleFactura_ID")),
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            DetalleVenta_ID = reader.GetInt32(reader.GetOrdinal("DetalleVenta_ID")),
                            Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                            Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                            PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                            Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total"))
                        });
                    }
                }
            }

            return detalles;
        }

        public async Task UpdateAsync(DetalleFactura detalleFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    UPDATE Detalles_Factura SET 
                        Cantidad = @Cantidad,
                        PrecioUnitario = @PrecioUnitario,
                        Subtotal = @Subtotal,
                        IVA = @IVA,
                        Descuento = @Descuento,
                        Total = @Total
                    WHERE DetalleFactura_ID = @DetalleFactura_ID",
                    connection);

                command.Parameters.AddWithValue("@DetalleFactura_ID", detalleFactura.DetalleFactura_ID);
                command.Parameters.AddWithValue("@Cantidad", detalleFactura.Cantidad);
                command.Parameters.AddWithValue("@PrecioUnitario", detalleFactura.PrecioUnitario);
                command.Parameters.AddWithValue("@Subtotal", detalleFactura.Subtotal);
                command.Parameters.AddWithValue("@IVA", detalleFactura.IVA);
                command.Parameters.AddWithValue("@Descuento", detalleFactura.Descuento);
                command.Parameters.AddWithValue("@Total", detalleFactura.Total);

                await connection.OpenAsync();
                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                    throw new KeyNotFoundException("Detalle de factura no encontrado.");
            }
        }

        public async Task DeleteAsync(int idDetalleFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "DELETE FROM Detalles_Factura WHERE DetalleFactura_ID = @DetalleFactura_ID",
                    connection);
                command.Parameters.AddWithValue("@DetalleFactura_ID", idDetalleFactura);

                await connection.OpenAsync();
                int affectedRows = await command.ExecuteNonQueryAsync();

                if (affectedRows == 0)
                    throw new KeyNotFoundException("Detalle de factura no encontrado.");
            }
        }
    }
}