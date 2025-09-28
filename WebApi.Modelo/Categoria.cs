namespace WebApi.Modelo
{
    public class Categorias
    {
        public int Categoria_ID { get; set; }
        public string? NombreCategoria { get; set; }
        public string? Descripcion { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}