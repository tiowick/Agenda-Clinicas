using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Clientes;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Entidades.Empresas;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Clientes;
using Agenda.Dominio.Reflection;
using Agenda.Infra.Padronizar.CriacaoTabelas;
using Agenda.Infra.Padronizar.Texto;
using Agenda.Repositorio.Servicos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Repositorio.Repositorios.Clientes
{
    public class ClientesRepositorio
        : BaseRepositorio<Cliente>
        , IClientesRepositorio
    {
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        private readonly TransferenciaIdentidadeDTO _identidade;
        public ClientesRepositorio(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(configuration, identidade)
        {
            _accessor = accessor; _configuration = configuration;
            _identidade = identidade;
        }

        public override async Task<bool> CreateList(IEnumerable<Cliente> entity)
        {
            try
            {
                using DataTable _dTable = entity.ToDataTable<Cliente>();
                IDCreated = await _dTable.MapperDatatableToPostgres<Cliente>(ConnectionString).ConfigureAwait(true);
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


        public override async Task<IEnumerable<Cliente>> GetData()
        {
            return await Task.FromResult(new List<Cliente>()).ConfigureAwait(false);
        }

        public async override Task<long> CreateOrUpdate(Cliente entity)
        {
            try
            {
                var _query = $@"select * from public.ssp_alteraragenda( precisa criar function );";

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
    }
}
