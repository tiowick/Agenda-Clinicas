using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Entidades.Empresas;
using Agenda.Dominio.Interfaces.Repositorio.Agenda;
using Agenda.Dominio.Interfaces.Repositorio.Empresas;
using Agenda.Dominio.Interfaces.Servicos.Agenda;
using Agenda.Dominio.Interfaces.Servicos.Empresas;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Dominio.Servicos.Empresas
{
    public class EmpresaServicos : 
        BaseServicos<Empresa>
        , IEmpresasServicos
    {
        private readonly IEmpresasRepositorio _repositorio = default!;
        private readonly IUser? _user = default!;
        private readonly IConfiguration? _configuration = default!;

        public EmpresaServicos(IEmpresasRepositorio EmpresasServicos, IUser? accessor, IConfiguration? configuration, TransferenciaIdentidadeDTO identidade)
            : base(EmpresasServicos, accessor, configuration, identidade)
        {
            _repositorio = EmpresasServicos;
            _user = accessor;
            _configuration = configuration;
        }


    }

}
