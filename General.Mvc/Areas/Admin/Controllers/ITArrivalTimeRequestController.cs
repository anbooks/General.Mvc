using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itArrivalTimeRequest")]
    public class ITArrivalTimeRequestController : AdminPermissionController
    {
        [Route("", Name = "itArrivalTimeRequest")]
        [Function("要求到货日期", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITArrivalTimeRequestIndex()
        {
            return View();
        }
    }
}