using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Empresas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Interfaces.Empresas
{
    public interface IEmpresasAppServicos 
        : IBaseAppServicos<Empresa>
        , IDisposable
    {

    }
}
