using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace IdentityServer4MVC.Controllers
{
    public class AccountController : Controller
    {
        public async Task<IActionResult> SignOut()
        {
            return new SignOutResult(new[] { "Cookies", "oidc" }, new AuthenticationProperties { RedirectUri = "/" });
        }
    }
}