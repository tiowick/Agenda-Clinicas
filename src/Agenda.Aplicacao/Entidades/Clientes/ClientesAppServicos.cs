using Agenda.Aplicacao.Interfaces.Agenda;
using Agenda.Aplicacao.Interfaces.Clientes;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Clientes;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Clientes;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Clientes;
using Agenda.Dominio.Servicos.Agenda;
using Agenda.Dominio.Servicos.Clientes;
using Agenda.Repositorio.Repositorios.Agenda;
using Agenda.Repositorio.Repositorios.Clientes;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Entidades.Clientes
{
    public class ClientesAppServicos 
        : BaseAppServicos<Cliente>
        , IClientesAppServicos
        , IDisposable
    {
        private readonly IClientesServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;

        public ClientesAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IClientesRepositorio _repositorio = new ClientesRepositorio(_accessor, _configuration, identidade);
            _servico = new ClientesServicos(_repositorio, _accessor, _configuration, identidade);
            SetBaseServicos(_servico);

        }

        public ClientesAppServicos(IClientesServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IClientesRepositorio _repositorio = new ClientesRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new ClientesServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }
    }
}
