namespace WebApi.Modelo
{
    public class Proveedor
    {
        public int Proveedor_ID { get; set; }
        public string NombreEmpresa { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
        public bool AceptaDevoluciones { get; set; }
        public int TiempoDevolucion { get; set; }
        public decimal PorcentajeCobertura { get; set; }
    }
}