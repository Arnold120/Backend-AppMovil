using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly string _connectionString;

        public ProductoService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection")
                ?? throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
        }

        public async Task<Producto> Registrar(Producto producto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    INSERT INTO Productos (Marca_ID, Categoria_ID, Codigo, NombreProducto, UnidadMedida, CapacidadUnidad, Cantidad, Activo, FechaRegistro)
                    OUTPUT INSERTED.Producto_ID 
                    VALUES (@Marca_ID, @Categoria_ID, @Codigo, @NombreProducto, @UnidadMedida, @CapacidadUnidad, @Cantidad, @Activo, @FechaRegistro)", connection);

                command.Parameters.AddWithValue("@Marca_ID", producto.Marca_ID);
                command.Parameters.AddWithValue("@Categoria_ID", producto.Categoria_ID);
                command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                command.Parameters.AddWithValue("@NombreProducto", producto.NombreProducto ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UnidadMedida", producto.UnidadMedida ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CapacidadUnidad", producto.CapacidadUnidad);
                command.Parameters.AddWithValue("@Cantidad", producto.Cantidad);
                command.Parameters.AddWithValue("@Activo", producto.Activo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaRegistro", producto.FechaRegistro ?? (object)DBNull.Value);

                await connection.OpenAsync();
                producto.Producto_ID = (int)await command.ExecuteScalarAsync();
            }

            return producto;
        }

        public IEnumerable<Producto> GetAll()
        {
            var productos = new List<Producto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Producto_ID, Marca_ID, Categoria_ID, Codigo, NombreProducto, UnidadMedida, CapacidadUnidad, Cantidad, Activo, FechaRegistro " +
                            "FROM Productos " +
                            "ORDER BY Producto_ID DESC";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            Producto_ID = reader.GetInt32(0),
                            Marca_ID = reader.GetInt32(1),
                            Categoria_ID = reader.GetInt32(2),
                            Codigo = reader.GetInt32(3),
                            NombreProducto = reader.IsDBNull(4) ? null : reader.GetString(4),
                            UnidadMedida = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CapacidadUnidad = reader.GetInt32(6),
                            Cantidad = reader.GetInt32(7),
                            Activo = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8),
                            FechaRegistro = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                        });
                    }
                }
            }

            return productos;
        }

        public Producto GetById(int id)
        {
            Producto? producto = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Producto_ID, Marca_ID, Categoria_ID, Codigo, NombreProducto, UnidadMedida, CapacidadUnidad, Cantidad, Activo, FechaRegistro " +
                            "FROM Productos " +
                            "WHERE Producto_ID = @Producto_ID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Producto_ID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        producto = new Producto
                        {
                            Producto_ID = reader.GetInt32(0),
                            Marca_ID = reader.GetInt32(1),
                            Categoria_ID = reader.GetInt32(2),
                            Codigo = reader.GetInt32(3),
                            NombreProducto = reader.IsDBNull(4) ? null : reader.GetString(4),
                            UnidadMedida = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CapacidadUnidad = reader.GetInt32(6),
                            Cantidad = reader.GetInt32(7),
                            Activo = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8),
                            FechaRegistro = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                        };
                    }
                }
            }

            return producto!;
        }

        public List<Producto> GetByNombre(string nombreProducto)
        {
            List<Producto> productos = new List<Producto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Producto_ID, Marca_ID, Categoria_ID, Codigo, NombreProducto, UnidadMedida, CapacidadUnidad, Cantidad, Activo, FechaRegistro " +
                            "FROM Productos " +
                            "WHERE NombreProducto LIKE @NombreProducto";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@NombreProducto", nombreProducto + "%");

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            Producto_ID = reader.GetInt32(0),
                            Marca_ID = reader.GetInt32(1),
                            Categoria_ID = reader.GetInt32(2),
                            Codigo = reader.GetInt32(3),
                            NombreProducto = reader.IsDBNull(4) ? null : reader.GetString(4),
                            UnidadMedida = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CapacidadUnidad = reader.GetInt32(6),
                            Cantidad = reader.GetInt32(7),
                            Activo = reader.IsDBNull(8) ? (bool?)null : reader.GetBoolean(8),
                            FechaRegistro = reader.IsDBNull(9) ? (DateTime?)null : reader.GetDateTime(9)
                        });
                    }
                }
            }

            return productos;
        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("UPDATE Productos SET Activo = 0 WHERE Producto_ID = @Producto_ID", connection);
                command.Parameters.AddWithValue("@Producto_ID", id);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new KeyNotFoundException("Producto no encontrado.");
            }
        }

        public void Update(int id, Producto producto)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(@"
                    UPDATE Productos 
                    SET Marca_ID = @Marca_ID, Categoria_ID = @Categoria_ID,Codigo = @Codigo, NombreProducto = @NombreProducto, UnidadMedida = @UnidadMedida, CapacidadUnidad = @CapacidadUnidad, Cantidad = @Cantidad, Activo = @Activo, FechaRegistro = @FechaRegistro 
                    WHERE Producto_ID = @Producto_ID", connection);

                command.Parameters.AddWithValue("@Producto_ID", id);
                command.Parameters.AddWithValue("@Marca_ID", producto.Marca_ID);
                command.Parameters.AddWithValue("@Categoria_ID", producto.Categoria_ID);
                command.Parameters.AddWithValue("@Codigo", producto.Codigo);
                command.Parameters.AddWithValue("@NombreProducto", producto.NombreProducto ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UnidadMedida", producto.UnidadMedida ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CapacidadUnidad", producto.CapacidadUnidad);
                command.Parameters.AddWithValue("@Cantidad", producto.Cantidad);
                command.Parameters.AddWithValue("@Activo", producto.Activo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@FechaRegistro", producto.FechaRegistro ?? (object)DBNull.Value);

                connection.Open();
                var rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected == 0)
                    throw new KeyNotFoundException("Producto no encontrado.");
            }
        }
    }
}