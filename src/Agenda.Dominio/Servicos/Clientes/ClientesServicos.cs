using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Clientes;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Clientes;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Clientes;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Servicos.Clientes
{
    public class ClientesServicos
        : BaseServicos<Cliente>
        , IClientesServicos
    {

        private readonly IClientesRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;

        public ClientesServicos(IClientesRepositorio ClienteServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(ClienteServicos, accessor, configuration, identidade)
        {
            _repositorio = ClienteServicos;
            _user = accessor;
            _configuration = configuration;
        }


    }
}
