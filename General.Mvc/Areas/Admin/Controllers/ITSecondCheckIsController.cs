using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itSecondCheckIs")]
    public class ITSecondCheckIsController : AdminPermissionController
    {
        [Route("", Name = "itSecondCheckIs")]
        [Function("是否需要二检", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITSecondCheckIsIndex()
        {
            return View();
        }
    }
}