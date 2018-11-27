using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using DotNetCoreDashboardArch.Models.Login;


namespace DotNetCoreDashboardArch.Controllers
{
    public class LoginController : Controller
    {
        [AllowAnonymous]
        public IActionResult Main()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                var items = new Dictionary<string, string>();
                var parms = new Dictionary<string, object>()
                {
                    {"login", login}
                };
                return Challenge(new AuthenticationProperties(items, parms));
            }
            else
            {
                return new BadRequestResult();
            }

        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        public IActionResult Failure()
        {
            Dictionary<string, string> x = new Dictionary<string, string>
            {
                { "LogonResult", "Failure" }
            };
            return Json(x);
        }
    }
}
