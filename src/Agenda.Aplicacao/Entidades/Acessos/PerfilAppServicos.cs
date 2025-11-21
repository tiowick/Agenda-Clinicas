
using Agenda.Aplicacao;
using Agenda.Aplicacao.Interfaces.Acessos;
using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Acessos.DTO;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Acessos;
using Agenda.Dominio.Interfaces.Servicos.Acessos;
using Agenda.Repositorio.Repositorios.Acessos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using CRM_Dominio.Servicos.Acessos;
using Microsoft.Extensions.Configuration;

namespace CRM_Aplicacao.Entidades.Acessos
{
    //Entidade de Dominio: Perfil
    public class PerfilAppServicos
        : BaseAppServicos<Perfil>
        , IPerfilAppServicos
        , IDisposable
    {
        private readonly IPerfilServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;

        public PerfilAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IPerfilRepositorio _repositorio = new PerfilRepositorio(_accessor, _configuration, identidade);
            _servico = new PerfilServicos(_repositorio, _accessor, _configuration, identidade);
            base.SetBaseServicos(_servico);
        }

        public PerfilAppServicos(IPerfilServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IPerfilRepositorio _repositorio = new PerfilRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new PerfilServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboNivelAcesso(string search, int page, int? length = 10)
        {
            return await _servico.CarregarComboNivelAcesso(search, page, length).ConfigureAwait(true);
        }

        public async Task<RetornoGridPaginado<PerfilGridDto>> CarregarGridPerfil(DataTableSearch search, int start, int length, int draw)
        {
            return await _servico.CarregarGridPerfil(search, start, length, draw).ConfigureAwait(true);
        }

        public async Task<IEnumerable<MenusLiberadosDTO>> CarregarMenusLiberadosAcessos(long? idModulo)
        {
            var _retorno = await _servico.CarregarMenusLiberadosAcessos(idModulo);
            return _retorno;
        }
    }
}