namespace WebApi.Modelo
{
    public class Devolucion
    {
        public int Devolucion_ID { get; set; }
        public int Venta_ID { get; set; }
        public DateTime FechaDevolucion { get; set; }
        public int CantidadDevuelta { get; set; }
        public decimal SubTotalDevuelto { get; set; }
        public decimal TotalDevuelto { get; set; }
        public string? Motivo { get; set; }
        public string TipoDevolucion { get; set; } = "Parcial";
        public bool Activo { get; set; }
    }
}