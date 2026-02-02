using Agenda.Aplicacao.Entidades.Produtos;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Produtos;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Produtos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Servicos.Produtos
{
    public sealed class ProdutosServicos 
        : BaseServicos<Produto>
        , IProdutosServicos
    {
        private readonly IProdutosRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;
        public ProdutosServicos(IProdutosRepositorio ProdutosServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(ProdutosServicos, accessor, configuration, identidade)
        {
            _repositorio = ProdutosServicos;
            _user = accessor;
            _configuration = configuration;
        }



    }
}
