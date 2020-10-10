using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etShippingListCreate")]
    public class ETShippingListCreateController : AdminPermissionController
    {
        [Route("", Name = "etShippingListCreate")]
        [Function("创建发运清单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETShippingListCreateIndex()
        {
            return View();
        }
    }
}