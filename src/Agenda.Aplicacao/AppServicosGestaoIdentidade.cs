

using Agenda.Aplicacao.Interfaces;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Servicos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using CRM_Repositorio.Repositorios;
using Microsoft.Extensions.Configuration;

namespace Agenda.Aplicacao
{
    public class AppServicosGestaoIdentidade 
        : IAppServicosGestaoIdentidade
        , IDisposable
    {
        private bool disposedValue;

        private IServicosGestaoIdentidade _servicosBase { get; set; } = default!;
        public bool ErrorRepositorio { get; private set; } = default!;

        public string MessageError { get; private set; } = default!;

        public AppServicosGestaoIdentidade(IConfiguration? configuration, IUser? accessor)
        {
            if (configuration == null)
                throw new Exception("Erro de permissão - Valide acessos e regras de níveis de acesso: Error CS0001 (AppServicosGestaoIdentidade)");

            if (accessor == null)
                throw new Exception("Erro de permissão - Valide acessos e regras de níveis de acesso: Error CS0002 (AppServicosGestaoIdentidade)");


            IRepositorioGestaoIdentidade repositorio = new RepositorioGestaoIdentidade(configuration, accessor);
            _servicosBase = new ServicosGestaoIdentidade(repositorio, accessor, configuration);
        }

        public async Task<TransferenciaIdentidadeDTO> GetCredenciaisUsuario()
        {
            var _return = await _servicosBase.GetCredenciaisUsuario().ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
        }


        public async Task<TransferenciaIdentidadeDTO> GetLogoutUsuario()
        {
            var _return = await _servicosBase.GetLogoutUsuario().ConfigureAwait(true);
            ErrorRepositorio = _servicosBase.ErrorRepositorio;
            MessageError = _servicosBase.MessageError;
            return _return;
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
        // ~AppServicosGestaoIdentidade()
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
