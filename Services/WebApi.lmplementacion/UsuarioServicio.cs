using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApi.Interfaz;
using WebApi.Modelo;

namespace WebApi.Implementacion
{
    public class UsuarioService : IUsuarioService
    {
        private readonly string _connectionString;
        private readonly IConfiguration _configuration;

        public UsuarioService(IConfiguration configuration)
        {

            _configuration = configuration;

            _connectionString = _configuration.GetConnectionString("DatabaseConnection");

        }

        private string CreatePasswordHash(string password, out byte[] salt)
        {
            using var hmac = new HMACSHA256();
            salt = hmac.Key;
            var combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            var hash = hmac.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hash);
        }

        public async Task<Usuario> Registrar(Usuario usuario, string password)
        {
            byte[] salt;
            usuario.Contraseña = CreatePasswordHash(password, out salt);
            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("INSERT INTO Usuario (NombreUsuario, Contraseña, Salt, FechaRegistro) OUTPUT INSERTED.Usuario_ID VALUES (@NombreUsuario, @Contraseña, @Salt, @FechaRegistro)", connection);

                command.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
                command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
                command.Parameters.AddWithValue("@Salt", salt);
                command.Parameters.AddWithValue("@FechaRegistro", DateTime.UtcNow);

                await connection.OpenAsync();
                usuario.Usuario_ID = (int)await command.ExecuteScalarAsync();
            }

            return usuario;
        }

        private bool VerifyPasswordHash(string password, string storedHash, byte[] salt)
        {
            using var hmac = new HMACSHA256(salt);
            var combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            var computedHash = hmac.ComputeHash(combinedBytes);
            return storedHash == Convert.ToBase64String(computedHash);
        }

        public async Task<Usuario> Autenticar(string NombreUsuario, string password)
        {
            Usuario usuario = null;
            byte[] storedSalt = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var command = new SqlCommand("SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario", connection);
                command.Parameters.AddWithValue("@NombreUsuario", NombreUsuario);

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (reader.Read())
                    {
                        var passwordHash = reader["Contraseña"].ToString();
                        storedSalt = (byte[])reader["Salt"];
                        if (VerifyPasswordHash(password, passwordHash, storedSalt))
                        {
                            usuario = new Usuario
                            {
                                Usuario_ID = (int)reader["Usuario_ID"],
                                NombreUsuario = reader["NombreUsuario"].ToString(),
                                Contraseña = passwordHash
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public IEnumerable<Usuario> GetAll()
        {
            var usuarios = new List<Usuario>();

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Usuario_ID, NombreUsuario FROM Usuario";
                var command = new SqlCommand(query, connection);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usuarios.Add(new Usuario
                        {
                            Usuario_ID = reader.GetInt32(0),
                            NombreUsuario = reader.GetString(1)
                        });
                    }
                }
            }

            return usuarios;
        }

        public Usuario GetById(int id)
        {
            Usuario? usuario = null;

            using (var connection = new SqlConnection(_connectionString))
            {
                var query = "SELECT Usuario_ID, NombreUsuario FROM Usuario WHERE Usuario_ID = @Usuario_ID";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Usuario_ID", id);

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        usuario = new Usuario
                        {
                            Usuario_ID = reader.GetInt32(0),
                            NombreUsuario = reader.GetString(1)
                        };
                    }
                }
            }

            if (usuario == null)
                throw new KeyNotFoundException("Usuario no encontrado.");

            return usuario;
        }

        public string ObtenerRolDelUsuario(int idUsuario)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var query = @"SELECT TOP 1 R.NombreRol FROM UsuarioRol UR INNER JOIN Rol R ON UR.Rol_ID = R.Rol_ID WHERE UR.Usuario_ID = @Usuario_ID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Usuario_ID", idUsuario);
                    var result = command.ExecuteScalar();
                    return result?.ToString();
                }
            }
        }


        public string GenerateJwtToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            string rol = ObtenerRolDelUsuario(usuario.Usuario_ID);

            if (string.IsNullOrEmpty(rol))
            {
                throw new InvalidOperationException("El rol del usuario no se encontró.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Usuario_ID.ToString()),
                new Claim(ClaimTypes.Role, rol)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}