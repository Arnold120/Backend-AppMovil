namespace WebApi.Modelo
{
    public class Usuario
    {
        public int Usuario_ID { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Contraseña { get; set; }
        public DateTime? UltimaActividad { get; set; }
        public bool EnSesion { get; set; }

    }
}