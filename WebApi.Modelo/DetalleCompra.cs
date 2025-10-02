namespace WebApi.Modelo
{
    public class DetalleCompra
    {
        public int DetalleCompra_ID { get; set; }
        public int Compra_ID { get; set; }
        public int Producto_ID { get; set; }
        public int CantidadUnitaria { get; set; }
        public decimal MontoUnitario { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
    }
}