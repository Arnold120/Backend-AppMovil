using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class FacturaService : IFacturaService
    {
        private readonly string _connectionString;

        public FacturaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        public async Task<List<Factura>> GetAllAsync()
        {
            var facturas = new List<Factura>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    SELECT * FROM Factura 
                    ORDER BY FechaFactura DESC",
                    connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var factura = new Factura
                        {
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Cliente_ID = reader.IsDBNull(reader.GetOrdinal("Cliente_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
                            Serie = reader.IsDBNull(reader.GetOrdinal("Serie")) ? null : reader.GetString(reader.GetOrdinal("Serie")),
                            Correlativo = reader.IsDBNull(reader.GetOrdinal("Correlativo")) ? null : reader.GetString(reader.GetOrdinal("Correlativo")),
                            FechaFactura = reader.GetDateTime(reader.GetOrdinal("FechaFactura")),
                            FechaVencimiento = reader.IsDBNull(reader.GetOrdinal("FechaVencimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            TotalFactura = reader.GetDecimal(reader.GetOrdinal("TotalFactura")),
                            Moneda = reader.GetString(reader.GetOrdinal("Moneda")),
                            MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
                            TipoPago = reader.GetString(reader.GetOrdinal("TipoPago")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesFactura = new List<DetalleFactura>()
                        };

                        await CargarDetallesFactura(factura);
                        facturas.Add(factura);
                    }
                }
            }

            return facturas;
        }

        public async Task<Factura> GetByIDAsync(int idFactura)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Factura WHERE Factura_ID = @Factura_ID",
                    connection);
                command.Parameters.AddWithValue("@Factura_ID", idFactura);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var factura = new Factura
                        {
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Cliente_ID = reader.IsDBNull(reader.GetOrdinal("Cliente_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
                            Serie = reader.IsDBNull(reader.GetOrdinal("Serie")) ? null : reader.GetString(reader.GetOrdinal("Serie")),
                            Correlativo = reader.IsDBNull(reader.GetOrdinal("Correlativo")) ? null : reader.GetString(reader.GetOrdinal("Correlativo")),
                            FechaFactura = reader.GetDateTime(reader.GetOrdinal("FechaFactura")),
                            FechaVencimiento = reader.IsDBNull(reader.GetOrdinal("FechaVencimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            TotalFactura = reader.GetDecimal(reader.GetOrdinal("TotalFactura")),
                            Moneda = reader.GetString(reader.GetOrdinal("Moneda")),
                            MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
                            TipoPago = reader.GetString(reader.GetOrdinal("TipoPago")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesFactura = new List<DetalleFactura>()
                        };

                        await CargarDetallesFactura(factura);
                        return factura;
                    }
                }
            }

            throw new KeyNotFoundException("Factura no encontrada.");
        }

        public async Task<Factura> GetByVentaIDAsync(int idVenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Factura WHERE Venta_ID = @Venta_ID",
                    connection);
                command.Parameters.AddWithValue("@Venta_ID", idVenta);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var factura = new Factura
                        {
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Cliente_ID = reader.IsDBNull(reader.GetOrdinal("Cliente_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
                            Serie = reader.IsDBNull(reader.GetOrdinal("Serie")) ? null : reader.GetString(reader.GetOrdinal("Serie")),
                            Correlativo = reader.IsDBNull(reader.GetOrdinal("Correlativo")) ? null : reader.GetString(reader.GetOrdinal("Correlativo")),
                            FechaFactura = reader.GetDateTime(reader.GetOrdinal("FechaFactura")),
                            FechaVencimiento = reader.IsDBNull(reader.GetOrdinal("FechaVencimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            TotalFactura = reader.GetDecimal(reader.GetOrdinal("TotalFactura")),
                            Moneda = reader.GetString(reader.GetOrdinal("Moneda")),
                            MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
                            TipoPago = reader.GetString(reader.GetOrdinal("TipoPago")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesFactura = new List<DetalleFactura>()
                        };

                        await CargarDetallesFactura(factura);
                        return factura;
                    }
                }
            }

            throw new KeyNotFoundException("Factura no encontrada para la venta especificada.");
        }

        public async Task<List<Factura>> GetByClienteAsync(int idCliente)
        {
            var facturas = new List<Factura>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Factura WHERE Cliente_ID = @Cliente_ID ORDER BY FechaFactura DESC",
                    connection);
                command.Parameters.AddWithValue("@Cliente_ID", idCliente);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var factura = new Factura
                        {
                            Factura_ID = reader.GetInt32(reader.GetOrdinal("Factura_ID")),
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Cliente_ID = reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            NumeroFactura = reader.GetString(reader.GetOrdinal("NumeroFactura")),
                            Serie = reader.IsDBNull(reader.GetOrdinal("Serie")) ? null : reader.GetString(reader.GetOrdinal("Serie")),
                            Correlativo = reader.IsDBNull(reader.GetOrdinal("Correlativo")) ? null : reader.GetString(reader.GetOrdinal("Correlativo")),
                            FechaFactura = reader.GetDateTime(reader.GetOrdinal("FechaFactura")),
                            FechaVencimiento = reader.IsDBNull(reader.GetOrdinal("FechaVencimiento")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("FechaVencimiento")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            TotalFactura = reader.GetDecimal(reader.GetOrdinal("TotalFactura")),
                            Moneda = reader.GetString(reader.GetOrdinal("Moneda")),
                            MetodoPago = reader.GetString(reader.GetOrdinal("MetodoPago")),
                            TipoPago = reader.GetString(reader.GetOrdinal("TipoPago")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesFactura = new List<DetalleFactura>()
                        };

                        await CargarDetallesFactura(factura);
                        facturas.Add(factura);
                    }
                }
            }

            return facturas;
        }


        public async Task<Factura> AddFacturaConDetallesAsync(Factura factura, int idUsuarioAutenticado)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                var checkFacturaCmd = new SqlCommand(
                    "SELECT COUNT(*) FROM Factura WHERE Venta_ID = @Venta_ID",
                    connection, transaction);
                checkFacturaCmd.Parameters.AddWithValue("@Venta_ID", factura.Venta_ID);

                var existe = (int)await checkFacturaCmd.ExecuteScalarAsync();
                if (existe > 0)
                    throw new InvalidOperationException("Ya existe una factura para esta venta.");

                var numeroFactura = await GenerarNumeroFacturaAsync();
                factura.NumeroFactura = numeroFactura;
                factura.FechaFactura = DateTime.Now;
                factura.Estado = "Emitida";

                var insertFacturaCmd = new SqlCommand(@"
                    INSERT INTO Factura (
                        Venta_ID, Cliente_ID, NumeroFactura, Serie, Correlativo,
                        FechaFactura, FechaVencimiento, SubTotal, IVA, Descuento,
                        TotalFactura, Moneda, MetodoPago, TipoPago, Estado
                    ) 
                    OUTPUT INSERTED.Factura_ID
                    VALUES (
                        @Venta_ID, @Cliente_ID, @NumeroFactura, @Serie, @Correlativo,
                        @FechaFactura, @FechaVencimiento, @SubTotal, @IVA, @Descuento,
                        @TotalFactura, @Moneda, @MetodoPago, @TipoPago, @Estado
                    )",
                    connection, transaction);

                insertFacturaCmd.Parameters.AddWithValue("@Venta_ID", factura.Venta_ID);
                insertFacturaCmd.Parameters.AddWithValue("@Cliente_ID", factura.Cliente_ID ?? (object)DBNull.Value);
                insertFacturaCmd.Parameters.AddWithValue("@NumeroFactura", factura.NumeroFactura);
                insertFacturaCmd.Parameters.AddWithValue("@Serie", factura.Serie ?? (object)DBNull.Value);
                insertFacturaCmd.Parameters.AddWithValue("@Correlativo", factura.Correlativo ?? (object)DBNull.Value);
                insertFacturaCmd.Parameters.AddWithValue("@FechaFactura", factura.FechaFactura);
                insertFacturaCmd.Parameters.AddWithValue("@FechaVencimiento", factura.FechaVencimiento ?? (object)DBNull.Value);
                insertFacturaCmd.Parameters.AddWithValue("@SubTotal", factura.SubTotal);
                insertFacturaCmd.Parameters.AddWithValue("@IVA", factura.IVA);
                insertFacturaCmd.Parameters.AddWithValue("@Descuento", factura.Descuento);
                insertFacturaCmd.Parameters.AddWithValue("@TotalFactura", factura.TotalFactura);
                insertFacturaCmd.Parameters.AddWithValue("@Moneda", factura.Moneda);
                insertFacturaCmd.Parameters.AddWithValue("@MetodoPago", factura.MetodoPago);
                insertFacturaCmd.Parameters.AddWithValue("@TipoPago", factura.TipoPago);
                insertFacturaCmd.Parameters.AddWithValue("@Estado", factura.Estado);

                var facturaId = Convert.ToInt32(await insertFacturaCmd.ExecuteScalarAsync());

                foreach (var detalle in factura.DetallesFactura)
                {
                    var insertDetalleCmd = new SqlCommand(@"
                        INSERT INTO Detalles_Factura (
                            Factura_ID, Venta_ID, DetalleVenta_ID, Producto_ID,
                            Cantidad, PrecioUnitario, Subtotal, IVA, Descuento, Total
                        ) 
                        VALUES (
                            @Factura_ID, @Venta_ID, @DetalleVenta_ID, @Producto_ID,
                            @Cantidad, @PrecioUnitario, @Subtotal, @IVA, @Descuento, @Total
                        )",
                        connection, transaction);

                    insertDetalleCmd.Parameters.AddWithValue("@Factura_ID", facturaId);
                    insertDetalleCmd.Parameters.AddWithValue("@Venta_ID", factura.Venta_ID);
                    insertDetalleCmd.Parameters.AddWithValue("@DetalleVenta_ID", detalle.DetalleVenta_ID);
                    insertDetalleCmd.Parameters.AddWithValue("@Producto_ID", detalle.Producto_ID);
                    insertDetalleCmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                    insertDetalleCmd.Parameters.AddWithValue("@Subtotal", detalle.Subtotal);
                    insertDetalleCmd.Parameters.AddWithValue("@IVA", detalle.IVA);
                    insertDetalleCmd.Parameters.AddWithValue("@Descuento", detalle.Descuento);
                    insertDetalleCmd.Parameters.AddWithValue("@Total", detalle.Total);

                    await insertDetalleCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
                return await GetByIDAsync(facturaId);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Factura> UpdateAsync(Factura factura)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                UPDATE Factura SET 
                    Serie = @Serie,
                    Correlativo = @Correlativo,
                    FechaVencimiento = @FechaVencimiento,
                    Descuento = @Descuento,
                    Moneda = @Moneda,
                    MetodoPago = @MetodoPago,
                    TipoPago = @TipoPago,
                    Estado = @Estado
                WHERE Factura_ID = @Factura_ID",
                connection);

            command.Parameters.AddWithValue("@Factura_ID", factura.Factura_ID);
            command.Parameters.AddWithValue("@Serie", factura.Serie ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Correlativo", factura.Correlativo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FechaVencimiento", factura.FechaVencimiento ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Descuento", factura.Descuento);
            command.Parameters.AddWithValue("@Moneda", factura.Moneda);
            command.Parameters.AddWithValue("@MetodoPago", factura.MetodoPago);
            command.Parameters.AddWithValue("@TipoPago", factura.TipoPago);
            command.Parameters.AddWithValue("@Estado", factura.Estado);

            int affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
                throw new KeyNotFoundException("Factura no encontrada.");

            return await GetByIDAsync(factura.Factura_ID);
        }

        public async Task AnularFacturaAsync(int idFactura)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(
                "UPDATE Factura SET Estado = 'Anulada' WHERE Factura_ID = @Factura_ID",
                connection);
            command.Parameters.AddWithValue("@Factura_ID", idFactura);

            int affectedRows = await command.ExecuteNonQueryAsync();

            if (affectedRows == 0)
                throw new KeyNotFoundException("Factura no encontrada.");
        }

        public async Task<string> GenerarNumeroFacturaAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT COUNT(*) + 1 
                FROM Factura 
                WHERE YEAR(FechaFactura) = YEAR(GETDATE())",
                connection);

            var consecutivo = Convert.ToInt32(await command.ExecuteScalarAsync());
            return $"F-{DateTime.Now:yyyyMMdd}-{consecutivo:D4}";
        }

        private async Task CargarDetallesFactura(Factura factura)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                "SELECT * FROM Detalles_Factura WHERE Factura_ID = @Factura_ID",
                connection);
            command.Parameters.AddWithValue("@Factura_ID", factura.Factura_ID);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                factura.DetallesFactura.Add(new DetalleFactura
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
}