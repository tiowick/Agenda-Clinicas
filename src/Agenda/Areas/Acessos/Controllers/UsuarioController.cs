using Agenda.Aplicacao.Entidades.Acessos;
using Agenda.Controllers;
using Agenda.Dominio.Entidades.Acessos;
using Agenda.Dominio.Entidades.Acessos.DTO;
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

namespace Agenda.Areas.Acessos.Controllers
{

    [Authorize]
    [Area("Acessos")]
    public class UsuarioController : BasicController
    {
        public UsuarioController([FromServices] IWebHostEnvironment environment
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
        public async Task<JsonResult> AlterarUsuarios([FromForm] UsuarioDTO dados)
        {
            try
            {
                using var app = new UsuariosAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
                var _logon = dados?.Logon?.ToLower(culture: CultureInfo.CurrentCulture).Trim() ?? "";
                var _email = dados?.Email?.ToLower(culture: CultureInfo.CurrentCulture).Trim() ?? "";
                var _senha = dados?.Senha?.ToLower(culture: CultureInfo.CurrentCulture).Trim() ?? "";

                if (dados == null)
                    throw new TratamentoExcecao("Objeto vazio");

                if (string.IsNullOrEmpty(_logon) || string.IsNullOrEmpty(_email))
                    throw new TratamentoExcecao("email ou logon não informado");

                var _dados = new Usuarios
                {
                    ID = dados.ID,
                    Status = (dados?.Status ?? 0),
                    Cidade = (dados?.Cidade ?? ""),
                    Nome = (dados?.Descricao ?? ""),
                    Documento = (dados?.Documento ?? ""),
                    DtCriacao = DateTime.Now,
                    Email = _email,
                    IdClains = "not update",
                    IdEmpresa = base.Identidade.IdEmpresaLogado,
                    IdVendedor = (dados?.IdVendedor ?? 0),
                    UF = (dados?.UF ?? ""),
                    Senha = _senha,
                    Logon = _logon,
                };

                var _idUsuario = dados?.ID ?? 0;
                var _validarCriacao = await app.ValidarCriacaoUsuario(_idUsuario, _dados.Email, _dados.Documento).ConfigureAwait(false);
                if (!string.IsNullOrEmpty(_validarCriacao))
                {
                    throw new Exception(_validarCriacao.Replace('\r', ' ').Replace("\n", "<br />"));
                }

                Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("pt-BR");
                var context = new ValidationContext(_dados, serviceProvider: null, items: null);
                var validationResults = new List<ValidationResult>();
                bool isValid = Validator.TryValidateObject(_dados, context, validationResults, true);
                validationResults ??= new List<ValidationResult>();

                if (!isValid)
                {
                    var _validSenha = validationResults?.Where(x => x?.MemberNames?.FirstOrDefault()?.ToLower(culture: CultureInfo.CurrentCulture) == "senha")?.FirstOrDefault();
                    if ((dados?.ID ?? 0) != 0 && _validSenha != null)
                        validationResults?.Remove(_validSenha);
                }

                validationResults ??= new List<ValidationResult>();

                if (validationResults.Any())
                {
                    var _erroMensagem = (validationResults?.FirstOrDefault()?.ErrorMessage ?? "Erro no processamento").ToLower(culture: CultureInfo.CurrentCulture);
                    throw new TratamentoExcecao(_erroMensagem.Traduzir());
                }

                var user = new IdentityUser();

                // --- INICIO DO BLOCO IDENTITY ---

                if ((dados?.ID ?? 0) == 0) // CRIAÇÃO
                {
                    //cria a referencia no identity
                    var _securitystamp = Guid.NewGuid().ToString().ToLower(culture: CultureInfo.CurrentCulture);
                    var _concurrencystamp = Guid.NewGuid().ToString().ToLower(culture: CultureInfo.CurrentCulture);

                    user = new IdentityUser
                    {
                        UserName = _logon,
                        NormalizedUserName = _logon,
                        Email = _email,
                        NormalizedEmail = _email,
                        EmailConfirmed = true,
                        SecurityStamp = _securitystamp,
                        ConcurrencyStamp = _concurrencystamp,
                        PhoneNumber = null,
                        PhoneNumberConfirmed = false,
                        TwoFactorEnabled = false,
                        LockoutEnd = null,
                        LockoutEnabled = false,
                        AccessFailedCount = 0
                    };

                    if (UserManager == null)
                        throw new TratamentoExcecao("Erro de criação de usuários");

                    var _create = await UserManager.CreateAsync(user, _senha).ConfigureAwait(false);
                    if (!_create.Succeeded)
                        throw new TratamentoExcecao(_create?.Errors?.FirstOrDefault()?.Description ?? "");
                }
                else
                {
                    if (UserManager == null)
                        throw new TratamentoExcecao("Erro de alteração de usuários");

                    var dadosAtuaisList = await app.GetData(_idUsuario).ConfigureAwait(false);
                    var dadosAtuais = dadosAtuaisList.FirstOrDefault();
                    string senhaNoBanco = dadosAtuais?.Senha?.ToLower(culture: CultureInfo.CurrentCulture).Trim() ?? "";

                    // 2. Busca o usuário no Identity
                    user = await UserManager.FindByNameAsync(_logon).ConfigureAwait(false);
                    user ??= await UserManager.FindByEmailAsync(_email).ConfigureAwait(false);

                    if (user == null)
                        throw new TratamentoExcecao("Erro a localização para alteração do usuário (Identity)");

                    // 3. Atualiza propriedades básicas do Identity para manter sincronia
                    user.Email = _email;
                    user.UserName = _logon;

                    if (_senha != senhaNoBanco && !string.IsNullOrEmpty(_senha))
                    {
                        var token = await UserManager.GeneratePasswordResetTokenAsync(user);
                        var resultSenha = await UserManager.ResetPasswordAsync(user, token, _senha);

                        if (!resultSenha.Succeeded)
                            throw new TratamentoExcecao("Erro ao atualizar a senha no Identity: " + resultSenha.Errors.FirstOrDefault()?.Description);
                    }

                    // 5. Persiste as alterações no Identity (Email/Logon)
                    var updateResult = await UserManager.UpdateAsync(user);
                    if (!updateResult.Succeeded)
                        throw new TratamentoExcecao("Erro ao atualizar dados do Identity: " + updateResult.Errors.FirstOrDefault()?.Description);
                }

                // --- FIM DO BLOCO IDENTITY ---

                _dados.IdClains = user.Id;

                _ = await app.CreateOrUpdate(_dados);

                return await ResponseJson(ResponseJsonTypes.Success);
            }
            catch (TratamentoExcecao e) { return await ResponseJson(ResponseJsonTypes.Error, e.Message); }
            catch (Exception ex) { return await ResponseJson(ResponseJsonTypes.Error, ex.Message); }
        }

        public async Task<JsonResult> Editar(long idItem)
        {
            try
            {
                using var app = new UsuariosAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
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
                using var app = new UsuariosAppServicos(base.UserIdentity, base.Configuration, base.Identidade);
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
