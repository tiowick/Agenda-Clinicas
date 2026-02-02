using Agenda.Aplicacao.Interfaces.Agenda;
using Agenda.Aplicacao.Interfaces.Produtos;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Produtos;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Produtos;
using Agenda.Dominio.Servicos.Agenda;
using Agenda.Dominio.Servicos.Produtos;
using Agenda.Repositorio.Repositorios.Agenda;
using Agenda.Repositorio.Repositorios.Produtos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Entidades.Produtos
{
    public class ProdutosAppServicos 
        : BaseAppServicos<Produto>
        , IProdutosAppServicos
        , IDisposable
    {

        private readonly IProdutosServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;

        public ProdutosAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IProdutosRepositorio _repositorio = new ProdutosRepositorio(_accessor, _configuration, identidade);
            _servico = new ProdutosServicos(_repositorio, _accessor, _configuration, identidade);
            SetBaseServicos(_servico);

        }

        public ProdutosAppServicos(IProdutosServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IProdutosRepositorio _repositorio = new ProdutosRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new ProdutosServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }
    }
}
