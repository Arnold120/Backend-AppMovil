namespace WebApi.Modelo
{
    public class UsuarioRol
    {
        public int UsuarioRol_ID { get; set; }
        public int Usuario_ID { get; set; }
        public int Rol_ID { get; set; }
        public DateTime FechaAsignacion { get; set; }
    }
}