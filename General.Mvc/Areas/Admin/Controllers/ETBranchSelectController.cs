using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etBranchSelect")]
    public class ETBranchSelectController : AdminPermissionController
    {
        [Route("", Name = "etBranchSelect")]
        [Function("流程分支选择", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETBranchSelectIndex()
        {
            return View();
        }
    }
}