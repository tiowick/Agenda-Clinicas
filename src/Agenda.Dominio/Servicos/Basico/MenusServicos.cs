using Agenda.Dominio.Entidades.Basico;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Basico;
using Agenda.Dominio.Interfaces.Servicos.Basico;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Servicos.Basico
{
    public sealed class MenusServicos
        : BaseServicos<Menus>
        , IMenusServicos
    {

        private readonly IMenusRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;
        public MenusServicos(IMenusRepositorio MenusServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(MenusServicos, accessor, configuration, identidade)
        {
            _repositorio = MenusServicos;
            _user = accessor;
            _configuration = configuration;
        }

        public async Task<RetornoGridPaginado<Menus>> CarregarGridMenus(DataTableSearch search, int start, int length, int draw)
        {
            var _result = await _repositorio.CarregarGridMenus(search, start, length, draw);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }

    }
}
