using Agenda.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Gestao_Winsiga.Apresentacao.Controllers
{

    [AllowAnonymous]
    [AutoValidateAntiforgeryToken]
    public class AcessosController : Controller
    {

        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        private readonly ILogger<AcessosController> _logger;


        public AcessosController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager, ILogger<AcessosController> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index([FromForm] LoginViewModel requestForm)
        {
            try
            {
                if (requestForm == null || string.IsNullOrEmpty(requestForm.Email) || string.IsNullOrEmpty(requestForm.Senha))
                {
                    return Json(new { success = false, message = "Usuário e senha são obrigatórios." });
                }

                var email = requestForm.Email.Trim().ToLowerInvariant();
                var password = requestForm.Senha;

                var user = await _userManager.FindByEmailAsync(email) ?? await _userManager.FindByNameAsync(email);
                if (user == null){ return Json(new { success = false, message = "Usuário ou senha inválidos." }); }
                var result = await _signInManager.PasswordSignInAsync(user.UserName, password, isPersistent: true, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    await _userManager.UpdateSecurityStampAsync(user);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),
                       
                    };

                    foreach (var role in roles)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, role));
                    }

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    await _signInManager.RefreshSignInAsync(user);

                    var redirectUrl = Url.Action("Index", "Calendario", new { area = "Agenda" });
                    return Json(new { success = true, redirectUrl });
                }

                // Se chegou aqui, a senha estava incorreta
                return Json(new { success = false, message = "Usuário ou senha inválidos." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro inesperado durante o login.");
                return Json(new { success = false, message = "Ocorreu um erro no servidor. Tente novamente mais tarde." });
            }
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Logout()
        {
            if (_signInManager != null)
            {
                await _signInManager.SignOutAsync();

                // Limpeza adicional de cookies
                Response.Cookies.Delete(".AspNetCore.Identity.Application");
                Response.Cookies.Delete(".AspNetCore.Antiforgery");

                // Invalida o cache do usuário
                HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult EsqueceuSenha()
        {
            return View();
        }

    }
}
