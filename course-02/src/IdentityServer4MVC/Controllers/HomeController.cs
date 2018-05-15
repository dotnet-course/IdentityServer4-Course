// Copyright (c) Jeffcky <see cref="http://www.cnblogs.com/createmyself"/> All rights reserved.
using IdentityServer4MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityServer4MVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
