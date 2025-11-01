using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class DevolucionService : IDevolucionService
    {
        private readonly string _connectionString;

        public DevolucionService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
        }

        public async Task<Devolucion> AddDevolucionAsync(Devolucion devolucion)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                INSERT INTO Devoluciones
                (Venta_ID, FechaDevolucion, CantidadDevuelta, SubTotalDevuelto, TotalDevuelto, Motivo, TipoDevolucion, Activo)
                OUTPUT INSERTED.Devolucion_ID
                VALUES (@Venta_ID, GETDATE(), @CantidadDevuelta, @SubTotalDevuelto, @TotalDevuelto, @Motivo, @TipoDevolucion, 1);
            ";

            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Venta_ID", devolucion.Venta_ID);
            cmd.Parameters.AddWithValue("@CantidadDevuelta", devolucion.CantidadDevuelta);
            cmd.Parameters.AddWithValue("@SubTotalDevuelto", devolucion.SubTotalDevuelto);
            cmd.Parameters.AddWithValue("@TotalDevuelto", devolucion.TotalDevuelto);
            cmd.Parameters.AddWithValue("@Motivo", (object?)devolucion.Motivo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoDevolucion", devolucion.TipoDevolucion);

            devolucion.Devolucion_ID = Convert.ToInt32(await cmd.ExecuteScalarAsync());
            devolucion.FechaDevolucion = DateTime.Now;
            devolucion.Activo = true;

            return devolucion;
        }

        public async Task<List<Devolucion>> GetAllAsync()
        {
            var list = new List<Devolucion>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Devoluciones";
            var cmd = new SqlCommand(query, connection);

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                list.Add(new Devolucion
                {
                    Devolucion_ID = reader.GetInt32(0),
                    Venta_ID = reader.GetInt32(1),
                    FechaDevolucion = reader.GetDateTime(2),
                    CantidadDevuelta = reader.GetInt32(3),
                    SubTotalDevuelto = reader.GetDecimal(4),
                    TotalDevuelto = reader.GetDecimal(5),
                    Motivo = reader.IsDBNull(6) ? null : reader.GetString(6),
                    TipoDevolucion = reader.GetString(7),
                    Activo = reader.GetBoolean(8)
                });
            }

            return list;
        }

        public async Task<Devolucion> GetByIDAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "SELECT * FROM Devoluciones WHERE Devolucion_ID = @id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Devolucion
                {
                    Devolucion_ID = reader.GetInt32(0),
                    Venta_ID = reader.GetInt32(1),
                    FechaDevolucion = reader.GetDateTime(2),
                    CantidadDevuelta = reader.GetInt32(3),
                    SubTotalDevuelto = reader.GetDecimal(4),
                    TotalDevuelto = reader.GetDecimal(5),
                    Motivo = reader.IsDBNull(6) ? null : reader.GetString(6),
                    TipoDevolucion = reader.GetString(7),
                    Activo = reader.GetBoolean(8)
                };
            }

            throw new KeyNotFoundException("Devolución no encontrada.");
        }

        public async Task UpdateAsync(Devolucion devolucion)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                UPDATE Devoluciones
                SET Venta_ID = @Venta_ID,
                    CantidadDevuelta = @CantidadDevuelta,
                    SubTotalDevuelto = @SubTotalDevuelto,
                    TotalDevuelto = @TotalDevuelto,
                    Motivo = @Motivo,
                    TipoDevolucion = @TipoDevolucion,
                    Activo = @Activo
                WHERE Devolucion_ID = @Devolucion_ID;
            ";

            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@Devolucion_ID", devolucion.Devolucion_ID);
            cmd.Parameters.AddWithValue("@Venta_ID", devolucion.Venta_ID);
            cmd.Parameters.AddWithValue("@CantidadDevuelta", devolucion.CantidadDevuelta);
            cmd.Parameters.AddWithValue("@SubTotalDevuelto", devolucion.SubTotalDevuelto);
            cmd.Parameters.AddWithValue("@TotalDevuelto", devolucion.TotalDevuelto);
            cmd.Parameters.AddWithValue("@Motivo", (object?)devolucion.Motivo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TipoDevolucion", devolucion.TipoDevolucion);
            cmd.Parameters.AddWithValue("@Activo", devolucion.Activo);

            if (await cmd.ExecuteNonQueryAsync() == 0)
                throw new KeyNotFoundException("No existe una devolución con el ID especificado.");
        }

        public async Task DeleteAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = "DELETE FROM Devoluciones WHERE Devolucion_ID = @id";
            var cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", id);

            if (await cmd.ExecuteNonQueryAsync() == 0)
                throw new KeyNotFoundException("Devolución no encontrada.");
        }
    }
}