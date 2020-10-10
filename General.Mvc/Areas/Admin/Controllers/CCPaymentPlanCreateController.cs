using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/ccPaymentPlanCreate")]
    public class CCPaymentPlanCreateController : AdminPermissionController
    {
        [Route("", Name = "ccPaymentPlanCreate")]
        [Function("编制付款计划", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.CashingController", Sort = 1)]

        public IActionResult CCPaymentPlanCreateIndex()
        {
            return View();
        }
    }
}