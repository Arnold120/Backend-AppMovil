using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        private readonly TimeZoneInfo _zonaMexico;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(IConfiguration configuration, ILogger<UsuarioService> logger)
        {
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _connectionString = _configuration.GetConnectionString("DatabaseConnection");

            try
            {
                _zonaMexico = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time (Mexico)");
            }
            catch
            {
                _zonaMexico = TimeZoneInfo.FindSystemTimeZoneById("America/Mexico_City");
            }
        }

        private DateTime FechaActualLocal() =>
            TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, _zonaMexico);

        private string CreatePasswordHash(string password, out byte[] salt)
        {
            using var hmac = new HMACSHA256();
            salt = hmac.Key;
            var combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
            var hash = hmac.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hash);
        }

        private bool VerifyPasswordHash(string password, string storedHash, byte[] salt)
        {
            try
            {
                using var hmac = new HMACSHA256(salt);
                var combinedBytes = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
                var computedHash = hmac.ComputeHash(combinedBytes);
                var computedHashString = Convert.ToBase64String(computedHash);
                return storedHash == computedHashString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar hash de contraseña");
                return false;
            }
        }

        public async Task<Usuario> Registrar(Usuario usuario, string password)
        {
            byte[] salt;
            usuario.Contraseña = CreatePasswordHash(password, out salt);

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var fechaLocal = FechaActualLocal();

            var command = new SqlCommand(@"
                INSERT INTO Usuario 
                (NombreUsuario, Contraseña, Salt, FechaRegistro, UltimaActividad) 
                OUTPUT INSERTED.Usuario_ID 
                VALUES (@NombreUsuario, @Contraseña, @Salt, @FechaRegistro, @UltimaActividad)", connection);

            command.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
            command.Parameters.AddWithValue("@Contraseña", usuario.Contraseña);
            command.Parameters.AddWithValue("@Salt", salt);
            command.Parameters.AddWithValue("@FechaRegistro", fechaLocal);
            command.Parameters.AddWithValue("@UltimaActividad", fechaLocal);

            usuario.Usuario_ID = (int)await command.ExecuteScalarAsync();
            return usuario;
        }

        public async Task<Usuario?> Autenticar(string nombreUsuario, string password)
        {
            try
            {
                Usuario usuario = null;
                byte[] storedSalt = null;
                string storedHash = null;

                using (var connection = new SqlConnection(_connectionString))
                {
                    var command = new SqlCommand("SELECT * FROM Usuario WHERE NombreUsuario = @NombreUsuario", connection);
                    command.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (reader.Read())
                        {
                            storedHash = reader["Contraseña"]?.ToString();
                            storedSalt = reader["Salt"] as byte[];

                            if (!string.IsNullOrEmpty(storedHash) && storedSalt != null)
                            {
                                if (VerifyPasswordHash(password, storedHash, storedSalt))
                                {
                                    usuario = new Usuario
                                    {
                                        Usuario_ID = (int)reader["Usuario_ID"],
                                        NombreUsuario = reader["NombreUsuario"].ToString(),
                                        Contraseña = storedHash,
                                        EnSesion = reader["EnSesion"] != DBNull.Value && Convert.ToBoolean(reader["EnSesion"])
                                    };
                                }
                            }
                        }
                    }

                    if (usuario != null)
                    {
                        var update = new SqlCommand(
                            @"UPDATE Usuario 
                      SET UltimaActividad = @UltimaActividad, 
                          EnSesion = 1 
                      WHERE Usuario_ID = @Usuario_ID",
                            connection);
                        update.Parameters.AddWithValue("@UltimaActividad", FechaActualLocal());
                        update.Parameters.AddWithValue("@Usuario_ID", usuario.Usuario_ID);
                        await update.ExecuteNonQueryAsync();

                        usuario.EnSesion = true;
                    }
                }

                return usuario;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Autenticar: {ex.Message}");
                throw;
            }
        }

        public IEnumerable<Usuario> GetAll()
        {
            var usuarios = new List<Usuario>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand("SELECT Usuario_ID, NombreUsuario, UltimaActividad FROM Usuario", connection);
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                usuarios.Add(new Usuario
                {
                    Usuario_ID = reader.GetInt32(0),
                    NombreUsuario = reader.GetString(1),
                    UltimaActividad = reader["UltimaActividad"] == DBNull.Value
                        ? null
                        : (DateTime?)reader["UltimaActividad"]
                });
            }
            return usuarios;
        }

        public Usuario GetById(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var command = new SqlCommand(
                "SELECT Usuario_ID, NombreUsuario, UltimaActividad FROM Usuario WHERE Usuario_ID = @Usuario_ID",
                connection);
            command.Parameters.AddWithValue("@Usuario_ID", id);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Usuario
                {
                    Usuario_ID = reader.GetInt32(0),
                    NombreUsuario = reader.GetString(1),
                    UltimaActividad = reader["UltimaActividad"] == DBNull.Value
                        ? null
                        : (DateTime?)reader["UltimaActividad"]
                };
            }

            throw new KeyNotFoundException("Usuario no encontrado.");
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            try
            {
                _logger.LogInformation("Generando token JWT para usuario: {UsuarioId}", usuario.Usuario_ID);

                var tokenHandler = new JwtSecurityTokenHandler();

                var jwtKey = _configuration["Jwt:Key"] ?? "SistemaDeLibreria2024@SecretKey@Minimo32Caracteres!!";

                if (jwtKey.Length < 32)
                {
                    jwtKey = jwtKey.PadRight(32, '!');
                    _logger.LogWarning("Clave JWT muy corta, se ha extendido a 32 caracteres");
                }

                var key = Encoding.UTF8.GetBytes(jwtKey);

                string rol;
                try
                {
                    rol = ObtenerRolDelUsuario(usuario.Usuario_ID);
                    _logger.LogInformation("Rol obtenido para usuario {UsuarioId}: {Rol}", usuario.Usuario_ID, rol);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al obtener rol para usuario {UsuarioId}", usuario.Usuario_ID);
                    rol = "Usuario";
                }

                if (string.IsNullOrEmpty(rol))
                {
                    rol = "Usuario";
                    _logger.LogWarning("Rol vacío para usuario {UsuarioId}, usando rol por defecto", usuario.Usuario_ID);
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, usuario.Usuario_ID.ToString()),
                    new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                    new Claim(ClaimTypes.Role, rol)
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                _logger.LogInformation("Token JWT generado exitosamente para usuario: {UsuarioId}", usuario.Usuario_ID);

                return tokenString;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar token JWT para usuario {UsuarioId}", usuario.Usuario_ID);
                throw new Exception($"Error al generar token JWT: {ex.Message}");
            }
        }

        public string ObtenerRolDelUsuario(int idUsuario)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                var query = @"SELECT TOP 1 R.NombreRol 
                              FROM UsuarioRol UR 
                              INNER JOIN Rol R ON UR.Rol_ID = R.Rol_ID 
                              WHERE UR.Usuario_ID = @Usuario_ID";

                using var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Usuario_ID", idUsuario);
                var result = command.ExecuteScalar();

                var rol = result?.ToString();
                return rol ?? "Usuario";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol para usuario {UsuarioId}", idUsuario);
                return "Usuario";
            }
        }
        public IEnumerable<Usuario> ObtenerUsuariosEnLinea()
        {
            var usuarios = GetAll();
            return usuarios.Where(EstaEnLinea).ToList();
        }

        public bool EstaEnLinea(Usuario usuario)
        {
            if (usuario.UltimaActividad == null)
                return false;

            var tiempoInactivo = (FechaActualLocal() - usuario.UltimaActividad.Value).TotalMinutes;
            return tiempoInactivo <= 1;
        }
        
        public async Task<bool> ActualizarActividad(int usuarioId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand(
                    @"UPDATE Usuario 
              SET UltimaActividad = @UltimaActividad, 
                  EnSesion = 1 
              WHERE Usuario_ID = @Usuario_ID",
                    connection);

                command.Parameters.AddWithValue("@UltimaActividad", FechaActualLocal());
                command.Parameters.AddWithValue("@Usuario_ID", usuarioId);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar actividad para usuario {UsuarioId}", usuarioId);
                return false;
            }
        }

        public async Task<bool> CerrarSesion(int usuarioId)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new SqlCommand(
                    @"UPDATE Usuario 
                      SET UltimaActividad = @UltimaActividad
                      , EnSesion = 0
                      WHERE Usuario_ID = @Usuario_ID",
                    connection);

                command.Parameters.AddWithValue("@UltimaActividad", FechaActualLocal());
                command.Parameters.AddWithValue("@Usuario_ID", usuarioId);

                var result = await command.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cerrar sesión para usuario {UsuarioId}", usuarioId);
                return false;
            }
        }

        public (bool estaEnLinea, DateTime? ultimaActividad) VerificarEstadoEnLinea(int usuarioId)
        {
            try
            {
                var usuario = GetById(usuarioId);
                var estaEnLinea = EstaEnLinea(usuario);

                return (estaEnLinea, usuario.UltimaActividad);
            }
            catch (KeyNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar estado en línea para usuario {UsuarioId}", usuarioId);
                throw;
            }
        }

        public IEnumerable<object> ObtenerTodosLosEstadosEnLinea()
        {
            try
            {
                var usuarios = GetAll();
                var estados = usuarios.Select(u => new
                {
                    usuarioId = u.Usuario_ID,
                    nombreUsuario = u.NombreUsuario,
                    estaEnLinea = EstaEnLinea(u),
                    enSesion = u.EnSesion,
                    ultimaActividad = u.UltimaActividad,
                    tiempoInactivo = u.UltimaActividad.HasValue ?
                        (double?)(DateTime.Now - u.UltimaActividad.Value).TotalMinutes : null
                });

                return estados;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los estados en línea");
                throw;
            }
        }
    }
}
