using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
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
      
        Task<IEnumerable<DataSelect2DTO>> CarregarComboStatus(string search, int page, int? length = 10);
        Task<IEnumerable<DataSelect2DTO>> CarregarComboEmpresas(string search, int page, int? length = 10);
        Task<IEnumerable<DataSelect2DTO>> CarregarComboTipoSolitacao(string search, int page, int? length = 10);
        Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(DataTableSearch search, int start, int draw, int length = 10);
    }
}
