using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Entidades.DTOS
{
    [DebuggerStepThrough]
    public class TransferenciaIdentidadeDTO
    {
        public long IdVendedorLogado { get; set; } = default!;
        public long IdUsuarioLogado { get; set; } = default!;
        public long IdEmpresaLogado { get; set; } = default!;
        public string NmUsuarioLogado { get; set; } = default!;
        public int AutoAgendamento { get; set; } = default!;
        public bool IsAuthorized { get; set; } = default!;
        public string RotaController { get; set; } = default!;
        public long IdCampanha { get; set; } = default!;
    }
}
