using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly string _connectionString;


        public CategoriaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
            }
        }

        public Categorias Add(Categorias categoria)
        {
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria), "La categoría no puede ser nula.");

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                var command = new SqlCommand(
                    "INSERT INTO Categorias (NombreCategoria, Descripcion, Activo, FechaRegistro) OUTPUT INSERTED.IDCategoria VALUES (@NombreCategoria, @Descripcion, @Activo, @FechaRegistro)",
                    connection);

                command.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
                command.Parameters.AddWithValue("@Descripcion", categoria.Descripcion);
                command.Parameters.AddWithValue("@Activo", categoria.Activo);
                command.Parameters.AddWithValue("@FechaRegistro", DateTime.Now);

                categoria.IDCategoria = (int)command.ExecuteScalar();
            }


            return categoria;
        }

        public List<Categorias> GetAll()
        {
            var categorias = new List<Categorias>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Categorias", connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var categoria = new Categorias
                        {
                            IDCategoria = reader.GetInt32(reader.GetOrdinal("IDCategoria")),
                            NombreCategoria = reader.GetString(reader.GetOrdinal("NombreCategoria")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                            FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                        };
                        categorias.Add(categoria);
                    }
                }
            }
            return categorias;
        }

        public Categorias GetByID(int id)
        {
            Categorias categoria = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Categorias WHERE IDCategoria = @IDCategoria", connection);
                command.Parameters.AddWithValue("@IDCategoria", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        categoria = new Categorias
                        {
                            IDCategoria = reader.GetInt32(reader.GetOrdinal("IDCategoria")),
                            NombreCategoria = reader.GetString(reader.GetOrdinal("NombreCategoria")),
                            Descripcion = reader.GetString(reader.GetOrdinal("Descripcion")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                            FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                        };
                    }
                }
            }

            return categoria ?? throw new KeyNotFoundException("No se encontró la categoría con el ID especificado.");
        }

        public void Update(Categorias categoria)
        {
            if (categoria == null)
                throw new ArgumentNullException(nameof(categoria), "La categoría no puede ser nula.");

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "UPDATE Categorias SET NombreCategoria = @NombreCategoria, Descripcion = @Descripcion, Activo = @Activo WHERE IDCategoria = @IDCategoria",
                    connection);

                command.Parameters.AddWithValue("@IDCategoria", categoria.IDCategoria);
                command.Parameters.AddWithValue("@NombreCategoria", categoria.NombreCategoria);
                command.Parameters.AddWithValue("@Descripcion", categoria.Descripcion);
                command.Parameters.AddWithValue("@Activo", categoria.Activo);

                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Categorias WHERE IDCategoria = @IDCategoria", connection);
                command.Parameters.AddWithValue("@IDCategoria", id);

                connection.Open();
                command.ExecuteNonQuery();
            }


        }
    }

}

