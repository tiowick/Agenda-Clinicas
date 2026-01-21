using Agenda.Aplicacao.Interfaces.Acessos;
using Agenda.Dominio;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Acessos;
using Agenda.Dominio.Interfaces.Servicos.Acessos;
using Agenda.Dominio.Servicos.Acessos;
using Agenda.Repositorio.Repositorios.Acessos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Entidades.Acessos
{
    //Entidade de Dominio: Usuarios
    public class UsuariosAppServicos 
        : BaseAppServicos<Usuarios>
        , IUsuariosAppServicos
        , IDisposable
    {
        private readonly IUsuariosServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;
        public UsuariosAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IUsuariosRepositorio _repositorio = new UsuariosRepositorio(_accessor, _configuration, identidade);
            _servico = new UsuariosServicos(_repositorio, _accessor, _configuration, identidade);
            base.SetBaseServicos(_servico);
        }

        public UsuariosAppServicos(IUsuariosServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            IUsuariosRepositorio _repositorio = new UsuariosRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new UsuariosServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }

        public async Task<RetornoGridPaginado<Usuarios>> CarregarGridUsuarios(DataTableSearch search, int start, int length, int draw, int status)
        {
            return await _servico.CarregarGridUsuarios(search, start, length, draw, status).ConfigureAwait(true);
        }

        public async Task<string> ValidarCriacaoUsuario(long idUsuario, string email, string documento)
        {
            return await _servico.ValidarCriacaoUsuario(idUsuario, email, documento).ConfigureAwait(true);
        }

    }
}
