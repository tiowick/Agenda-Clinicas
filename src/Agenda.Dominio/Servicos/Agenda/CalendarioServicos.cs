using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Servicos.Agenda
{
    public sealed class CalendarioServicos
        : BaseServicos<Calendario>
        , ICalendarioServicos
    {

        private readonly ICalendarioRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;

        public CalendarioServicos(ICalendarioRepositorio CalendarioServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(CalendarioServicos, accessor, configuration, identidade)
        {
            _repositorio = CalendarioServicos;
            _user = accessor;
            _configuration = configuration;
        }

        public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(DataTableSearch search, int start, int draw, int length = 10)
        {
            var _result = await _repositorio.CarregarGridEnventosCalendario(search, start, draw, length).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }
        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboStatus(string search, int page, int? length = 10)
        {
            var _result = await _repositorio.CarregarComboStatus(search, page, length).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboTipoSolicitacao(string search, int page, int? length = 10)
        {
            var _result = await _repositorio.CarregarComboTipoSolicitacao(search, page, length).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result;
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboEmpresas(string search, int page, int? length = 10)
        {
            var _result = await _repositorio.CarregarComboEmpresas(search, page, length).ConfigureAwait(true);
            ErrorRepositorio = _repositorio.ErrorRepositorio;
            MessageError = _repositorio.MessageError;
            return _result; ;
        }
    }
}
