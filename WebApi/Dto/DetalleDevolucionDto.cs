namespace WebApi.Modelo
{
    public class DetalleDevolucionDto
    {
        public int Devolucion_ID { get; set; }
        public int DetalleVenta_ID { get; set; }
        public int Producto_ID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal? IVADevuelto { get; set; }
        public decimal SubtotalDevuelto { get; set; }
        public string EstadoProducto { get; set; } = "Bueno";
    }
}