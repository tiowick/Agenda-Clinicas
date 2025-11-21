using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Interfaces
{
    public interface ISessaoServicoGlobal
    {
        Task RegistrarSessaoAsync(string userId);
        Task RemoverSessaoAsync(string userId);
    }
}
