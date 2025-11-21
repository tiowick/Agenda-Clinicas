using System.Diagnostics;

namespace Agenda.Models
{
    [DebuggerStepThrough]
    public abstract class RequestToken
    {
        public string __RequestVerificationToken { get; set; } = default!;
    }
}
