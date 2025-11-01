﻿using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class MovimientoInventarioService : IMovimientoInventarioService
    {
        private readonly string _connectionString;

        public MovimientoInventarioService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<MovimientoInventario> RegistrarMovimiento(MovimientoInventario movimiento)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();
            using var transaction = connection.BeginTransaction();

            try
            {
                if (movimiento.TipoMovimiento == "Entrada")
                {
                    await InsertarMovimiento(connection, transaction, movimiento);
                }
                else if (movimiento.TipoMovimiento == "Salida")
                {
                    int cantidadRestante = movimiento.Cantidad;

                    var entradaCmd = new SqlCommand(@"
                        SELECT MovimientoInventario_ID, Cantidad, Precio
                        FROM MovimientoInventario
                        WHERE Producto_ID = @Producto_ID AND TipoMovimiento = 'Entrada'
                        ORDER BY FechaMovimiento", connection, transaction);
                    entradaCmd.Parameters.AddWithValue("@Producto_ID", movimiento.Producto_ID);

                    var entradas = new List<MovimientoInventario>();
                    using var reader = await entradaCmd.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        entradas.Add(new MovimientoInventario
                        {
                            MovimientoInventario_ID = reader.GetInt32(0),
                            Cantidad = reader.GetInt32(1),
                            Precio = reader.GetDecimal(2)
                        });
                    }
                    reader.Close();

                    foreach (var entrada in entradas)
                    {
                        int cantidadUsar = System.Math.Min(entrada.Cantidad, cantidadRestante);

                        var salida = new MovimientoInventario
                        {
                            Producto_ID = movimiento.Producto_ID,
                            TipoMovimiento = "Salida",
                            Cantidad = cantidadUsar,
                            Precio = entrada.Precio,
                            Referencia_ID = movimiento.Referencia_ID,
                            TipoReferencia = movimiento.TipoReferencia,
                            FechaMovimiento = movimiento.FechaMovimiento
                        };

                        await InsertarMovimiento(connection, transaction, salida);

                        cantidadRestante -= cantidadUsar;
                        if (cantidadRestante <= 0) break;
                    }

                    if (cantidadRestante > 0)
                        throw new System.Exception("Stock insuficiente para completar la salida.");
                }

                await transaction.CommitAsync();
                return movimiento;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        private async Task InsertarMovimiento(SqlConnection connection, SqlTransaction transaction, MovimientoInventario movimiento)
        {
            var cmd = new SqlCommand(@"
                INSERT INTO MovimientoInventario (
                    Producto_ID, TipoMovimiento, Cantidad, Precio, Referencia_ID, TipoReferencia, FechaMovimiento)
                VALUES (
                    @Producto_ID, @TipoMovimiento, @Cantidad, @Precio, @Referencia_ID, @TipoReferencia, @FechaMovimiento);
                SELECT SCOPE_IDENTITY();", connection, transaction);

            cmd.Parameters.AddWithValue("@Producto_ID", movimiento.Producto_ID);
            cmd.Parameters.AddWithValue("@TipoMovimiento", movimiento.TipoMovimiento);
            cmd.Parameters.AddWithValue("@Cantidad", movimiento.Cantidad);
            cmd.Parameters.AddWithValue("@Precio", movimiento.Precio);
            cmd.Parameters.AddWithValue("@Referencia_ID", movimiento.Referencia_ID);
            cmd.Parameters.AddWithValue("@TipoReferencia", movimiento.TipoReferencia);
            cmd.Parameters.AddWithValue("@FechaMovimiento", movimiento.FechaMovimiento);

            var insertedId = await cmd.ExecuteScalarAsync();
            movimiento.MovimientoInventario_ID = System.Convert.ToInt32(insertedId);
        }

        public IEnumerable<MovimientoInventario> GetAll()
        {
            var movimientos = new List<MovimientoInventario>();
            using var connection = new SqlConnection(_connectionString);
            var query = @"
                SELECT m.MovimientoInventario_ID, m.Producto_ID, p.NombreProducto, m.TipoMovimiento,
                       m.Cantidad, m.Precio, m.Referencia_ID,
                       m.TipoReferencia, m.FechaMovimiento
                FROM MovimientoInventario m
                INNER JOIN Productos p ON m.Producto_ID = p.Producto_ID
                ORDER BY m.FechaMovimiento DESC";

            var command = new SqlCommand(query, connection);

            connection.Open();
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                movimientos.Add(MapMovimiento(reader));
            }

            return movimientos;
        }

        public MovimientoInventario GetById(int id)
        {
            MovimientoInventario movimiento = null;
            using var connection = new SqlConnection(_connectionString);
            var query = "SELECT * FROM MovimientoInventario WHERE MovimientoInventario_ID = @ID";
            var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@ID", id);

            connection.Open();
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                movimiento = MapMovimiento(reader);
            }

            return movimiento;
        }

        public async Task<List<MovimientoInventario>> GetByProductoId(int idProducto)
        {
            var movimientos = new List<MovimientoInventario>();
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("sp_ConsultarMovimientosPorProducto", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Producto_ID", idProducto);

            await connection.OpenAsync();
            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                movimientos.Add(MapMovimiento(reader));
            }

            return movimientos;
        }

        public async Task<int> ObtenerStockActual(int idProducto)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("sp_ObtenerStockActual", connection)
            {
                CommandType = CommandType.StoredProcedure
            };
            command.Parameters.AddWithValue("@Producto_ID", idProducto);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && int.TryParse(result.ToString(), out int stock) ? stock : 0;
        }

        public void Delete(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand("DELETE FROM MovimientoInventario WHERE MovimientoInventario_ID = @ID", connection);
            command.Parameters.AddWithValue("@ID", id);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
                throw new KeyNotFoundException("Movimiento no encontrado.");
        }

        public void Update(int id, MovimientoInventario movimiento)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                UPDATE MovimientoInventario SET 
                    Producto_ID = @Producto_ID,
                    TipoMovimiento = @TipoMovimiento,
                    Cantidad = @Cantidad,
                    Precio = @Precio,
                    Referencia_ID = @Referencia_ID,
                    TipoReferencia = @TipoReferencia,
                    FechaMovimiento = @FechaMovimiento,
                WHERE MovimientoInventario_ID = @ID", connection);

            command.Parameters.AddWithValue("@ID", id);
            command.Parameters.AddWithValue("@Producto_ID", movimiento.Producto_ID);
            command.Parameters.AddWithValue("@TipoMovimiento", movimiento.TipoMovimiento);
            command.Parameters.AddWithValue("@Cantidad", movimiento.Cantidad);
            command.Parameters.AddWithValue("@Precio", movimiento.Precio);
            command.Parameters.AddWithValue("@Referencia_ID", movimiento.Referencia_ID);
            command.Parameters.AddWithValue("@TipoReferencia", movimiento.TipoReferencia);
            command.Parameters.AddWithValue("@FechaMovimiento", movimiento.FechaMovimiento);

            connection.Open();
            var rowsAffected = command.ExecuteNonQuery();
            if (rowsAffected == 0)
                throw new KeyNotFoundException("Movimiento no encontrado.");
        }

        private MovimientoInventario MapMovimiento(SqlDataReader reader)
        {
            return new MovimientoInventario
            {
                MovimientoInventario_ID = reader.GetInt32(reader.GetOrdinal("MovimientoInventario_ID")),
                Producto_ID = reader.GetInt32(reader.GetOrdinal("Producto_ID")),
                NombreProducto = reader.HasColumn("NombreProducto") ? reader.GetString(reader.GetOrdinal("NombreProducto")) : null,
                TipoMovimiento = reader.GetString(reader.GetOrdinal("TipoMovimiento")),
                Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                Precio = reader.GetDecimal(reader.GetOrdinal("Precio")),
                Referencia_ID = reader.GetInt32(reader.GetOrdinal("Referencia_ID")),
                TipoReferencia = reader.GetString(reader.GetOrdinal("TipoReferencia")),
                FechaMovimiento = reader.GetDateTime(reader.GetOrdinal("FechaMovimiento"))
            };
        }

        public async Task<decimal> ObtenerUltimoPrecioEntradaAsync(int idProducto)
        {
            using var connection = new SqlConnection(_connectionString);
            var command = new SqlCommand(@"
                SELECT TOP 1 Precio
                FROM MovimientoInventario
                WHERE Producto_ID = @Producto_ID AND TipoMovimiento = 'Entrada'
                ORDER BY FechaMovimiento DESC", connection);

            command.Parameters.AddWithValue("@Producto_ID", idProducto);

            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            return result != null && decimal.TryParse(result.ToString(), out var precio) ? precio : 0;
        }
    }

    internal static class SqlDataReaderExtensions
    {
        public static bool HasColumn(this SqlDataReader reader, string columnName)
        {
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}