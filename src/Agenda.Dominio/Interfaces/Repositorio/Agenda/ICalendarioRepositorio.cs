using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Interfaces.Repositorio.Agenda
{
    public interface ICalendarioRepositorio
        : IBaseRepositorio<Calendario>
        , IDisposable
    {
        Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(DataTableSearch search, int start, int draw, int length = 10);
    }
}
