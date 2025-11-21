using Agenda.Aplicacao.Interfaces;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Agenda.Aplicacao
{
    [DebuggerStepThrough]
    public class BaseAppServicos<TEntity> 
        : IDisposable
        , IBaseAppServicos<TEntity> where TEntity : class
    {
        private IBaseServicos<TEntity> _servicosBase { get; set; } = default!;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;

        public TransferenciaIdentidadeDTO Identidade { get; private set; } = default!;
        public bool ErrorRepositorio { get; protected set; } = default!;
        public string MessageError { get; protected set; } = default!;
        public long ID { get; set; } = default!;
        public bool IDCreated { get; set; } = default!;

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~BaseAppServicos()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<long> CreateOrUpdate(TEntity entity)
        {
            var _return = await _servicosBase.CreateOrUpdate(entity).ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
        }

        public virtual async Task<bool> CreateList(IEnumerable<TEntity> entity)
        {
            var _return = await _servicosBase.CreateList(entity).ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            IDCreated = _servicosBase.IDCreated;
            return _return;
        }

        public virtual async Task<bool> Delete(long id)
        {
            var _return = await _servicosBase.Delete(id).ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
        }

        public virtual async Task<bool> DeleteList(IEnumerable<long> id)
        {
            var _return = await _servicosBase.DeleteList(id).ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
        }

        public virtual async Task<IEnumerable<TEntity>> GetData(long id)
        {
            var _return = await _servicosBase.GetData(id).ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
        }

        public virtual async Task<IEnumerable<TEntity>> GetData()
        {
            var _return = await _servicosBase.GetData().ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
        }


        public void SetBaseServicos(IBaseServicos<TEntity> servicosBase)
        {
            _servicosBase = servicosBase;
        }

        public BaseAppServicos(IBaseServicos<TEntity> servicosBase, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : this(accessor, configuration, identidade)
        {
            _servicosBase = servicosBase;
        }
        public BaseAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            Identidade = identidade;

            var _rota = "<br />Rota Controller:" + identidade?.RotaController ?? "";

            if (identidade == null)
                throw new Exception("Usuário sem permissão de acesso a está função do sistema: Identidade do Usuário Vazio" + _rota);

            if (!(identidade?.IsAuthorized ?? false))
                throw new Exception("Usuário sem permissão de acesso a está função do sistema: Nível de Acesso Negado" + _rota);

            if (configuration == null)
                throw new Exception("Usuário sem permissão de acesso a está função do sistema: Configuração de Acesso Vazio" + _rota);
        }
    }
}
