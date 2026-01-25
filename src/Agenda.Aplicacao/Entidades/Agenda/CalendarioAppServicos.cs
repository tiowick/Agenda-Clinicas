using Agenda.Aplicacao.Interfaces.Agenda;
using Agenda.Dominio;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Entidades.DTO;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Servicos.Agenda;
using Agenda.Infra.Padronizar.Components;
using Agenda.Repositorio.Repositorios.Agenda;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Aplicacao.Entidades.Agenda
{
    public class CalendarioAppServicos
        : BaseAppServicos<Calendario>
        , ICalendarioAppServicos
        , IDisposable
    {
        private readonly ICalendarioServicos _servico;
        private readonly IUser? _accessor;
        private readonly IConfiguration? _configuration;

        public CalendarioAppServicos(IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            ICalendarioRepositorio _repositorio = new CalendarioRepositorio(_accessor, _configuration, identidade);
            _servico = new CalendarioServicos(_repositorio, _accessor, _configuration, identidade);
            SetBaseServicos(_servico);

        }

        public CalendarioAppServicos(ICalendarioServicos servico, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(servico, accessor, configuration, identidade)
        {
            _accessor = accessor;
            _configuration = configuration;
            ICalendarioRepositorio _repositorio = new CalendarioRepositorio(_accessor, _configuration, identidade);
            if (servico == null)
            {
                _servico = new CalendarioServicos(_repositorio, _accessor, _configuration, identidade);
                base.SetBaseServicos(_servico);
            }
            else
                _servico = servico;
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboEmpresas(string search, int page, int? length = 10)
        {
            var result = await _servico.CarregarComboEmpresas(search, page, length).ConfigureAwait(true);
            return result.GetOptionsSelect2(page);
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboStatus(string search, int page, int? length = 10)
        {
            var result = await _servico.CarregarComboStatus(search, page, length).ConfigureAwait(true);
            return result.GetOptionsSelect2(page);
        }

        public async Task<IEnumerable<DataSelect2DTO>> CarregarComboTipoSolitacao(string search, int page, int? length = 10)
        {
            var result = await _servico.CarregarComboTipoSolitacao(search, page, length).ConfigureAwait(true);
            return result.GetOptionsSelect2(page);
        }

        public async Task<RetornoGridPaginado<Calendario>> CarregarGridEnventosCalendario(DataTableSearch search, int start, int draw, int length = 10)
        {
            return await _servico.CarregarGridEnventosCalendario(search, start, draw, length).ConfigureAwait(true);
        }


    }
}
