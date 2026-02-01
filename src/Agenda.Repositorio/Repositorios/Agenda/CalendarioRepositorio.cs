using Agenda.Dominio;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Enuns;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
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
using System.Xml.Linq;

namespace Agenda.Repositorio.Repositorios.Agenda
{
    public class CalendarioRepositorio
        : BaseRepositorio<Calendario>
        , ICalendarioRepositorio
    {

        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        private readonly TransferenciaIdentidadeDTO _identidade;

        public CalendarioRepositorio(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(configuration, identidade)
        {
            _accessor = accessor; _configuration = configuration;
            _identidade = identidade;
        }

        public override async Task<bool> CreateList(IEnumerable<Calendario> entity)
        {
            try
            {
                using DataTable _dTable = entity.ToDataTable<Calendario>();
                IDCreated = await _dTable.MapperDatatableToPostgres<Calendario>(ConnectionString).ConfigureAwait(true);
                return await Task.FromResult(IDCreated).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                IDCreated = false;
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

   
        public override async Task<IEnumerable<Calendario>> GetData()
        {
            return await Task.FromResult(new List<Calendario>()).ConfigureAwait(false);
        }

        public async override Task<long> CreateOrUpdate(Calendario entity)
        {
            try
            {
                var _query = $@"select * from public.ssp_alteraragenda(
                     {entity.ID}
                   , {Identidade.IdUsuarioLogado}
                   , {Identidade.IdEmpresaLogado}
                   , {Identidade.IdVendedorLogado}
                   , {entity.IdLaudo ?? 0} 
                   , {entity.IdSetor ?? 0} 
                   , {entity.IdResponsavel ?? 0}
                   , {entity.IdSituacao ?? 0}
                   , {entity.IdSolicitacao ?? 0}
                   , {entity.status ?? 0}
                   , '{entity.Descricao.VarcharToSQL()}'
                   , '{entity.Solicitante?.VarcharToSQL() ?? ""}'
                   , {(entity.Privativa == true ? "true" : "false")}
                   , '{entity.Contato?.VarcharToSQL() ?? ""}'
                   , '{entity.Local?.VarcharToSQL() ?? ""}'
                   , '{entity.Observacao?.VarcharToSQL() ?? ""}'
                   , '{entity.Objetivo?.VarcharToSQL() ?? ""}'
                   , '{entity.DataHora.DatatimeToSQL()}'
                   , '{(entity.DataHoraIni.DatatimeToSQL())}'
                   , '{entity.LocalizacaoIni?.VarcharToSQL() ?? ""}'
                   , '{(entity.datahorafim.DatatimeToSQL())}'
                   , '{entity.LocalizacaoFim?.VarcharToSQL() ?? ""}');";

                var cn = new SqlSystemConnect(ConnectionString);
                ID = cn.Query<long>(_query, buffered: true, commandTimeout: 1440).FirstOrDefault();
                return await Task.FromResult(ID).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

        public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(DataTableSearch search, int start, int draw, int length = 10)
        {
            try
            {
                var _query = string.Format("select * from public.ssp_carregargridagendas ({0}, {1}, {2}, {3}, '{4}');", _identidade.IdVendedorLogado, _identidade.IdUsuarioLogado, start, length, (search?.value ?? "")?.Trim().VarcharToSQL());
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<Calendario>(_query, buffered: true, commandTimeout: 1440);

                if (!_result.Any())
                    return new RetornoGridPaginado<Calendario>().RetornoVazio(draw);

                var _return = new RetornoGridPaginado<Calendario>
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

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboStatus(string search, int page, int? length = 10)
        {
            try
            {
                var _page = ((page <= 0 ? 1 : page) - 1) * 10;
                var _length = (length ?? 10);
                var _query = string.Format("select * from public.fn_carregar_combo_status ({0}, {1}, {2}, {3}, '{4}');", _identidade.IdEmpresaLogado, _identidade.IdUsuarioLogado, _page, _length, (search ?? "")?.Trim().VarcharToSQL());
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<DataSelect2DTO>(_query, buffered: true, commandTimeout: 1440);

                return await Task.FromResult(_result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboTipoSolicitacao(string search, int page, int? length = 10)
        {
            try
            {
                var _page = ((page <= 0 ? 1 : page) - 1) * 10;
                var _length = (length ?? 10);
                var _query = string.Format("select * from public.fn_carregar_combo_tiposolicitacao ({0}, {1}, {2}, {3}, '{4}');", _identidade.IdEmpresaLogado, _identidade.IdUsuarioLogado, _page, _length, (search ?? "")?.Trim().VarcharToSQL());
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<DataSelect2DTO>(_query, buffered: true, commandTimeout: 1440);

                return await Task.FromResult(_result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboEmpresas(string search, int page, int? length = 10)
        {
            try
            {
                var _page = ((page <= 0 ? 1 : page) - 1) * 10;
                var _length = (length ?? 10);
                var _query = string.Format("select * from public.fn_carregar_combo_empresas ({0}, {1}, {2}, {3}, '{4}');", _identidade.IdEmpresaLogado, _identidade.IdUsuarioLogado, _page, _length, (search ?? "")?.Trim().VarcharToSQL());
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<DataSelect2DTO>(_query, buffered: true, commandTimeout: 1440);

                return await Task.FromResult(_result).ConfigureAwait(false);
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

