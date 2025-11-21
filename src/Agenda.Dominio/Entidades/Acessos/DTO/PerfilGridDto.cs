using System.Diagnostics;

namespace Agenda.Dominio.Entidades.Acessos.DTO
{
    [DebuggerStepThrough]
    public class PerfilGridDto
    {
        public string ID { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public string Codigo { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string HoraIni { get; set; } = default!;
        public string HoraFim { get; set; } = default!;
        public string ControleIP { get; set; } = default!;
        public string Nivel { get; set; } = default!;
        public long RecordsTotal { get; set; } = default!;
    }
}
