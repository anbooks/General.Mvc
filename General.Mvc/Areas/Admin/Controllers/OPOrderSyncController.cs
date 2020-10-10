using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/opOrderSync")]
    public class OPOrderSyncController : AdminPermissionController
    {
        [Route("", Name = "opOrderSync")]
        [Function("同步订单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.OutwardProcessingController", Sort = 1)]

        public IActionResult OPOrderSyncIndex()
        {
            return View();
        }
    }
}