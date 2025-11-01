namespace WebApi.Modelo
{
    public class ComprasDto
    {
        public int Proveedor_ID { get; set; }
        public DateTime FechaRegistro { get; set; }

        public List<DetalleCompraDto> DetallesCompra { get; set; } = new List<DetalleCompraDto>();

        public class DetalleCompraDto
        {
            public int Producto_ID { get; set; }
            public int CantidadUnitaria { get; set; }
            public decimal MontoUnitario { get; set; }
            public decimal IVA { get; set; }
        }
    }
}