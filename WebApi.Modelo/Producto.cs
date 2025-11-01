namespace WebApi.Modelo
{
    public class Producto
    {
        public int Producto_ID { get; set; }
        public int Marca_ID { get; set; }
        public int Categoria_ID { get; set; }
        public int Codigo { get; set; }
        public string? NombreProducto { get; set; }
        public string? UnidadMedida { get; set; }
        public int CapacidadUnidad { get; set; }
        public int Cantidad { get; set; }
        public bool? Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}