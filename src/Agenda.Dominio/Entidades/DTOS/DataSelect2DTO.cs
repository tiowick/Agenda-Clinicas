using System.Diagnostics;

namespace Agenda.Dominio.Entidades.DTO
{
    [DebuggerStepThrough]
    public class DataSelect2DTO
    {
        public string? id { get; set; } = default!;
        public string text { get; set; } = default!;

        public IEnumerable<DataSelect2DTO>? children { get; set; } = default!;
        public string element { get; set; } = "HTMLOptionElement";
        public string grpoption { get; set; } = default!;
        public string label { get; set; } = default!;
    }
}
