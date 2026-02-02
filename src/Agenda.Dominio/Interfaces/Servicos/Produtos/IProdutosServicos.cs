using Agenda.Aplicacao.Entidades.Produtos;
using Agenda.Dominio.Entidades.Agenda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Interfaces.Servicos.Produtos
{
    public interface IProdutosServicos 
        : IBaseServicos<Produto>
        , IDisposable
    {

    }
}
