using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Agenda.Dominio.Enuns.IGroupPolicies;

namespace Agenda.Dominio.Interfaces.Autenticacao
{
    public interface IStoreRoles
    {
        public IList<string> Roles { get; set; }
        public bool IsInPolicy(Policy roleName);
        public bool IsInRole(UserRoles roleName);
        public bool IsAuthorized { get; set; }
    }
}
