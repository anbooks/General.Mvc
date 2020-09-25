using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/iTModeShippingGiven")]
    public class ITModeShippingGivenController : AdminPermissionController
    {
        [Route("", Name = "itModeShippingGiven")]
        [Function("填写运输方式", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 4)]

        public IActionResult ITModeShippingGivenIndex()
        {
            return View();
        }
    }
}