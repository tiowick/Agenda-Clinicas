using Agenda.Aplicacao.Interfaces.Agenda;
using Agenda.Aplicacao.Interfaces.Empresas;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Entidades.Empresas;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Empresas;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Empresas;
using Agenda.Dominio.Servicos.Agenda;
using Agenda.Dominio.Servicos.Empresas;
using Agenda.Repositorio.Repositorios.Agenda;
using Agenda.Repositorio.Repositorios.Empresas;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Entidades.Empresas
{
    public class EmpresasAppServicos 
        : BaseAppServicos<Empresa>
        , IEmpresasAppServicos
        , IDisposable
    {
        private readonly IEmpresasServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        public EmpresasAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IEmpresasRepositorio _repositorio = new EmpresasRepositorio(_accessor, _configuration, identidade);
            _servico = new EmpresaServicos(_repositorio, _accessor, _configuration, identidade);
            SetBaseServicos(_servico);

        }

        public EmpresasAppServicos(IEmpresasServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IEmpresasRepositorio _repositorio = new EmpresasRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new EmpresaServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }

    }
}
