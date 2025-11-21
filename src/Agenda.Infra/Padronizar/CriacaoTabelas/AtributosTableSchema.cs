using System.Diagnostics;

namespace Agenda.Infra.Padronizar.CriacaoTabelas
{
    [DebuggerStepThrough]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class AtributosTableSchema : Attribute
    {
        public string Name { get; private set; } = default!;
        public AtributosTableSchema(string name)
        {
            Name = name;
        }
    }
}
