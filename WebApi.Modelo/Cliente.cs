using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Modelo
{
    public class Cliente
    {
        public int IDCliente { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
