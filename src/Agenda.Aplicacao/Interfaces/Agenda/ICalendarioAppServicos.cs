using Agenda.Dominio.Entidades.Agenda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Interfaces.Agenda
{
    public interface ICalendarioAppServicos 
        : IBaseAppServicos<Calendario>
        , IDisposable
    {

    }
}
