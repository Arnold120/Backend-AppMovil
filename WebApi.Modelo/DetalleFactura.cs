namespace WebApi.Modelo
{
    public class DetalleFactura
    {
        public int DetalleFactura_ID { get; set; }
        public int Factura_ID { get; set; }
        public int Venta_ID { get; set; }
        public int DetalleVenta_ID { get; set; }
        public int Producto_ID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
    }
}