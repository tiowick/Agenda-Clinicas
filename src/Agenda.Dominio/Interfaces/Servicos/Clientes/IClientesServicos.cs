using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Interfaces.Servicos.Clientes
{
    public interface IClientesServicos 
        : IBaseServicos<Cliente>
        , IDisposable
    {
    }
}
