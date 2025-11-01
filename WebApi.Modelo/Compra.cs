namespace WebApi.Modelo
{
    public class Compras
    {
        public int Compra_ID { get; set; }
        public int Usuario_ID { get; set; }
        public int Proveedor_ID { get; set; }
        public int CantidadTotal { get; set; }
        public decimal MontoTotal { get; set; }
        public decimal SubTotal { get; set; }
        public decimal IVATotal { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaRegistro { get; set; }

        public List<DetalleCompra> DetallesCompra { get; set; } = new List<DetalleCompra>();
    }
}