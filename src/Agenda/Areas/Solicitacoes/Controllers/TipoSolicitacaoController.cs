using Agenda.Controllers;
using Agenda.Dominio.Interfaces.Autenticacao;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using static Agenda.Dominio.Enuns.IGroupPolicies;

namespace Agenda.Areas.Solicitacoes.Controllers
{
    [Authorize]
    [Area("Solicitacoes")]
    public class TipoSolicitacaoController : BasicController
    {
        public TipoSolicitacaoController(
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


    }
}
