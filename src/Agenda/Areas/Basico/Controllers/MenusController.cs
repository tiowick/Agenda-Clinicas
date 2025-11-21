using Agenda.Aplicacao.Entidades.Basico;
using Agenda.Controllers;
using Agenda.Dominio.Entidades.DataTablePaginado;
using Agenda.Dominio.Interfaces.Autenticacao;
using Agenda.Dominio.Reflection;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Text.Json;
using static Agenda.Dominio.Enuns.IGroupPolicies;
using static Agenda.Dominio.Enuns.IResponseController;

namespace Agenda.Areas.Basico.Controllers
{

    [Authorize]
    [Area("Basico")]
    public class MenusController : BasicController
    {
        public MenusController(
             [FromServices] IWebHostEnvironment environment
             , IHttpContextAccessor context
             , IConfiguration configuration
             , SignInManager<IdentityUser> SignInManager
             , UserManager<IdentityUser> UserManager
             , IPrincipal principal
             , IUser user
             , IStoreRoles storeRoles)
             : base(environment, Policy.Admin, context, configuration, SignInManager, UserManager, principal, user, storeRoles) { }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View()).ConfigureAwait(false);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> CarregarGridMenus(DataTableSearch? search = null, int start = 0, int length = 0, int draw = 0)
        {
            try
            {
                search ??= new DataTableSearch
                {
                    value = "",
                };
                using var app = new MenusAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
                var _result = Json(await app.CarregarGridMenus(search, start, length, draw), new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                return await Task.FromResult(_result).ConfigureAwait(false);
            }
            catch (TratamentoExcecao e) { return await ResponseJson(ResponseJsonTypes.Error, e.Message); }
            catch (Exception ex) { return await ResponseJson(ResponseJsonTypes.Error, ex.Message); }
        }


    }
}
