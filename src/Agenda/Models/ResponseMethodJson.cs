using System.Diagnostics;

namespace Agenda.Models
{
    [DebuggerStepThrough]
    public class ResponseMethodJson
    {
        public string JsonTypes { get; set; } = default!;
        public string Mensagem { get; set; } = default!;
        public object? Data { get; set; }
        public long? RecordsTotal { get; set; }
    }
}
