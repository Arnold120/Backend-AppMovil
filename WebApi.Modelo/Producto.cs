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
        public string? Marca { get; set; }
        public string? Categoria { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal? CostoCompra { get; set; }
        public decimal? MargenGanancia { get; set; }
        public decimal? PorcentajeMargen { get; set; }
        public string? EstadoStock { get; set; }

    }
}