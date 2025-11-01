namespace WebApi.Modelo
{
    public class MovimientoInventario
    {
        public int MovimientoInventario_ID { get; set; }
        public int Producto_ID { get; set; }
        public int PrecioProducto_ID { get; set; }
        public string? NombreProducto { get; set; }
        public string? TipoMovimiento { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public int Referencia_ID { get; set; }
        public string? TipoReferencia { get; set; }
        public DateTime FechaMovimiento { get; set; }
    }
}