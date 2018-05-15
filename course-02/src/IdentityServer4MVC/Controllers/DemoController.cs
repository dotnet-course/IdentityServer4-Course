using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer4MVC.Controllers
{
    [Authorize]
    public class DemoController : Controller
    {
        public IActionResult Get()
        {
            return Ok(new[] { "ASP.NET Core", "EntityFramework Core", "RabbitMQ" });
        }
    }
}