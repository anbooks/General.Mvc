using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Ordinary;
using General.Framework.Security.Ordinary;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Ordinary.Controllers
{
    [Area("Ordinary")]
    [Route("ordinary/main")]
    public class MainController : PublicOrdinaryController   //这样Controller一集成就得登录后才能用了
    {
        private IOrdinaryAuthService _ordinaryAuthService;

        public MainController(IOrdinaryAuthService ordinaryAuthService)
        {
            this._ordinaryAuthService = ordinaryAuthService;
        }

        [Route("", Name = "o_mainIndex")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("o_out", Name = "o_signOut")]
        public IActionResult SignOut()
        {
            _ordinaryAuthService.signOut();
            return RedirectToRoute("ordinaryLogin");
        }

    }
}