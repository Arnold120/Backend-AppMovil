﻿using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using WebApi.Interfaz;
using WebApi.Modelo;


namespace WebApi.Implementacion
{
    public class MarcaService : IMarcaService
    {
        private readonly string _connectionString;

        public MarcaService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DatabaseConnection");
            if (string.IsNullOrEmpty(_connectionString))
            {
                throw new InvalidOperationException("La cadena de conexión no puede ser nula.");
            }
        }


        public Marcas Add(Marcas marca)
        {
            if (marca == null)
                throw new ArgumentNullException(nameof(marca), "La marca no puede ser nula.");

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "INSERT INTO Marcas (NombreMarca, Activo, FechaRegistro) OUTPUT INSERTED.IDMarca VALUES (@NombreMarca, @Activo, @FechaRegistro)",
                    connection);

                command.Parameters.AddWithValue("@NombreMarca", marca.NombreMarca);
                command.Parameters.AddWithValue("@Activo", marca.Activo);
                command.Parameters.AddWithValue("@FechaRegistro", marca.FechaRegistro);

                connection.Open();
                marca.IDMarca = (int)command.ExecuteScalar();
            }


            return marca;
        }

        public void Update(Marcas marca)
        {
            if (marca == null)
                throw new ArgumentNullException(nameof(marca), "La marca no puede ser nula.");

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand(
                    "UPDATE Marcas SET NombreMarca = @NombreMarca, Activo = @Activo, FechaRegistro = @FechaRegistro WHERE IDMarca = @IDMarca",
                    connection);

                command.Parameters.AddWithValue("@IDMarca", marca.IDMarca);
                command.Parameters.AddWithValue("@NombreMarca", marca.NombreMarca);
                command.Parameters.AddWithValue("@Activo", marca.Activo);
                command.Parameters.AddWithValue("@FechaRegistro", marca.FechaRegistro);

                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        public void Delete(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("DELETE FROM Marcas WHERE IDMarca = @IDMarca", connection);
                command.Parameters.AddWithValue("@IDMarca", id);

                connection.Open();
                command.ExecuteNonQuery();
            }

        }

        public List<Marcas> GetAll()
        {
            var marcas = new List<Marcas>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Marcas", connection);
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var marca = new Marcas
                        {
                            IDMarca = reader.GetInt32(reader.GetOrdinal("IDMarca")),
                            NombreMarca = reader.GetString(reader.GetOrdinal("NombreMarca")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                            FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                        };
                        marcas.Add(marca);
                    }
                }
            }

            return marcas;
        }
        public Marcas GetByID(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Marcas WHERE IDMarca = @IDMarca", connection);
                command.Parameters.AddWithValue("@IDMarca", id);

                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Marcas
                        {
                            IDMarca = reader.GetInt32(reader.GetOrdinal("IDMarca")),
                            NombreMarca = reader.GetString(reader.GetOrdinal("NombreMarca")),
                            Activo = reader.GetBoolean(reader.GetOrdinal("Activo")),
                            FechaRegistro = reader.GetDateTime(reader.GetOrdinal("FechaRegistro"))
                        };
                    }
                }
            }
            return null;
        }
    }
}

