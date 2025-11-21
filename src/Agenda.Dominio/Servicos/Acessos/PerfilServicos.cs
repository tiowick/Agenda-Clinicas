using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Acessos.DTO;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Acessos;
using Agenda.Dominio.Interfaces.Servicos.Acessos;
using Agenda.Dominio.Servicos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;

namespace CRM_Dominio.Servicos.Acessos
{
    //Entidade de Dominio: Perfil
    public sealed class PerfilServicos
        : BaseServicos<Perfil>
        , IPerfilServicos 
    {
        private readonly IPerfilRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;

        public PerfilServicos(IPerfilRepositorio PerfilServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(PerfilServicos, accessor, configuration, identidade)
        {
            _repositorio = PerfilServicos;
            _user = accessor;
            _configuration = configuration;
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboNivelAcesso(string search, int page, int? length = 10)
        {
            var _result = await _repositorio.CarregarComboNivelAcesso(search, page, length).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }

        public async Task<RetornoGridPaginado<PerfilGridDto>> CarregarGridPerfil(DataTableSearch search, int start, int length, int draw)
        {
            var _result = await _repositorio.CarregarGridPerfil(search, start, length, draw).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }

        private void CarregarChildrem(IEnumerable<MenusLiberadosDTO> lista, MenusLiberadosDTO menus)
        {
            if (menus == null)
                return;

            var _childrem = lista.Where(p => p.CodigoPai == menus.Codigo).ToList();
            if(_childrem != null)
            {
                if(_childrem.Any())
                {
                    menus.Filhos = new List<MenusLiberadosDTO>();

                    foreach (var menu in _childrem)
                    {
                        var _netos = lista.Where(p => p.CodigoPai == menu.Codigo).ToList();
                        if(_netos != null)
                        {
                            if (_netos.Any())
                                CarregarChildrem(lista, menu);
                            else
                                menus.Filhos.Add(menu);
                        }
                    }
                    MenusLiberadosDTOs.Add(menus);
                }
                else
                {
                    MenusLiberadosDTOs.Add(menus);
                }
            }
        }

        private List<MenusLiberadosDTO> MenusLiberadosDTOs = new List<MenusLiberadosDTO>();

        public async Task<IEnumerable<MenusLiberadosDTO>> CarregarMenusLiberadosAcessos(long? idModulo)
        {
            var _result = await _repositorio.CarregarMenusLiberadosAcessos(idModulo).ConfigureAwait(true);
            var _menuspai = _result.Where(p => p.CodigoPai == null).ToList();
            MenusLiberadosDTOs = new List<MenusLiberadosDTO>();

            foreach (var menu in _menuspai)
                CarregarChildrem(_result, menu);

            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return MenusLiberadosDTOs ?? new List<MenusLiberadosDTO>();
        }
    }
}
