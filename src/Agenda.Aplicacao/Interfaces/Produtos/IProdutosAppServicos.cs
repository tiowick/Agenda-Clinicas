using Agenda.Aplicacao.Entidades.Produtos;
using Agenda.Dominio.Entidades.Agenda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Interfaces.Produtos
{
    public interface IProdutosAppServicos 
        : IBaseAppServicos<Produto>
        , IDisposable
    {

    }
}
