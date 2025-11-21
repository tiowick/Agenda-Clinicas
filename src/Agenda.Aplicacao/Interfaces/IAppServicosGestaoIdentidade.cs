using Agenda.Dominio.Entidades.DTOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Interfaces
{
    public interface IAppServicosGestaoIdentidade : IDisposable
    {
        bool ErrorRepositorio { get; }
        string MessageError { get; }
        Task<TransferenciaIdentidadeDTO> GetCredenciaisUsuario();
        Task<TransferenciaIdentidadeDTO> GetLogoutUsuario();
    }
}
