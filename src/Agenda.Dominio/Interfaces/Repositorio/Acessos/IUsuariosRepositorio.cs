using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Interfaces.Repositorio.Acessos
{
    public interface IUsuariosRepositorio
        : IBaseRepositorio<Usuarios>
        , IDisposable
    {
        Task<RetornoGridPaginado<Usuarios>> CarregarGridUsuarios(DataTableSearch search, int start, int length, int draw, int status);
        Task<string> ValidarCriacaoUsuario(long idUsuario, string email, string documento);

    
    }
}
