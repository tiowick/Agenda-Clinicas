using Agenda.Dominio;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Basico;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Enuns;
using Agenda.Dominio.Interfaces.Repositorio.Basico;
using Agenda.Dominio.Reflection;
using Agenda.Infra.Padronizar.CriacaoTabelas;
using Agenda.Infra.Padronizar.Texto;
using Agenda.Repositorio.Servicos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Repositorio.Repositorios.Basico
{
    public class MenusRepositorio
        : BaseRepositorio<Menus>
        , IMenusRepositorio

    {
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        private readonly TransferenciaIdentidadeDTO _identidade;
        public MenusRepositorio(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            _identidade = identidade;
        }

        public override async Task<IEnumerable<Menus>> GetData()
        {
            return await Task.FromResult(new List<Menus>()).ConfigureAwait(false);
        }

        public override async Task<bool> CreateList(IEnumerable<Menus> entity)
        {
            try
            {
                using DataTable _dTable = entity.ToDataTable<Menus>();
                IDCreated = await _dTable.MapperDatatableToPostgres<Menus>(ConnectionString).ConfigureAwait(true);
                return await Task.FromResult(IDCreated).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                IDCreated = false;
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            };
        }

        public override Task<long> CreateOrUpdate(Menus entity)
        {
            throw new NotImplementedException();
        }

        public async Task<RetornoGridPaginado<Menus>> CarregarGridMenus(DataTableSearch search, int start, int length, int draw)
        {
            try
            {
                //Para criar a proc de carregamento da grid use: exec dbo.geradorproccarregargridpadrao 'atendimento.Campanhas'
                var _query = string.Format("SELECT * FROM public.ssp_carregargridvw_exibicaomenussistema ({0}, {1}, {2}, {3}, '{4}');", _identidade.IdVendedorLogado, _identidade.IdUsuarioLogado, start, length, (search?.value ?? "")?.Trim().VarcharToSQL()); ;
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<Menus>(_query, buffered: true, commandTimeout: 1440);

                if (!_result.Any())
                    return new RetornoGridPaginado<Menus>().RetornoVazio(draw);

                var _return = new RetornoGridPaginado<Menus>
                {
                    data = _result,
                    draw = draw,
                    recordsFiltered = _result?.FirstOrDefault()?.RecordsTotal ?? 0,
                    recordsTotal = _result?.FirstOrDefault()?.RecordsTotal ?? 0,
                    JsonTypes = IResponseController.ResponseJsonTypes.Success.ToString().ToLower(culture: CultureInfo.CurrentCulture),
                };

                return await Task.FromResult(_return).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }
    }
}
