using System.Diagnostics;

namespace Agenda.Infra.Padronizar.CriacaoTabelas
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AtributosTabelas : Attribute
    {
        public string Name { get; private set; } = default!;
        public int StartIdentity { get; private set; } = default!;
        public AtributosTabelas(int startIdentity = -1) { StartIdentity = startIdentity; }
        public AtributosTabelas(string name) { Name = name; StartIdentity = -1; }
        public AtributosTabelas(string name, int startIdentity = -1)
            : this(name) => StartIdentity = startIdentity;
    }
}
