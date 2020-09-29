using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etCPAInventoryInput")]
    public class ETCPAInventoryInputController : AdminPermissionController
    {
        [Route("", Name = "etCPAInventoryInput")]
        [Function("综保区填写核注清单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETCPAInventoryInputIndex()
        {
            return View();
        }
    }
}