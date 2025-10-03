namespace WebApi.Modelo
{
    public class Factura
    {
        public int Factura_ID { get; set; }
        public int Venta_ID { get; set; }
        public int? Cliente_ID { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public string? Serie { get; set; }
        public string? Correlativo { get; set; }
        public DateTime FechaFactura { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IVA { get; set; }
        public decimal Descuento { get; set; }
        public decimal TotalFactura { get; set; }
        public string Moneda { get; set; } = "Córdoba";
        public string MetodoPago { get; set; } = "Efectivo";
        public string TipoPago { get; set; } = "Contado";
        public string Estado { get; set; } = "Emitida";
        public List<DetalleFactura> DetallesFactura { get; set; } = new List<DetalleFactura>();
    }
}