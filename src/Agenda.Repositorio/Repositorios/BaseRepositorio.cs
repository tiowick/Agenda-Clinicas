using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Reflection;
using Agenda.Infra.Padronizar.Texto;
using Agenda.Repositorio.Servicos;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Agenda.Repositorio.Repositorios
{
    [DebuggerStepThrough]
    public abstract class BaseRepositorio<TEntity>
        : IDisposable
        , IBaseRepositorio<TEntity> where TEntity : class, new()
    {
        public string ConnectionString { get; private set; } = default!;
        public bool ErrorRepositorio { get; protected set; } = default!;
        public string MessageError { get; protected set; } = default!;
        public long ID { get; set; } = default!;
        public bool IDCreated { get; set; } = default!;

        public TransferenciaIdentidadeDTO Identidade { get; }

        private readonly string Table;

        private bool disposedValue;
        private string msgErroDominio = "A definição de dominio não foi feita corretamente:<br /> falta incluir [Table(\"{0}\", Schema = \"{1}\")]<br />[DebuggerStepThrough]";
        public BaseRepositorio(IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
        {
            Table = typeof(TEntity).ToSchemaTable();

            var _rota = "<br />Rota Controller:" + identidade?.RotaController ?? "";
            if (identidade == null)
                throw new Exception("Usuário sem permissão de acesso a está função do sistema: Identidade do Usuário Vazio" + _rota);

            if (!(identidade?.IsAuthorized ?? false))
                throw new Exception("Usuário sem permissão de acesso a está função do sistema: Nível de Acesso Negado" + _rota);

            if (configuration == null)
                throw new Exception("Usuário sem permissão de acesso a está função do sistema: Configuração de Acesso Vazio" + _rota);

            if (string.IsNullOrEmpty(Table))
            {
                var _erro = typeof(TEntity).FullName.ReverseString().Split('.');
                var _table = "";
                var _schame = "";
                if (_erro.Length >= 2)
                {
                    _table = _erro[0].ReverseString();
                    _schame = _erro[1].ReverseString();
                }
                else
                    _table = _erro[0].ReverseString();

                msgErroDominio = string.Format(msgErroDominio, _table, _schame);
            }

            ConnectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? "Server=bd1.winsiga.com.br; Port=5432; User Id=postgres; Password=soft@2013; Database=DadosAgendaBTG;";
            Identidade = identidade;
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }
                disposedValue = true;
            }
        }
        ~BaseRepositorio()
        {
            Dispose(disposing: false);
        }

        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        public abstract Task<IEnumerable<TEntity>> GetData();

        public abstract Task<long> CreateOrUpdate(TEntity entity);

        public abstract Task<bool> CreateList(IEnumerable<TEntity> entity);

        public virtual async Task<bool> Delete(long id)
        {
            try
            {
                if (string.IsNullOrEmpty(Table))
                    throw new Exception(msgErroDominio);

                var _query = string.Format("delete from " + Table + " where id = {0};", id);
                var cn = new SqlSystemConnect(ConnectionString);

                cn.Execute(_query, commandTimeout: 1440);
                ErrorRepositorio = false;
                MessageError = "";
                return await Task.FromResult(true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                ErrorRepositorio = true;
                MessageError = e.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

        public virtual async Task<bool> DeleteList(IEnumerable<long> id)
        {
            try
            {
                if (string.IsNullOrEmpty(Table))
                    throw new Exception(msgErroDominio);

                var itens = id.GroupBy(g => g.ToString()).ToList();
                var cn = new SqlSystemConnect(ConnectionString);
                foreach (var item in itens)
                {
                    var _query = string.Format("delete from " + Table + " where id = {0};", item.Key);
                    cn.Execute(_query, commandTimeout: 1440);
                }

                ErrorRepositorio = false;
                MessageError = "";
                return await Task.FromResult(true).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                ErrorRepositorio = true;
                MessageError = e.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetData(long id)
        {
            try
            {
                if (string.IsNullOrEmpty(Table))
                    throw new Exception(msgErroDominio);

                var cn = new SqlSystemConnect(ConnectionString);
                var _query = string.Format("SELECT * FROM " + Table + " WHERE id = {0} LIMIT 1;", id);

                var _return = cn.Query<TEntity>(_query);
                ErrorRepositorio = false;
                MessageError = "";
                return await Task.FromResult(_return).ConfigureAwait(true);
            }
            catch (Exception e)
            {
                ErrorRepositorio = true;
                MessageError = e.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }
    }
}