namespace WebApi.Modelo
{
    public class FacturaDto
    {
        public int Venta_ID { get; set; }
        public int? Cliente_ID { get; set; }
        public string? Serie { get; set; }
        public string? Correlativo { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal Descuento { get; set; }
        public string Moneda { get; set; } = "Córdoba";
        public string MetodoPago { get; set; } = "Efectivo";
        public string TipoPago { get; set; } = "Contado";
        public List<DetalleFacturaDto> DetallesFactura { get; set; } = new List<DetalleFacturaDto>();
    }

    public class DetalleFacturaDto
    {
        public int DetalleVenta_ID { get; set; }
        public int Producto_ID { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Descuento { get; set; }
    }
}