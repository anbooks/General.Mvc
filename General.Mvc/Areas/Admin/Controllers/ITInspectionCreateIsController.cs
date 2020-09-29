using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itInspectionCreateIs")]
    public class ITInspectionCreateIsController : AdminPermissionController
    {
        [Route("", Name = "itInspectionCreateIs")]
        [Function("生成送检单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITInspectionCreateIsIndex()
        {
            return View();
        }
    }
}