namespace WebApi.Modelo
{
    public class VentaDto
    {
        public int? Cliente_ID { get; set; }
        public decimal MontoRecibido { get; set; }
        public decimal Descuento { get; set; } = 0;
        public List<DetalleVentaDto> DetallesVenta { get; set; } = new List<DetalleVentaDto>();
    }

    public class DetalleVentaDto
    {
        public int Producto_ID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal IVA { get; set; } 
        public string TipoComprobante { get; set; } = "Factura";
    }
}