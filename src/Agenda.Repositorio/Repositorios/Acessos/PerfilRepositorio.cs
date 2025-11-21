using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Acessos.DTO;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Acessos;
using Agenda.Dominio.Reflection;
using Agenda.Infra.Padronizar.CriacaoTabelas;
using Agenda.Infra.Padronizar.Texto;
using Agenda.Repositorio.Servicos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Repositorio.Repositorios.Acessos
{
    public class PerfilRepositorio
        : BaseRepositorio<Perfil>
        , IPerfilRepositorio
    {

        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        private readonly TransferenciaIdentidadeDTO _identidade;
        public PerfilRepositorio(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            _identidade = identidade;
        }



        public override async Task<bool> CreateList(IEnumerable<Perfil> entity)
        {
            try
            {
                using DataTable _dTable = entity.ToDataTable<Perfil>();
                IDCreated = await _dTable.MapperDatatableToPostgres<Perfil>(ConnectionString).ConfigureAwait(true);
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

        public override async Task<IEnumerable<Perfil>> GetData()
        {
            return await Task.FromResult(new List<Perfil>()).ConfigureAwait(false);
        }


        public override Task<long> CreateOrUpdate(Perfil entity)
        {
            throw new NotImplementedException();
        }





        public Task<IEnumerable<DataSelect2DTO>> CarregarComboNivelAcesso(string search, int page, int? length = 10)
        {
            throw new NotImplementedException();
        }

        public Task<RetornoGridPaginado<PerfilGridDto>> CarregarGridPerfil(DataTableSearch search, int start, int length, int draw)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<MenusLiberadosDTO>> CarregarMenusLiberadosAcessos(long? idModulo)
        {
            try
            {
                if (_identidade == null)
                    throw new Exception("Usuário não está logado");

                if (_identidade?.IdVendedorLogado == null)
                    throw new Exception("Usuário não está logado");

                if (_identidade?.IdVendedorLogado == 0)
                    throw new Exception("Usuário não está logado");

                var _idModulo = idModulo ?? 0;
                var _query = string.Format("exec acessos.ssp_carregarmenusliberadosacessos {0}, {1}, {2}", _identidade?.IdVendedorLogado ?? 0, _identidade?.IdUsuarioLogado ?? 0, _idModulo);
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<MenusLiberadosDTO>(_query, buffered: true, commandTimeout: 1440);

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
