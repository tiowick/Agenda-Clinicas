using Agenda.Dominio.Entidades.Basico;
using Agenda.Dominio.Entidades.DataTablePaginado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Interfaces.Servicos.Basico
{
    public interface IMenusServicos : IBaseServicos<Menus>
    {
        Task<RetornoGridPaginado<Menus>> CarregarGridMenus(DataTableSearch search, int start, int length, int draw);
    }
}
