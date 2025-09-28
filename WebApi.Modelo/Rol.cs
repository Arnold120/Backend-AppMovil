namespace WebApi.Modelo
{
    public class Rol
    {
        public int Rol_ID { get; set; }
        public string? NombreRol { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}