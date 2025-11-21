using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces;
using Agenda.Dominio.Reflection;
using Agenda.Infra.Padronizar.Texto;
using Agenda.Repositorio.Servicos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;

namespace CRM_Repositorio.Repositorios
{
    public class RepositorioGestaoIdentidade
        : IRepositorioGestaoIdentidade
        , IDisposable
    {
        private readonly IUser? _accessor;
        private bool disposedValue;
        public string ConnectionString { get; private set; } = default!;
        public bool ErrorRepositorio { get; private set; } = default!;
        public string MessageError { get; private set; } = default!;

        public RepositorioGestaoIdentidade(IConfiguration? configuration, IUser? accessor)
        {
            ConnectionString = configuration?.GetConnectionString("DefaultConnection")
                ?? "Server=bd1.winsiga.com.br; Port=5432; User Id=postgres; Password=soft@2013; Database=DadosAgendaBTG;";
            _accessor = accessor;
        }

        public async Task<TransferenciaIdentidadeDTO> GetCredenciaisUsuario()
        {
            try
            {
                var _name = _accessor?.Name ?? "";
                var _query = string.Format("SELECT * FROM public.sfn_get_credenciais_usuario('{0}')", _name); // criar via sql mesmo!!
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<TransferenciaIdentidadeDTO>(_query, buffered: true, commandTimeout: 1440).FirstOrDefault();

                if (_result == null)
                    return await Task.FromResult(new TransferenciaIdentidadeDTO() { IdVendedorLogado = 0 }).ConfigureAwait(false);

                return await Task.FromResult(_result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }

        #region get logout
        // método que seria de logout, Logica
        public async Task<TransferenciaIdentidadeDTO> GetLogoutUsuario()
        {
            try
            {
                var _name = _accessor?.Name ?? "";
                var _query = string.Format("exec acessos.ssp_logoutusuario '{0}'", _name);
                var cn = new SqlSystemConnect(ConnectionString);
                var _result = cn.Query<TransferenciaIdentidadeDTO>(_query, buffered: true, commandTimeout: 1440).FirstOrDefault();

                if (_result == null)
                    return await Task.FromResult(new TransferenciaIdentidadeDTO() { IdVendedorLogado = 0 }).ConfigureAwait(false);


                return await Task.FromResult(_result).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorRepositorio = true;
                MessageError = ex.Message.Traduzir();
                throw new TratamentoExcecao(MessageError);
            }
        }
        #endregion

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
        // ~RepositorioGestaoIdentidade()
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
