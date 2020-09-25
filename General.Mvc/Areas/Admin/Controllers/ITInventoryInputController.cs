using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itInventoryInput")]
    public class ITInventoryInputController : AdminPermissionController
    {
        [Route("", Name = "itInventoryInput")]
        [Function("填写核注清单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITInventoryInputIndex()
        {
            return View();
        }
    }
}