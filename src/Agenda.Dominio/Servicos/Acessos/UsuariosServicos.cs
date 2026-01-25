using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Acessos;
using Agenda.Dominio.Interfaces.Servicos.Acessos;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Servicos.Acessos
{
    public sealed class UsuariosServicos
        : BaseServicos<Usuarios>
        , IUsuariosServicos
    {
        private readonly IUsuariosRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;
        public UsuariosServicos(IUsuariosRepositorio UsuariosServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(UsuariosServicos, accessor, configuration, identidade)
        {
            _repositorio = UsuariosServicos;
            _user = accessor;
            _configuration = configuration;
        }

        public async Task<RetornoGridPaginado<Usuarios>> CarregarGridUsuarios(DataTableSearch search, int start, int length, int draw, int status)
        {
            var _result = await _repositorio.CarregarGridUsuarios(search, start, length, draw, status).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }

        public async Task<string> ValidarCriacaoUsuario(long idUsuario, string email, string documento)
        {
            var _result = await _repositorio.ValidarCriacaoUsuario(idUsuario, email, documento).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }
    }
}
