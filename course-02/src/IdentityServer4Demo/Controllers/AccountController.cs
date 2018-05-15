// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4Demo.Identity;
using IdentityServer4Demo.Models;
using IdentityServer4Demo.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;


namespace IdentityServer4Demo.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger _logger;

        private readonly ConfigurationDbContext _clientStore;

        private readonly UserManager<IdentityServerDemoIdentityUser> _userManager;
        private readonly SignInManager<IdentityServerDemoIdentityUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;

        public AccountController(
            ILogger<AccountController> logger,
            ConfigurationDbContext clientStore,
            UserManager<IdentityServerDemoIdentityUser> userManager,
            SignInManager<IdentityServerDemoIdentityUser> signInManager,
            IIdentityServerInteractionService interaction)
        {
            _logger = logger;

            _clientStore = clientStore;

            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("~/");
            }
            var user = new IdentityServerDemoIdentityUser { UserName = model.UserName };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return Redirect("~/");
            }
            await _signInManager.SignInAsync(user, isPersistent: false);
            return Ok(model.ReturnUrl);
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

            var vm = await BuildLoginViewModelAsync(returnUrl, context);
            return View(vm);
        }

        async Task<LoginViewModel> BuildLoginViewModelAsync(string returnUrl, AuthorizationRequest context)
        {

            var providers = HttpContext.Authentication.GetAuthenticationSchemes()
                .Where(x => x.DisplayName != null)
                .Select(x => new ExternalProvider
                {
                    DisplayName = x.DisplayName,
                    AuthenticationScheme = x.AuthenticationScheme
                });

            var allowLocal = true;
            if (context?.ClientId != null)
            {
                var client = _clientStore.Clients.FirstOrDefault(c => c.ClientId == context.ClientId);
                if (client != null)
                {
                    allowLocal = client.EnableLocalLogin;

                    if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                    {
                        providers = providers.Where(provider =>
                        client.IdentityProviderRestrictions.Exists(r => r.Provider == provider.AuthenticationScheme));
                    }
                }
            }
            var vm = await Task.FromResult(new LoginViewModel
            {
                AllowRememberLogin = allowLocal,
                ReturnUrl = returnUrl,
                UserName = context?.LoginHint,
                ExternalProviders = providers.ToArray()
            });
            return vm;
        }

        /// <summary>
        /// 本地登录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Redirect("~/");
            }
            if (!ValidateRtnUrl(model.ReturnUrl))
            {
                return Redirect("~/");
            }

            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (context is null)
            {
                return Redirect("~/");
            }
            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null)
            {
                return Redirect("~/");
            }
            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return Redirect("~/");
            }
            AuthenticationProperties properties = null;
            if (model.AllowRememberLogin)
            {
                properties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                };
            }
            await _signInManager.SignInAsync(user, properties);
            return Redirect(model.ReturnUrl);
        }
        private async Task<LogoutViewModel> BuildLogoutViewModelAsync(string logoutId)
        {
            var vm = new LogoutViewModel { LogoutId = logoutId, ShowLogoutPrompt = AccountOptions.ShowLogoutPrompt };

            if (User?.Identity.IsAuthenticated != true)
            {
                // if the user is not authenticated, then just show logged out page
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                vm.ShowLogoutPrompt = false;
                return vm;
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            return vm;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                // if the user is not authenticated, then just show logged out page
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            var context = await _interaction.GetLogoutContextAsync(logoutId);
            if (context?.ShowSignoutPrompt == false)
            {
                // it's safe to automatically sign-out
                return await Logout(new LogoutViewModel { LogoutId = logoutId });
            }

            // show the logout prompt. this prevents attacks where the user
            // is automatically signed out by another malicious web page.
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };

            return View(vm);
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("account/logout")]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {

            var idp = User?.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;
            var subjectId = HttpContext.User.Identity.GetSubjectId();
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                if (model.LogoutId == null)
                {
                    // if there's no current logout context, we need to create one
                    // this captures necessary info from the current logged in user
                    // before we signout and redirect away to the external IdP for signout
                    model.LogoutId = await _interaction.CreateLogoutContextAsync();
                }

                var url = "/Account/Logout?logoutId=" + model.LogoutId;
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.SignOutAsync(idp, new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException ex)
                {
                    //return BadRequest(OperateResult.Failed("", ex.InnerException.Message));
                }
            }

            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // set this so UI rendering sees an anonymous user
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };
            return Redirect(vm.PostLogoutRedirectUri);
        }

        /// <summary>
        /// 验证回调地址
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        bool ValidateRtnUrl(string returnUrl)
        {
            if (!_interaction.IsValidReturnUrl(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                return false;
            }
            return true;
        }
    }
}