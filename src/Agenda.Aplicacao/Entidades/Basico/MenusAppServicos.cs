using Agenda.Aplicacao.Interfaces.Basico;
using Agenda.Dominio;
using Agenda.Dominio.Entidades.Basico;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Basico;
using Agenda.Dominio.Interfaces.Servicos.Basico;
using Agenda.Dominio.Servicos.Basico;
using Agenda.Repositorio.Repositorios.Basico;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Entidades.Basico
{
    public class MenusAppServicos
        : BaseAppServicos<Menus>
        , IMenusAppServicos
        , IDisposable
    {

        private readonly IMenusServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;

        public MenusAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IMenusRepositorio _repositorio = new MenusRepositorio(_accessor, _configuration, identidade);
            _servico = new MenusServicos(_repositorio, _accessor, _configuration, identidade);
            base.SetBaseServicos(_servico);
        }

        public MenusAppServicos(IMenusServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
           : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IMenusRepositorio _repositorio = new MenusRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new MenusServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }

        public async Task<RetornoGridPaginado<Menus>> CarregarGridMenus(DataTableSearch search, int start, int length, int draw)
        {
            return await _servico.CarregarGridMenus(search, start, length, draw);
        }


    }
}
