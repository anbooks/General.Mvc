using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Controllers
{
    [Route("login")]
    public class LoginController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}