using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Reflection;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;

namespace Agenda.Dominio.Servicos
{
    public class ServicosGestaoIdentidade 
        : IServicosGestaoIdentidade
        , IDisposable
    {
        private readonly IRepositorioGestaoIdentidade _repositorio = default!;
        private bool disposedValue;

        private static IUser? _accessor {  get; set; }
        private static IConfiguration? _configuration { get; set; }
        public TransferenciaIdentidadeDTO TransferenciaIdentidadeDTO { get; private set; } = default!;

        public bool ErrorRepositorio { get; private set; } = default!;

        public string MessageError { get; private set; } = default!;

        public ServicosGestaoIdentidade(IRepositorioGestaoIdentidade repositorio, IUser? accessor, IConfiguration? configuration)
        {
            _accessor = accessor;
            _configuration = configuration;
            _repositorio = repositorio;
        }

        public async Task<TransferenciaIdentidadeDTO> GetCredenciaisUsuario()
        {
            try
            {
                TransferenciaIdentidadeDTO usuario = await _repositorio.GetCredenciaisUsuario().ConfigureAwait(false);
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);

                ErrorRepositorio = _repositorio.ErrorRepositorio;
                MessageError = _repositorio.MessageError;
                return await Task.FromResult(usuario).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
        }

        public async Task<TransferenciaIdentidadeDTO> GetLogoutUsuario()
        {
            try
            {
                TransferenciaIdentidadeDTO usuario = await _repositorio.GetLogoutUsuario().ConfigureAwait(false);
                if (_repositorio.ErrorRepositorio)
                    throw new Exception(_repositorio.MessageError);

                ErrorRepositorio = _repositorio.ErrorRepositorio;
                MessageError = _repositorio.MessageError;
                return await Task.FromResult(usuario).ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message;
                throw new TratamentoExcecao(ex);
            }
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

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~ServicosGestaoIdentidade()
        // {
        //     // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


    }
}
