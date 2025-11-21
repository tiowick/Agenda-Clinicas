using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Reflection;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;

namespace Agenda.Dominio.Servicos
{
    [DebuggerStepThrough]
    public class BaseServicos<TEntity> : IDisposable, IBaseServicos<TEntity> where TEntity : class
    {
        private readonly IBaseRepositorio<TEntity> _repositorio = default!;
        private readonly IUser? _acessor = default!;
        private readonly IConfiguration? _configuration = default!;

        private bool disposedValue;

        public TransferenciaIdentidadeDTO Identidade { get; private set; } = default!;
        public bool ErrorRepositorio { get; protected set; } = default!;
        public string MessageError { get; protected set; } = default!;
        public long ID { get; set; } = default!;
        public bool IDCreated { get; set; } = default!;

        public BaseServicos(IBaseRepositorio<TEntity> repositorio, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
        {
            _repositorio = repositorio;
            _acessor = accessor;
            _configuration = configuration;
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

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }
        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        ~BaseServicos()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: false);
        }
        void IDisposable.Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public virtual async Task<long> CreateOrUpdate(TEntity entity)
        {
            try
            {
                await _repositorio.CreateOrUpdate(entity);
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);

                ErrorRepositorio = _repositorio.ErrorRepositorio;
                MessageError = _repositorio.MessageError;

                ID = _repositorio.ID;
                return await Task.FromResult(_repositorio.ID).ConfigureAwait(true);

            }
            catch (Exception ex)
            {
                ID = 0;
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

        public virtual async Task<bool> CreateList(IEnumerable<TEntity> entity)
        {
            try
            {
                await _repositorio.CreateList(entity);
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);

                ErrorRepositorio = _repositorio.ErrorRepositorio;
                MessageError = _repositorio.MessageError;

                IDCreated = _repositorio.IDCreated;
                return await Task.FromResult(_repositorio.IDCreated).ConfigureAwait(true);

            }
            catch (Exception ex)
            {
                IDCreated = false;
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

        public virtual async Task<bool> Delete(long id)
        {
            ID = id;
            try
            {
                await _repositorio.Delete(id);
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);

                ErrorRepositorio = _repositorio.ErrorRepositorio;
                MessageError = _repositorio.MessageError;

                return await Task.FromResult(ErrorRepositorio).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ID = 0;
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

        public virtual async Task<bool> DeleteList(IEnumerable<long> id)
        {
            try
            {
                await _repositorio.DeleteList(id);
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);

                ErrorRepositorio = _repositorio.ErrorRepositorio;
                MessageError = _repositorio.MessageError;

                return await Task.FromResult(ErrorRepositorio).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ID = 0;
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetData(long id)
        {
            try
            {
                return await _repositorio.GetData(id).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

        public virtual async Task<IEnumerable<TEntity>> GetData()
        {
            try
            {
                return await _repositorio.GetData().ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

     
    }
}
