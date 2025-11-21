using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Enuns
{
    public interface IGroupPolicies
    {
        public enum UserRoles
        {
            User = 1,
            Vendedor = 2,
            Enfermeira = 3,
            Gerente = 4,
            Diretor = 5,
            Admin = 6,
            Developer = 7,
        }

        public enum Policy
        {
            User = 1,
            Vendedor = 2,
            Enfermeira = 3,
            Gerente = 4,
            Diretor = 5,
            Admin = 6,
            Developer = 7,
        }
    }
}
