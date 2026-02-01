using Agenda.Aplicacao.Entidades.Agenda;
using Agenda.Aplicacao.Entidades.Empresas;
using Agenda.Controllers;
using Agenda.Dominio.Entidades.Agenda;
using Agenda.Dominio.Entidades.Empresas;
using Agenda.Dominio.Interfaces.Autenticacao;
using Agenda.Dominio.Reflection;
using Agenda.Infra.Padronizar.Texto;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Security.Principal;
using static Agenda.Dominio.Enuns.IGroupPolicies;
using static Agenda.Dominio.Enuns.IResponseController;

namespace Agenda.Areas.Empresas.Controllers
{
    [Authorize]
    [Area("Agenda")]
    public class EmpresaController : BasicController
    {
        public EmpresaController(
             [FromServices] IWebHostEnvironment environment
             , IHttpContextAccessor context
             , IConfiguration configuration
             , SignInManager<IdentityUser> SignInManager
             , UserManager<IdentityUser> UserManager
             , IPrincipal principal
             , IUser user
             , IStoreRoles storeRoles)
             //Definição de acesso / niveis de seguraça
             : base(environment, Policy.Gerente, context, configuration, SignInManager, UserManager, principal, user, storeRoles) { }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View()).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> AlterarAgendamentos([FromForm] Empresa dados)
        {
            try
            {
                if (dados == null)
                    throw new Exception("Dados do fomulário vázio");

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
                var context = new ValidationContext(dados, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(dados, context, validationResults, true);
                validationResults ??= new List<ValidationResult>();

                if (validationResults.Any())
                {
                    var _erroMensagem = (validationResults?.FirstOrDefault()?.ErrorMessage ?? "Erro no processamento").ToLower(culture: CultureInfo.CurrentCulture);
                    throw new TratamentoExcecao(_erroMensagem.Traduzir());
                }

                using var app = new EmpresasAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
                _ = await app.CreateOrUpdate(dados);

                return await ResponseJson(ResponseJsonTypes.Success);
            }
            catch (TratamentoExcecao e) { return await ResponseJson(ResponseJsonTypes.Error, e.Message); }
            catch (Exception ex) { return await ResponseJson(ResponseJsonTypes.Error, ex.Message); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Editar(long idItem)
        {
            try
            {
                using var app = new EmpresasAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
                var _result = await app.GetData(idItem);
                var _return = await ResponseJson(ResponseJsonTypes.Success, "", _result.FirstOrDefault()).ConfigureAwait(false);

                return await Task.FromResult(_return).ConfigureAwait(false);
            }
            catch (TratamentoExcecao e) { return await ResponseJson(ResponseJsonTypes.Error, e.Message); }
            catch (Exception ex) { return await ResponseJson(ResponseJsonTypes.Error, ex.Message); }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Excluir(long idItem)
        {
            try
            {
                using var app = new EmpresasAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
                var _result = await app.Delete(idItem);
                var _type = !app.ErrorRepositorio ? ResponseJsonTypes.Success : ResponseJsonTypes.Error;
                var _return = await ResponseJson(_type, app.MessageError, _result).ConfigureAwait(false);

                return await Task.FromResult(_return).ConfigureAwait(false);
            }
            catch (TratamentoExcecao e) { return await ResponseJson(ResponseJsonTypes.Error, e.Message); }
            catch (Exception ex) { return await ResponseJson(ResponseJsonTypes.Error, ex.Message); }
        }



    }
}
