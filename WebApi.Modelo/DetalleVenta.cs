namespace WebApi.Modelo
{
    public class DetalleVenta
    {
        public int DetalleVenta_ID { get; set; }
        public int Venta_ID { get; set; }
        public int Producto_ID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string TipoComprobante { get; set; } = "Factura";
    }
}