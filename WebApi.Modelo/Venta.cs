namespace WebApi.Modelo
{
    public class Venta
    {
        public int Venta_ID { get; set; }
        public int Usuario_ID { get; set; }
        public int? Cliente_ID { get; set; }
        public DateTime FechaVenta { get; set; }
        public int CantidadTotal { get; set; }
        public decimal MontoRecibido { get; set; }
        public decimal MontoDevuelto { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = "Activo";
        public List<DetalleVenta> DetallesVenta { get; set; } = new List<DetalleVenta>();
    }
}