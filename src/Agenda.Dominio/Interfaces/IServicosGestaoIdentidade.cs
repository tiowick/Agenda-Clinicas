using Agenda.Dominio.Entidades.DTOS;

namespace Agenda.Dominio.Interfaces
{
    public interface IServicosGestaoIdentidade : IDisposable
    {
        bool ErrorRepositorio { get; }
        string MessageError { get; }
        Task<TransferenciaIdentidadeDTO> GetCredenciaisUsuario();
        Task<TransferenciaIdentidadeDTO> GetLogoutUsuario();
    }
}
