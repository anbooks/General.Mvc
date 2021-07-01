using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/ccPaymentDateInput")]
    public class CCPaymentDateInputController : AdminPermissionController
    {
        [Route("", Name = "ccPaymentDateInput")]
        [Function("当月汇率维护", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.CashingController", Sort = 1)]

        public IActionResult CCPaymentDateInputIndex()
        {
            return View();
        }
    }
}