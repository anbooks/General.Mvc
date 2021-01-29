using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("付款流程", true, "menu-icon fa fa-pencil-square-o", Sort = 3)]
    public class CashingController : AdminPermissionController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}