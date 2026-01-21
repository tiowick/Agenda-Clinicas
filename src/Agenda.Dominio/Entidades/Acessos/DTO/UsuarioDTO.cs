using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Entidades.Acessos.DTO
{
    [DebuggerStepThrough]
    public class UsuarioDTO
    {
        public long ID { get; set; } = default!;
        public long IdCliente { get; set; } = default!;
        public long IdVendedor { get; set; } = default!;
        public long IdPromotora { get; set; } = default!;
        public string Documento { get; set; } = default!;
        //public string Matricula { get; set; } = default!;
        public string Descricao { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Cidade { get; set; } = default!;
        public string UF { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public string Logon { get; set; } = default!;
        public int Status { get; set; } = default!;
    }
}
