using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.DataTablePaginado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Interfaces.Acessos
{
    //Entidade de Dominio: Usuarios
    public interface IUsuariosAppServicos
        : IBaseAppServicos<Usuarios>
        , IDisposable
    {
        Task<RetornoGridPaginado<Usuarios>> CarregarGridUsuarios(DataTableSearch search, int start, int length, int draw, int status);
        Task<string> ValidarCriacaoUsuario(long idUsuario, string email, string documento);
    }
}
