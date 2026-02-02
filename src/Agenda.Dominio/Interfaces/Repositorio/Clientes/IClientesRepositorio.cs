using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Interfaces.Repositorio.Clientes
{
    public interface IClientesRepositorio 
        : IBaseRepositorio<Cliente>
        , IDisposable
    {
    }
}
