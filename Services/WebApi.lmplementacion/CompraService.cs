using WebApi.Interfaz;
using WebApi.Modelo;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;

namespace WebApi.Implementacion
{
    public class CompraService : ICompraService
    {
        private readonly string _connectionString;
        private readonly IMovimientoInventarioService _movimientoInventarioService;

        public CompraService(IConfiguration configuration, IMovimientoInventarioService movimientoInventarioService)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
            _movimientoInventarioService = movimientoInventarioService;
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
            }
        }

        public async Task<List<Compras>> GetAllAsync()
        {
            var compras = new List<Compras>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Compras", connection);
                connection.Open();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var compra = new Compras
                        {
                            Compra_ID = reader.IsDBNull(reader.GetOrdinal("Compra_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Compra_ID")),
                            Usuario_ID = reader.IsDBNull(reader.GetOrdinal("Usuario_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Proveedor_ID = reader.IsDBNull(reader.GetOrdinal("Proveedor_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Proveedor_ID")),
                            CantidadTotal = reader.IsDBNull(reader.GetOrdinal("CantidadTotal")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadTotal")),
                            MontoTotal = reader.IsDBNull(reader.GetOrdinal("MontoTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                            IVATotal = reader.IsDBNull(reader.GetOrdinal("IVATotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("IVATotal")),
                            SubTotal = reader.IsDBNull(reader.GetOrdinal("SubTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Total")),
                            FechaRegistro = reader.IsDBNull(reader.GetOrdinal("FechaRegistro")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                            DetallesCompra = new List<DetalleCompra>()
                        };


                        using (var detallesConnection = new SqlConnection(_connectionString))
                        {
                            await detallesConnection.OpenAsync();
                            var detallesCommand = new SqlCommand("SELECT * FROM DetallesCompra WHERE Compra_ID = @Compra_ID", detallesConnection);
                            detallesCommand.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);

                            using (var detallesReader = await detallesCommand.ExecuteReaderAsync())
                            {
                                while (await detallesReader.ReadAsync())
                                {
                                    var detalle = new DetalleCompra
                                    {
                                        Compra_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("Compra_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("Compra_ID")),
                                        Producto_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("Producto_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("Producto_ID")),
                                        CantidadUnitaria = detallesReader.IsDBNull(detallesReader.GetOrdinal("CantidadUnitaria")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("CantidadUnitaria")),
                                        MontoUnitario = detallesReader.IsDBNull(detallesReader.GetOrdinal("MontoUnitario")) ? 0 : detallesReader.GetDecimal(detallesReader.GetOrdinal("MontoUnitario")),
                                        IVA = detallesReader.IsDBNull(detallesReader.GetOrdinal("IVA")) ? 0 : detallesReader.GetDecimal(detallesReader.GetOrdinal("IVA")),
                                        Total = detallesReader.IsDBNull(detallesReader.GetOrdinal("Total")) ? 0 : detallesReader.GetDecimal(detallesReader.GetOrdinal("Total"))
                                    };
                                    compra.DetallesCompra.Add(detalle);
                                }
                            }
                        }

                        compras.Add(compra);
                    }
                }
            }

            return compras;
        }



        public async Task<Compras> GetByIDAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Compras WHERE Compra_ID = @Compra_ID", connection);
                command.Parameters.AddWithValue("@Compra_ID", id);
                connection.Open();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        var compra = new Compras
                        {
                            Compra_ID = reader.IsDBNull(reader.GetOrdinal("Compra_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Compra_ID")),
                            Usuario_ID = reader.IsDBNull(reader.GetOrdinal("Usuario_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Proveedor_ID = reader.IsDBNull(reader.GetOrdinal("Proveedor_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Proveedor_ID")),
                            CantidadTotal = reader.IsDBNull(reader.GetOrdinal("CantidadTotal")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadTotal")),
                            MontoTotal = reader.IsDBNull(reader.GetOrdinal("MontoTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                            IVATotal = reader.IsDBNull(reader.GetOrdinal("IVATotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("IVATotal")),
                            SubTotal = reader.IsDBNull(reader.GetOrdinal("SubTotal")) ? 0 : reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0 : reader.GetDecimal(reader.GetOrdinal("Total")),
                            FechaRegistro = reader.IsDBNull(reader.GetOrdinal("FechaRegistro")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                            DetallesCompra = new List<DetalleCompra>()
                        };

                        using (var detallesConnection = new SqlConnection(_connectionString))
                        {
                            await detallesConnection.OpenAsync();
                            var detallesCommand = new SqlCommand("SELECT * FROM DetallesCompra WHERE Compra_ID = @Compra_ID", detallesConnection);
                            detallesCommand.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);

                            using (var detallesReader = await detallesCommand.ExecuteReaderAsync())
                            {
                                while (await detallesReader.ReadAsync())
                                {
                                    var detalle = new DetalleCompra
                                    {
                                        DetalleCompra_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("DetalleCompra_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("DetalleCompra_ID")),
                                        Compra_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("Compra_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("Compra_ID")),
                                        Producto_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("Producto_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("Producto_ID")),
                                        CantidadUnitaria = detallesReader.IsDBNull(detallesReader.GetOrdinal("CantidadUnitaria")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("CantidadUnitaria")),
                                        MontoUnitario = detallesReader.IsDBNull(detallesReader.GetOrdinal("MontoUnitario")) ? 0 : detallesReader.GetDecimal(detallesReader.GetOrdinal("MontoUnitario")),
                                        IVA = detallesReader.IsDBNull(detallesReader.GetOrdinal("IVA")) ? 0 : detallesReader.GetDecimal(detallesReader.GetOrdinal("IVA")),
                                        Total = detallesReader.IsDBNull(detallesReader.GetOrdinal("Total")) ? 0 : detallesReader.GetDecimal(detallesReader.GetOrdinal("Total"))
                                    };

                                    compra.DetallesCompra.Add(detalle);
                                }
                            }
                        }

                        return compra;
                    }
                }
            }

            throw new KeyNotFoundException("No se encontró la compra con el ID especificado.");
        }



        public async Task<List<Compras>> GetByUsuarioAsync(int Usuario_ID)
        {
            var compras = new List<Compras>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Compras WHERE Usuario_ID = @Usuario_ID", connection);
                command.Parameters.AddWithValue("@Usuario_ID", Usuario_ID);

                await connection.OpenAsync();

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var compra = new Compras
                        {
                            Compra_ID = reader.IsDBNull(reader.GetOrdinal("Compra_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Compra_ID")),
                            Usuario_ID = reader.IsDBNull(reader.GetOrdinal("Usuario_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Usuario_ID")),
                            Proveedor_ID = reader.IsDBNull(reader.GetOrdinal("Proveedor_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("Proveedor_ID")),
                            CantidadTotal = reader.IsDBNull(reader.GetOrdinal("CantidadTotal")) ? 0 : reader.GetInt32(reader.GetOrdinal("CantidadTotal")),
                            MontoTotal = reader.IsDBNull(reader.GetOrdinal("MontoTotal")) ? 0m : reader.GetDecimal(reader.GetOrdinal("MontoTotal")),
                            FechaRegistro = reader.IsDBNull(reader.GetOrdinal("FechaRegistro")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("FechaRegistro")),
                            IVATotal = reader.IsDBNull(reader.GetOrdinal("IVATotal")) ? 0m : reader.GetDecimal(reader.GetOrdinal("IVATotal")),
                            SubTotal = reader.IsDBNull(reader.GetOrdinal("SubTotal")) ? 0m : reader.GetDecimal(reader.GetOrdinal("SubTotal")),
                            Total = reader.IsDBNull(reader.GetOrdinal("Total")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Total")),
                            DetallesCompra = new List<DetalleCompra>()
                        };

                        using (var detallesConnection = new SqlConnection(_connectionString))
                        {
                            await detallesConnection.OpenAsync();
                            var detallesCommand = new SqlCommand("SELECT * FROM DetallesCompra WHERE Compra_ID = @Compra_ID", detallesConnection);
                            detallesCommand.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);

                            using (var detallesReader = await detallesCommand.ExecuteReaderAsync())
                            {
                                while (await detallesReader.ReadAsync())
                                {
                                    var detalle = new DetalleCompra
                                    {
                                        DetalleCompra_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("DetalleCompra_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("DetalleCompra_ID")),
                                        Compra_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("Compra_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("Compra_ID")),
                                        Producto_ID = detallesReader.IsDBNull(detallesReader.GetOrdinal("Producto_ID")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("Producto_ID")),
                                        CantidadUnitaria = detallesReader.IsDBNull(detallesReader.GetOrdinal("CantidadUnitaria")) ? 0 : detallesReader.GetInt32(detallesReader.GetOrdinal("CantidadUnitaria")),
                                        MontoUnitario = detallesReader.IsDBNull(detallesReader.GetOrdinal("MontoUnitario")) ? 0m : detallesReader.GetDecimal(detallesReader.GetOrdinal("MontoUnitario")),
                                        IVA = detallesReader.IsDBNull(detallesReader.GetOrdinal("IVA")) ? 0m : detallesReader.GetDecimal(detallesReader.GetOrdinal("IVA")),
                                        Total = detallesReader.IsDBNull(detallesReader.GetOrdinal("Total")) ? 0m : detallesReader.GetDecimal(detallesReader.GetOrdinal("Total"))
                                    };
                                    compra.DetallesCompra.Add(detalle);
                                }
                            }
                        }

                        compras.Add(compra);
                    }
                }
            }

            return compras;
        }


        public async Task DeleteAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var transaction = connection.BeginTransaction();

                try
                {
                    var deleteMov = new SqlCommand("DELETE FROM MovimientoInventario WHERE Referencia_ID = @Compra_ID AND TipoReferencia = 'Compra'", connection, transaction);
                    deleteMov.Parameters.AddWithValue("@Compra_ID", id);
                    await deleteMov.ExecuteNonQueryAsync();

                    var deleteDetallesCommand = new SqlCommand("DELETE FROM DetallesCompra WHERE Compra_ID = @Compra_ID", connection, transaction);
                    deleteDetallesCommand.Parameters.AddWithValue("@Compra_ID", id);
                    await deleteDetallesCommand.ExecuteNonQueryAsync();

                    var deleteCompraCommand = new SqlCommand("DELETE FROM Compras WHERE Compra_ID = @Compra_ID", connection, transaction);
                    deleteCompraCommand.Parameters.AddWithValue("@Compra_ID", id);
                    await deleteCompraCommand.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var errorMessage = $"Error al eliminar la compra: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
                    }
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
        }

        public async Task<Compras> AddCompraConDetallesAsync(Compras compra, int Usuario_IDAutenticado)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();

                try
                {
                    if (compra.FechaRegistro == DateTime.MinValue)
                    {
                        compra.FechaRegistro = DateTime.Now;
                    }

                    if (compra.FechaRegistro < new DateTime(1753, 1, 1) || compra.FechaRegistro > new DateTime(9999, 12, 31))
                    {
                        throw new ArgumentOutOfRangeException("FechaRegistro", "La fecha de registro debe estar entre el 1/1/1753 y el 31/12/9999.");
                    }

                    int cantidadTotal = 0;
                    decimal subTotalGeneral = 0;
                    decimal ivaTotal = 0;

                    var commandCompra = new SqlCommand(
                        "INSERT INTO Compras (Usuario_ID, Proveedor_ID, FechaRegistro, CantidadTotal, SubTotal, IVATotal, MontoTotal, Total) " +
                        "VALUES (@Usuario_ID, @Proveedor_ID, @FechaRegistro, 0, 0, 0, 0, 0);" +
                        "SELECT SCOPE_IDENTITY();",
                        connection, transaction);

                    commandCompra.Parameters.AddWithValue("@Usuario_ID", Usuario_IDAutenticado);
                    commandCompra.Parameters.AddWithValue("@Proveedor_ID", compra.Proveedor_ID);
                    commandCompra.Parameters.AddWithValue("@FechaRegistro", compra.FechaRegistro);

                    var insertedCompra_ID = await commandCompra.ExecuteScalarAsync();
                    compra.Compra_ID = Convert.ToInt32(insertedCompra_ID);

                    foreach (var detalle in compra.DetallesCompra)
                    {
                        int cantidad = detalle.CantidadUnitaria;
                        decimal montoUnitario = detalle.MontoUnitario;
                        decimal iva = detalle.IVA;
                        decimal totalDetalle = (cantidad * montoUnitario) + iva;

                        detalle.Compra_ID = compra.Compra_ID;
                        detalle.Total = totalDetalle;

                        cantidadTotal += cantidad;
                        subTotalGeneral += cantidad * montoUnitario;
                        ivaTotal += iva;

                        var commandDetalle = new SqlCommand(
                            "INSERT INTO DetallesCompra (Compra_ID, Producto_ID, CantidadUnitaria, MontoUnitario, IVA, Total) " +
                            "VALUES (@Compra_ID, @Producto_ID, @CantidadUnitaria, @MontoUnitario, @IVA, @Total);",
                            connection, transaction);

                        commandDetalle.Parameters.AddWithValue("@Compra_ID", detalle.Compra_ID);
                        commandDetalle.Parameters.AddWithValue("@Producto_ID", detalle.Producto_ID);
                        commandDetalle.Parameters.AddWithValue("@CantidadUnitaria", cantidad);
                        commandDetalle.Parameters.AddWithValue("@MontoUnitario", montoUnitario);
                        commandDetalle.Parameters.AddWithValue("@IVA", iva);
                        commandDetalle.Parameters.AddWithValue("@Total", totalDetalle);

                        await commandDetalle.ExecuteNonQueryAsync();

                        await _movimientoInventarioService.RegistrarMovimiento(new MovimientoInventario
                        {
                            Producto_ID = detalle.Producto_ID,
                            TipoMovimiento = "Entrada",
                            Cantidad = detalle.CantidadUnitaria,
                            Precio = detalle.MontoUnitario,
                            Referencia_ID = compra.Compra_ID,
                            TipoReferencia = "Compra",
                            FechaMovimiento = compra.FechaRegistro
                        });
                    }

                    decimal montoTotal = subTotalGeneral + ivaTotal;

                    var commandUpdateCompra = new SqlCommand(
                        "UPDATE Compras SET CantidadTotal = @CantidadTotal, SubTotal = @SubTotal, IVATotal = @IVATotal, MontoTotal = @MontoTotal, Total = @Total " +
                        "WHERE Compra_ID = @Compra_ID", connection, transaction);

                    commandUpdateCompra.Parameters.AddWithValue("@CantidadTotal", cantidadTotal);
                    commandUpdateCompra.Parameters.AddWithValue("@SubTotal", subTotalGeneral);
                    commandUpdateCompra.Parameters.AddWithValue("@IVATotal", ivaTotal);
                    commandUpdateCompra.Parameters.AddWithValue("@MontoTotal", montoTotal);
                    commandUpdateCompra.Parameters.AddWithValue("@Total", montoTotal);
                    commandUpdateCompra.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);

                    await commandUpdateCompra.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();

                    compra.CantidadTotal = cantidadTotal;
                    compra.SubTotal = subTotalGeneral;
                    compra.IVATotal = ivaTotal;
                    compra.MontoTotal = montoTotal;
                    compra.Total = montoTotal;

                    return compra;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var errorMessage = $"Error al agregar la compra y sus detalles: {ex.Message}";

                    if (ex.InnerException != null)
                    {
                        errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
                    }

                    Console.WriteLine(errorMessage);
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
        }


        public async Task<Compras> UpdateAsync(Compras compra)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var transaction = connection.BeginTransaction();

                try
                {
                    var commandUpdateCompra = new SqlCommand(
                        "UPDATE Compras SET Usuario_ID = @Usuario_ID, Proveedor_ID = @Proveedor_ID, FechaRegistro = @FechaRegistro " +
                        "WHERE Compra_ID = @Compra_ID;",
                        connection, transaction);

                    commandUpdateCompra.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);
                    commandUpdateCompra.Parameters.AddWithValue("@Usuario_ID", compra.Usuario_ID);
                    commandUpdateCompra.Parameters.AddWithValue("@Proveedor_ID", compra.Proveedor_ID);
                    commandUpdateCompra.Parameters.AddWithValue("@FechaRegistro", compra.FechaRegistro);
                    await commandUpdateCompra.ExecuteNonQueryAsync();

                    var deleteDetalles = new SqlCommand("DELETE FROM DetallesCompra WHERE Compra_ID = @Compra_ID", connection, transaction);
                    deleteDetalles.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);
                    await deleteDetalles.ExecuteNonQueryAsync();

                    var deleteMovimientos = new SqlCommand("DELETE FROM MovimientoInventario WHERE Referencia_ID = @Compra_ID AND TipoReferencia = 'Compra'", connection, transaction);
                    deleteMovimientos.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);
                    await deleteMovimientos.ExecuteNonQueryAsync();

                    int cantidadTotal = 0;
                    decimal subTotalGeneral = 0;
                    decimal ivaTotal = 0;

                    foreach (var detalle in compra.DetallesCompra)
                    {
                        int cantidad = detalle.CantidadUnitaria;
                        decimal montoUnitario = detalle.MontoUnitario;
                        decimal iva = detalle.IVA;
                        decimal totalDetalle = (cantidad * montoUnitario) + iva;

                        detalle.Compra_ID = compra.Compra_ID;
                        detalle.Total = totalDetalle;

                        cantidadTotal += cantidad;
                        subTotalGeneral += cantidad * montoUnitario;
                        ivaTotal += iva;

                        var cmdDetalle = new SqlCommand(
                            "INSERT INTO DetallesCompra (Compra_ID, Producto_ID, CantidadUnitaria, MontoUnitario, IVA, Total) " +
                            "VALUES (@Compra_ID, @Producto_ID, @CantidadUnitaria, @MontoUnitario, @IVA, @Total);",
                            connection, transaction);

                        cmdDetalle.Parameters.AddWithValue("@Compra_ID", detalle.Compra_ID);
                        cmdDetalle.Parameters.AddWithValue("@Producto_ID", detalle.Producto_ID);
                        cmdDetalle.Parameters.AddWithValue("@CantidadUnitaria", cantidad);
                        cmdDetalle.Parameters.AddWithValue("@MontoUnitario", montoUnitario);
                        cmdDetalle.Parameters.AddWithValue("@IVA", iva);
                        cmdDetalle.Parameters.AddWithValue("@Total", totalDetalle);
                        await cmdDetalle.ExecuteNonQueryAsync();

 

                        var cmdMovimiento = new SqlCommand(
                            "INSERT INTO MovimientoInventario (Producto_ID, TipoMovimiento, Cantidad, Precio, Referencia_ID, TipoReferencia, FechaMovimiento) " +
                            "VALUES (@Producto_ID, @TipoMovimiento, @Cantidad, @Precio, @Referencia_ID, @TipoReferencia, @FechaMovimiento);",
                            connection, transaction);

                        cmdMovimiento.Parameters.AddWithValue("@Producto_ID", detalle.Producto_ID);
                        cmdMovimiento.Parameters.AddWithValue("@TipoMovimiento", "Entrada");
                        cmdMovimiento.Parameters.AddWithValue("@Cantidad", cantidad);
                        cmdMovimiento.Parameters.AddWithValue("@Precio", montoUnitario);
                        cmdMovimiento.Parameters.AddWithValue("@Referencia_ID", compra.Compra_ID);
                        cmdMovimiento.Parameters.AddWithValue("@TipoReferencia", "Compra");
                        cmdMovimiento.Parameters.AddWithValue("@FechaMovimiento", compra.FechaRegistro);

                        await cmdMovimiento.ExecuteNonQueryAsync();
                    }

                    decimal montoTotal = subTotalGeneral + ivaTotal;

                    var updateTotales = new SqlCommand(
                        "UPDATE Compras SET CantidadTotal = @CantidadTotal, SubTotal = @SubTotal, IVATotal = @IVATotal, MontoTotal = @MontoTotal, Total = @Total " +
                        "WHERE Compra_ID = @Compra_ID",
                        connection, transaction);

                    updateTotales.Parameters.AddWithValue("@CantidadTotal", cantidadTotal);
                    updateTotales.Parameters.AddWithValue("@SubTotal", subTotalGeneral);
                    updateTotales.Parameters.AddWithValue("@IVATotal", ivaTotal);
                    updateTotales.Parameters.AddWithValue("@MontoTotal", montoTotal);
                    updateTotales.Parameters.AddWithValue("@Total", montoTotal);
                    updateTotales.Parameters.AddWithValue("@Compra_ID", compra.Compra_ID);
                    await updateTotales.ExecuteNonQueryAsync();

                    await transaction.CommitAsync();

                    compra.CantidadTotal = cantidadTotal;
                    compra.SubTotal = subTotalGeneral;
                    compra.IVATotal = ivaTotal;
                    compra.MontoTotal = montoTotal;
                    compra.Total = montoTotal;

                    return compra;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    var errorMessage = $"Error al actualizar la compra y sus detalles: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMessage += $" | Inner Exception: {ex.InnerException.Message}";
                    }

                    Console.WriteLine(errorMessage);
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
        }
    }
}