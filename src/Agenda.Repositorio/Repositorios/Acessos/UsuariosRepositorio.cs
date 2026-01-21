using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Enuns;
using Agenda.Dominio.Interfaces.Repositorio.Acessos;
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

namespace Agenda.Repositorio.Repositorios.Acessos
{
    public class UsuariosRepositorio
        : BaseRepositorio<Usuarios>
        , IUsuariosRepositorio
    {

        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        private readonly TransferenciaIdentidadeDTO _identidade;

        public UsuariosRepositorio(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(configuration, identidade)
        {
            _accessor = accessor; _configuration = configuration;
            _identidade = identidade;
        }


        public override async Task<bool> CreateList(IEnumerable<Usuarios> entity)
        {
            try
            {
                using DataTable _dTable = entity.ToDataTable<Usuarios>();
                IDCreated = await _dTable.MapperDatatableToPostgres<Usuarios>(ConnectionString).ConfigureAwait(true);
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

        public override async Task<IEnumerable<Usuarios>> GetData()
        {
            return await Task.FromResult(new List<Usuarios>()).ConfigureAwait(false);
        }


        public async override Task<long> CreateOrUpdate(Usuarios entity)
        {
            try
            {
                var _query = $@"select * from public.ssp_alterarusuarios(
                       {entity.ID}                                                     
                    , '{entity.Documento.VarcharToSQL().RetirarMascaraDocumento()}'   
                    , '{entity.Nome.VarcharToSQL()}'
                    , '{entity.Email.VarcharToSQL()}'
                    , '{entity.Telefone.VarcharToSQL()}'
                    , '{entity?.Cidade?.VarcharToSQL() ?? ""}'
                    , '{entity?.UF?.VarcharToSQL() ?? ""}'
                    , '{entity?.Senha?.VarcharToSQL() ?? ""}'
                    , '{entity?.IdClains?.VarcharToSQL() ?? ""}'
                    , {(entity?.Status ?? 1)}::smallint     
                    , {(entity?.IdVendedor)}                             
                    , {entity?.IdEmpresa});";
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

        public async Task<RetornoGridPaginado<Usuarios>> CarregarGridUsuarios(DataTableSearch search, int start, int length, int draw, int status)
        {
            try
            {
                //var teste = "PRECISA CRIAR A PROC JEFERSON";
                var _query = string.Format("SELECT * FROM public.ssp_carregargridagendas ({0}, {1}, {2}, {3}, '{4}');", _identidade.IdVendedorLogado, _identidade.IdUsuarioLogado, start, length, (search?.value ?? "")?.Trim().VarcharToSQL());
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<Usuarios>(_query, buffered: true, commandTimeout: 1440);

                if (!_result.Any())
                    return new RetornoGridPaginado<Usuarios>().RetornoVazio(draw);

                var _return = new RetornoGridPaginado<Usuarios>
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

        public async Task<string> ValidarCriacaoUsuario(long idUsuario, string email, string documento)
        {
            try
            {
                var _email = (email ?? "")?.Trim().VarcharToSQL();
                var _documento = (documento ?? "")?.Trim().VarcharToSQL();

                var _query = string.Format("select * from public.ssp_validarcriacaousuario({0}, '{1}', '{2}', {3});", _identidade.IdEmpresaLogado, _email, _documento, idUsuario);
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<string>(_query, buffered: true, commandTimeout: 1440);


                if (_result == null)
                    return await Task.FromResult("").ConfigureAwait(false);

                if (!_result.Any())
                    return await Task.FromResult("").ConfigureAwait(false);

                var _count = 1;
                var _erro = new StringBuilder();

                foreach (var item in _result)
                {
                    _erro.AppendLine(string.Concat("(", _count, ") ", item.ToString()));
                    _count += 1;
                }
                var _mensagem = _erro.ToString();
                return await Task.FromResult(_mensagem).ConfigureAwait(false);
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
