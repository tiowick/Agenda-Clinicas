using Agenda.Aplicacao;
using Agenda.Dominio.Entidades.DTOS;
using Agenda.Dominio.Interfaces.Autenticacao;
using Agenda.Models;
using Controle_Agenda.Dominio.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Principal;
using System.Text.Json;
using static Agenda.Dominio.Enuns.IGroupPolicies;
using static Agenda.Dominio.Enuns.IResponseController;

namespace Agenda.Controllers
{
    [Authorize]
    public abstract class BasicController : Controller
    {
        public TransferenciaIdentidadeDTO Identidade { get; private set; }
        public bool IsAuthorized { get; private set; }
        public Policy NivelAcessoPermitido { get; private set; }

        public async Task<JsonResult> ResponseJson(ResponseJsonTypes type, string? mensagem = "", object? data = null, long? recordsTotal = 0)
        {
            var _padrao = (int)type == 0 && string.IsNullOrEmpty(mensagem) ? "Operação realizada com sucesso" : mensagem;
            var _return = new ResponseMethodJson
            {
                JsonTypes = type.ToString().ToLower(culture: CultureInfo.CurrentCulture),
                Mensagem = _padrao ?? "Verifique a mensagem para exibição ao usuário final!!! ERR: 0001",
                Data = data,
                RecordsTotal = recordsTotal
            };
            var _response = Task.FromResult(Json(_return, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true })).ConfigureAwait(false);
            return await _response;
        }

        public IWebHostEnvironment Environment { get; set; } = default!;
        public IHttpContextAccessor Context { get; set; } = default!;
        public IConfiguration? Configuration { get; set; } = default!;
        public SignInManager<IdentityUser>? SignInManager { get; set; } = default!;
        public UserManager<IdentityUser>? UserManager { get; set; } = default!;
        public IPrincipal? Principal { get; set; }
        public IUser? UserIdentity { get; set; }
        public IStoreRoles? StoreRoles { get; set; }


        public override ViewResult View()
        {

            var _logado = User?.Identity?.IsAuthenticated ?? false;
            var _autorizado = StoreRoles?.IsAuthorized ?? false;
            var _sessao = User?.Identity?.IsAuthenticated ?? false;

            AcessoNegado acesso = new AcessoNegado();

            if (!_logado || !_autorizado)
            {
                acesso.IsAuthorized = false;
                var url = !_logado ? "../Home/Index" : "../Home/Privacy";
                return base.View(url, acesso);
            }

            return base.View();
        }

        public override ViewResult View(string? viewName, object? model)
        {
            var _logado = User?.Identity?.IsAuthenticated ?? false;
            var _autorizado = StoreRoles?.IsAuthorized ?? false;
            AcessoNegado acesso = new AcessoNegado();

            if (!_logado || !_autorizado)
            {
                acesso.IsAuthorized = false;
                var url = !_logado ? "../Home/Index" : "../Home/Privacy";
                return base.View(url, acesso);
            }

            return base.View(viewName, model);
        }
        public override ViewResult View(string? viewName)
        {
            var _logado = User?.Identity?.IsAuthenticated ?? false;
            var _autorizado = StoreRoles?.IsAuthorized ?? false;
            AcessoNegado acesso = new AcessoNegado();

            if (!_logado || !_autorizado)
            {
                acesso.IsAuthorized = false;
                var url = !_logado ? "../Home/Index" : "../Home/Privacy";
                return base.View(url, acesso);
            }

            return base.View(viewName);
        }
        public override ViewResult View(object? model)
        {
            var _logado = User?.Identity?.IsAuthenticated ?? false;
            var _autorizado = StoreRoles?.IsAuthorized ?? false;
            AcessoNegado acesso = new AcessoNegado();

            if (!_logado || !_autorizado)
            {
                acesso.IsAuthorized = false;
                var url = !_logado ? "../Home/Index" : "../Home/Privacy";
                return base.View(url, acesso);
            }

            return base.View(model);
        }

        public BasicController([FromServices] IWebHostEnvironment environment, Policy policy, IHttpContextAccessor context, IConfiguration? configuration, SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, IPrincipal principal, IUser? user, IStoreRoles storeRoles)
        {
            var _rota = "";

            try
            {
                IsAuthorized = false;
                Environment = environment;
                Context = context;
                Configuration = configuration;
                SignInManager = signInManager;
                UserManager = userManager;
                UserIdentity = user;
                StoreRoles = storeRoles;
                Principal = principal;
                Thread.CurrentPrincipal = principal;

                var cultureInfo = new CultureInfo("pt-BR");
                CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
                CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

                _rota = context?.HttpContext?.Request?.Path.ToString() ?? "";

                using var app = new AppServicosGestaoIdentidade(configuration, user);
                var _credential = Task.Run(async () => await app.GetCredenciaisUsuario()).ConfigureAwait(true).GetAwaiter().GetResult();
                //var _result = app.GetCredenciaisUsuario().Result;
                if (_credential == null)
                    throw new Exception("Erro na tentativa de acessar o sistema");

                //TODO: Ajusta para buscar dados do cliente
                NivelAcessoPermitido = policy;


                IsAuthorized = storeRoles.IsInPolicy(policy);
                Identidade = new TransferenciaIdentidadeDTO
                {
                    IdVendedorLogado = _credential?.IdVendedorLogado ?? 0,
                    IdEmpresaLogado = _credential?.IdEmpresaLogado ?? 0,
                    IdUsuarioLogado = _credential?.IdUsuarioLogado ?? 0,
                    NmUsuarioLogado = _credential?.NmUsuarioLogado ?? "",
                    AutoAgendamento = _credential?.AutoAgendamento ?? 0,
                    IsAuthorized = IsAuthorized,
                    RotaController = _rota
                };
            }
            catch
            {
                Identidade = new TransferenciaIdentidadeDTO
                {
                    AutoAgendamento = 0,
                    IdVendedorLogado = 0,
                    IdEmpresaLogado = 0,
                    IdUsuarioLogado = 0,
                    NmUsuarioLogado = "",
                    IsAuthorized = false,
                    RotaController = _rota
                };
            }

        }


    }
}
