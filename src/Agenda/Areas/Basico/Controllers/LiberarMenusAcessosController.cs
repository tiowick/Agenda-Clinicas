using Agenda.Controllers;
using Agenda.Dominio.Interfaces.Autenticacao;
using Agenda.Dominio.Reflection;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using CRM_Aplicacao.Entidades.Acessos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using System.Text;
using static Agenda.Dominio.Enuns.IGroupPolicies;
using static Agenda.Dominio.Enuns.IResponseController;


namespace CRM_Vendas.Areas.Basico.Controllers
{
    [Area("Basico")]
    [Authorize]
    public class LiberarMenusAcessosController : BasicController
    {
        public LiberarMenusAcessosController(
            [FromServices] IWebHostEnvironment environment
            , IHttpContextAccessor context
            , IConfiguration configuration
            , SignInManager<IdentityUser> SignInManager
            , UserManager<IdentityUser> UserManager
            , IPrincipal principal
            , IUser user
            , IStoreRoles storeRoles)
            //Definição de acesso / niveis de seguraça
            : base(environment, Policy.User, context, configuration, SignInManager, UserManager, principal, user, storeRoles) { }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task.FromResult(View()).ConfigureAwait(false);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> MenusLiberados(long? idModulo)
        {
            try
            {
                using var app = new PerfilAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
                var _resultado = await app.CarregarMenusLiberadosAcessos(idModulo);

                if (_resultado == null)
                    throw new Exception("Erro ao carregar os menus de acessos");

                var _menus = new StringBuilder();
                foreach (var item in _resultado)
                    _menus.AppendLine(item.ToString());

                return await ResponseJson(ResponseJsonTypes.Success, data: _menus.ToString());
            }
            catch (TratamentoExcecao e) { return await ResponseJson(ResponseJsonTypes.Error, e.Message); }
            catch (Exception ex) { return await ResponseJson(ResponseJsonTypes.Error, ex.Message); }
        }


    }
}
