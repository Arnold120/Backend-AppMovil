namespace WebApi.Modelo
{
    public class DevolucionDto
    {
        public int Venta_ID { get; set; }
        public int CantidadDevuelta { get; set; }
        public decimal SubTotalDevuelto { get; set; }
        public decimal TotalDevuelto { get; set; }
        public string? Motivo { get; set; }
        public string TipoDevolucion { get; set; } = "Parcial";
    }
}