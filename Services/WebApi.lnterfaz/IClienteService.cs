using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Modelo;

namespace WebApi.Interfaz
{
    public interface IClienteService
    {
        Task<Cliente> Registrar(Cliente cliente);
        IEnumerable<Cliente> GetAll();
        Cliente GetById(int id);
        void Delete(int id);
        void Update(int id, Cliente cliente);
    }
}