using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly string _connectionString;

        public VentaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("Cadena de conexión no configurada.");
        }

        public async Task<List<Venta>> GetAllAsync()
        {
            var ventas = new List<Venta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    SELECT * FROM Ventas 
                    ORDER BY FechaVenta DESC",
                    connection);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var venta = new Venta
                        {
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Usuario_ID = reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Cliente_ID = reader.IsDBNull(reader.GetOrdinal("Cliente_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            FechaVenta = reader.GetDateTime(reader.GetOrdinal("FechaVenta")),
                            CantidadTotal = reader.GetInt32(reader.GetOrdinal("CantidadTotal")),
                            MontoRecibido = reader.GetDecimal(reader.GetOrdinal("MontoRecibido")),
                            MontoDevuelto = reader.GetDecimal(reader.GetOrdinal("MontoDevuelto")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesVenta = new List<DetalleVenta>()
                        };

                        await CargarDetallesVenta(venta);
                        ventas.Add(venta);
                    }
                }
            }

            return ventas;
        }

        public async Task<Venta> GetByIDAsync(int idVenta)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Ventas WHERE Venta_ID = @Venta_ID",
                    connection);
                command.Parameters.AddWithValue("@Venta_ID", idVenta);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var venta = new Venta
                        {
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Usuario_ID = reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Cliente_ID = reader.IsDBNull(reader.GetOrdinal("Cliente_ID")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            FechaVenta = reader.GetDateTime(reader.GetOrdinal("FechaVenta")),
                            CantidadTotal = reader.GetInt32(reader.GetOrdinal("CantidadTotal")),
                            MontoRecibido = reader.GetDecimal(reader.GetOrdinal("MontoRecibido")),
                            MontoDevuelto = reader.GetDecimal(reader.GetOrdinal("MontoDevuelto")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesVenta = new List<DetalleVenta>()
                        };

                        await CargarDetallesVenta(venta);
                        return venta;
                    }
                }
            }

            throw new KeyNotFoundException("Venta no encontrada.");
        }

        public async Task<List<Venta>> GetByClienteAsync(int idCliente)
        {
            var ventas = new List<Venta>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "SELECT * FROM Ventas WHERE Cliente_ID = @Cliente_ID",
                    connection);
                command.Parameters.AddWithValue("@Cliente_ID", idCliente);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var venta = new Venta
                        {
                            Venta_ID = reader.GetInt32(reader.GetOrdinal("Venta_ID")),
                            Usuario_ID = reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Cliente_ID = reader.GetInt32(reader.GetOrdinal("Cliente_ID")),
                            FechaVenta = reader.GetDateTime(reader.GetOrdinal("FechaVenta")),
                            CantidadTotal = reader.GetInt32(reader.GetOrdinal("CantidadTotal")),
                            MontoRecibido = reader.GetDecimal(reader.GetOrdinal("MontoRecibido")),
                            MontoDevuelto = reader.GetDecimal(reader.GetOrdinal("MontoDevuelto")),
                            SubTotal = reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            Descuento = reader.GetDecimal(reader.GetOrdinal("Descuento")),
                            IVA = reader.GetDecimal(reader.GetOrdinal("IVA")),
                            Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                            Estado = reader.GetString(reader.GetOrdinal("Estado")),
                            DetallesVenta = new List<DetalleVenta>()
                        };

                        await CargarDetallesVenta(venta);
                        ventas.Add(venta);
                    }
                }
            }

            return ventas;
        }

        public async Task<Venta> AddVentaConDetallesAsync(Venta venta, int idUsuarioAutenticado)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                var checkVentaCommand = new SqlCommand(@"
                    SELECT COUNT(*) FROM Ventas
                    WHERE Usuario_ID = @Usuario_ID 
                    AND ((@Cliente_ID IS NULL AND Cliente_ID IS NULL) OR Cliente_ID = @Cliente_ID)
                    AND MontoRecibido = @MontoRecibido
                    AND FechaVenta >= DATEADD(SECOND, -10, GETDATE())",
                    connection, transaction);

                checkVentaCommand.Parameters.AddWithValue("@Usuario_ID", idUsuarioAutenticado);
                checkVentaCommand.Parameters.AddWithValue("@Cliente_ID", venta.Cliente_ID ?? (object)DBNull.Value);
                checkVentaCommand.Parameters.AddWithValue("@MontoRecibido", venta.MontoRecibido);

                var existe = (int)await checkVentaCommand.ExecuteScalarAsync();
                if (existe > 0)
                    throw new InvalidOperationException("Ya se ha registrado una venta similar recientemente.");

                foreach (var detalle in venta.DetallesVenta)
                {
                    var stockDisponible = await ObtenerStockDisponible(detalle.Producto_ID, connection, transaction);
                    if (stockDisponible < detalle.Cantidad)
                        throw new InvalidOperationException($"Stock insuficiente para el producto ID {detalle.Producto_ID}. Disponible: {stockDisponible}, Solicitado: {detalle.Cantidad}");
                }

                var insertVentaCmd = new SqlCommand(@"
                    INSERT INTO Ventas (
                        Usuario_ID, Cliente_ID, FechaVenta, CantidadTotal, MontoRecibido, MontoDevuelto,
                        SubTotal, Descuento, IVA, Total, Estado
                    ) 
                    OUTPUT INSERTED.Venta_ID
                    VALUES (
                        @Usuario_ID, @Cliente_ID, GETDATE(), @CantidadTotal, @MontoRecibido, @MontoDevuelto,
                        @SubTotal, @Descuento, @IVA, @Total, @Estado
                    )",
                    connection, transaction);

                insertVentaCmd.Parameters.AddWithValue("@Usuario_ID", idUsuarioAutenticado);
                insertVentaCmd.Parameters.AddWithValue("@Cliente_ID", venta.Cliente_ID ?? (object)DBNull.Value);
                insertVentaCmd.Parameters.AddWithValue("@CantidadTotal", venta.CantidadTotal);
                insertVentaCmd.Parameters.AddWithValue("@MontoRecibido", venta.MontoRecibido);
                insertVentaCmd.Parameters.AddWithValue("@MontoDevuelto", venta.MontoDevuelto);
                insertVentaCmd.Parameters.AddWithValue("@SubTotal", venta.SubTotal);
                insertVentaCmd.Parameters.AddWithValue("@Descuento", venta.Descuento);
                insertVentaCmd.Parameters.AddWithValue("@IVA", venta.IVA);
                insertVentaCmd.Parameters.AddWithValue("@Total", venta.Total);
                insertVentaCmd.Parameters.AddWithValue("@Estado", "Activo");

                venta.Venta_ID = Convert.ToInt32(await insertVentaCmd.ExecuteScalarAsync());

                foreach (var detalle in venta.DetallesVenta)
                {
                    detalle.Venta_ID = venta.Venta_ID;

                    var insertDetalleCmd = new SqlCommand(@"
                        INSERT INTO Detalles_Ventas (
                            Venta_ID, Producto_ID, Cantidad, PrecioUnitario,
                            SubTotal, IVA, Total, TipoComprobante
                        ) 
                        VALUES (
                            @Venta_ID, @Producto_ID, @Cantidad, @PrecioUnitario,
                            @SubTotal, @IVA, @Total, @TipoComprobante
                        )",
                        connection, transaction);

                    insertDetalleCmd.Parameters.AddWithValue("@Venta_ID", detalle.Venta_ID);
                    insertDetalleCmd.Parameters.AddWithValue("@Producto_ID", detalle.Producto_ID);
                    insertDetalleCmd.Parameters.AddWithValue("@Cantidad", detalle.Cantidad);
                    insertDetalleCmd.Parameters.AddWithValue("@PrecioUnitario", detalle.PrecioUnitario);
                    insertDetalleCmd.Parameters.AddWithValue("@SubTotal", detalle.SubTotal);
                    insertDetalleCmd.Parameters.AddWithValue("@IVA", detalle.IVA);
                    insertDetalleCmd.Parameters.AddWithValue("@Total", detalle.Total);
                    insertDetalleCmd.Parameters.AddWithValue("@TipoComprobante", detalle.TipoComprobante);

                    await insertDetalleCmd.ExecuteNonQueryAsync();

                    await RegistrarMovimientoInventario(
                        detalle.Producto_ID,
                        detalle.Cantidad,
                        detalle.PrecioUnitario,
                        venta.Venta_ID,
                        "Venta",
                        connection,
                        transaction
                    );
                }

                await transaction.CommitAsync();
                return await GetByIDAsync(venta.Venta_ID);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Venta> UpdateAsync(Venta venta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                var anularVentaCmd = new SqlCommand(
                    "UPDATE Ventas SET Estado = 'Anulado' WHERE Venta_ID = @Venta_ID",
                    connection, transaction);
                anularVentaCmd.Parameters.AddWithValue("@Venta_ID", venta.Venta_ID);
                await anularVentaCmd.ExecuteNonQueryAsync();

                var detallesOriginalesCmd = new SqlCommand(
                    "SELECT * FROM Detalles_Ventas WHERE Venta_ID = @Venta_ID",
                    connection, transaction);
                detallesOriginalesCmd.Parameters.AddWithValue("@Venta_ID", venta.Venta_ID);

                using (var reader = await detallesOriginalesCmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var productoId = reader.GetInt32(reader.GetOrdinal("Producto_ID"));
                        var cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"));
                        var precioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario"));

                        await RegistrarMovimientoInventario(
                            productoId,
                            cantidad,
                            precioUnitario,
                            venta.Venta_ID,
                            "AnulacionVenta",
                            connection,
                            transaction
                        );
                    }
                }

                var nuevaVenta = new Venta
                {
                    Usuario_ID = venta.Usuario_ID,
                    Cliente_ID = venta.Cliente_ID,
                    MontoRecibido = venta.MontoRecibido,
                    Descuento = venta.Descuento,
                    DetallesVenta = venta.DetallesVenta,
                    Estado = "Activo",
                    CantidadTotal = venta.CantidadTotal,
                    SubTotal = venta.SubTotal,
                    IVA = venta.IVA,
                    Total = venta.Total,
                    MontoDevuelto = venta.MontoDevuelto
                };

                var ventaActualizada = await AddVentaConDetallesAsync(nuevaVenta, venta.Usuario_ID);

                await transaction.CommitAsync();
                return ventaActualizada;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task DeleteAsync(int idVenta)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            var transaction = connection.BeginTransaction();

            try
            {
                var checkVentaCmd = new SqlCommand(
                    "SELECT Estado FROM Ventas WHERE Venta_ID = @Venta_ID",
                    connection, transaction);
                checkVentaCmd.Parameters.AddWithValue("@Venta_ID", idVenta);

                var estado = (string)await checkVentaCmd.ExecuteScalarAsync();
                if (estado == null)
                    throw new KeyNotFoundException("Venta no encontrada.");

                if (estado != "Anulado")
                {
                    var detallesCmd = new SqlCommand(
                        "SELECT * FROM Detalles_Ventas WHERE Venta_ID = @Venta_ID",
                        connection, transaction);
                    detallesCmd.Parameters.AddWithValue("@Venta_ID", idVenta);

                    using (var reader = await detallesCmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var productoId = reader.GetInt32(reader.GetOrdinal("Producto_ID"));
                            var cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad"));
                            var precioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario"));

                            await RegistrarMovimientoInventario(
                                productoId,
                                cantidad,
                                precioUnitario,
                                idVenta,
                                "Devolucion",
                                connection,
                                transaction
                            );
                        }
                    }
                }

                var deleteDetallesCmd = new SqlCommand(
                    "DELETE FROM Detalles_Ventas WHERE Venta_ID = @Venta_ID",
                    connection, transaction);
                deleteDetallesCmd.Parameters.AddWithValue("@Venta_ID", idVenta);
                await deleteDetallesCmd.ExecuteNonQueryAsync();

                var deleteMovimientosCmd = new SqlCommand(
                    "DELETE FROM MovimientoInventario WHERE Referencia_ID = @Venta_ID AND TipoReferencia IN ('Venta', 'AnulacionVenta', 'Devolucion')",
                    connection, transaction);
                deleteMovimientosCmd.Parameters.AddWithValue("@Venta_ID", idVenta);
                await deleteMovimientosCmd.ExecuteNonQueryAsync();

                var deleteVentaCmd = new SqlCommand(
                    "DELETE FROM Ventas WHERE Venta_ID = @Venta_ID",
                    connection, transaction);
                deleteVentaCmd.Parameters.AddWithValue("@Venta_ID", idVenta);
                await deleteVentaCmd.ExecuteNonQueryAsync();

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<decimal> ObtenerPrecioUnitarioFIFO(int idProducto)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var command = new SqlCommand(@"
                SELECT TOP 1 pp.PrecioVenta  -- Cambiado para usar PrecioProducto
                FROM MovimientoInventario mi
                INNER JOIN PrecioProducto pp ON mi.PrecioProducto_ID = pp.Precio_ID
                WHERE mi.Producto_ID = @Producto_ID 
                AND mi.TipoMovimiento = 'Entrada' 
                AND mi.EstadoMovimiento = 'Activo'
                ORDER BY mi.FechaMovimiento ASC", 
                connection);

            command.Parameters.AddWithValue("@Producto_ID", idProducto);

            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0m;
        }

        private async Task CargarDetallesVenta(Venta venta)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(
                "SELECT * FROM Detalles_Ventas WHERE Venta_ID = @Venta_ID",
                connection);
            command.Parameters.AddWithValue("@Venta_ID", venta.Venta_ID);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                venta.DetallesVenta.Add(new DetalleVenta
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

        private async Task<int> ObtenerStockDisponible(int idProducto, SqlConnection connection, SqlTransaction transaction)
        {
            var command = new SqlCommand(@"
                SELECT SUM(CASE WHEN TipoMovimiento = 'Entrada' AND EstadoMovimiento = 'Activo' 
                           THEN Cantidad ELSE 0 END) - 
                       SUM(CASE WHEN TipoMovimiento = 'Salida' THEN Cantidad ELSE 0 END) 
                FROM MovimientoInventario
                WHERE Producto_ID = @Producto_ID",
                connection, transaction);

            command.Parameters.AddWithValue("@Producto_ID", idProducto);
            var result = await command.ExecuteScalarAsync();
            return result != DBNull.Value ? Convert.ToInt32(result) : 0;
        }

        private async Task RegistrarMovimientoInventario(int productoId, int cantidad, decimal precio, int referenciaId, string tipoReferencia, SqlConnection connection, SqlTransaction transaction)
        {
        var precioProductoId = await ObtenerPrecioProductoIdActivo(productoId, connection, transaction);

        var movimientoCmd = new SqlCommand(@"
        INSERT INTO MovimientoInventario (
            Producto_ID, PrecioProducto_ID, TipoMovimiento, Cantidad, Precio,
            Referencia_ID, TipoReferencia, FechaMovimiento, EstadoMovimiento
        ) 
        VALUES (
            @Producto_ID, @PrecioProducto_ID, @TipoMovimiento, @Cantidad, @Precio,
            @Referencia_ID, @TipoReferencia, GETDATE(), 'Activo'
        )",
        connection, transaction);

        movimientoCmd.Parameters.AddWithValue("@Producto_ID", productoId);
        movimientoCmd.Parameters.AddWithValue("@PrecioProducto_ID", precioProductoId ?? (object)DBNull.Value); // ¡ESTA ES LA LÍNEA CLAVE!
        movimientoCmd.Parameters.AddWithValue("@TipoMovimiento", tipoReferencia == "Venta" ? "Salida" : "Entrada");
        movimientoCmd.Parameters.AddWithValue("@Cantidad", cantidad);
        movimientoCmd.Parameters.AddWithValue("@Precio", precio);
        movimientoCmd.Parameters.AddWithValue("@Referencia_ID", referenciaId);
        movimientoCmd.Parameters.AddWithValue("@TipoReferencia", tipoReferencia);

        await movimientoCmd.ExecuteNonQueryAsync();
        }

        private async Task<int?> ObtenerPrecioProductoIdActivo(int productoId, SqlConnection connection, SqlTransaction transaction)
        {
            var command = new SqlCommand(@"
                SELECT Precio_ID 
                FROM PrecioProducto 
                WHERE Producto_ID = @Producto_ID AND Activo = 1",
                connection, transaction);

            command.Parameters.AddWithValue("@Producto_ID", productoId);

            var result = await command.ExecuteScalarAsync();
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : (int?)null;
        }
    }
}