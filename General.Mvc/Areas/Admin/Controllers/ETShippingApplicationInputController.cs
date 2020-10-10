using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etShippingApplicationInput")]
    public class ETShippingApplicationInputController : AdminPermissionController
    {
        [Route("", Name = "etShippingApplicationInput")]
        [Function("出口运输申请", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETShippingApplicationInputIndex()
        {
            return View();
        }
    }
}