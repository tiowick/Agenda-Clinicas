using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Entidades.Empresas;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Empresas;
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

namespace Agenda.Repositorio.Repositorios.Empresas
{
    public class EmpresasRepositorio : 
        BaseRepositorio<Empresa>
        , IEmpresasRepositorio
    {

        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        private readonly TransferenciaIdentidadeDTO _identidade;

        public EmpresasRepositorio(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(configuration, identidade)
        {
            _accessor = accessor; _configuration = configuration;
            _identidade = identidade;
        }

        public override async Task<bool> CreateList(IEnumerable<Empresa> entity)
        {
            try
            {
                using DataTable _dTable = entity.ToDataTable<Empresa>();
                IDCreated = await _dTable.MapperDatatableToPostgres<Empresa>(ConnectionString).ConfigureAwait(true);
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


        public override async Task<IEnumerable<Empresa>> GetData()
        {
            return await Task.FromResult(new List<Empresa>()).ConfigureAwait(false);
        }

        public async override Task<long> CreateOrUpdate(Empresa entity)
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
