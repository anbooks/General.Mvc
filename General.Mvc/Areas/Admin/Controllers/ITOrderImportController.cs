using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    //[Area("Admin")]
    [Route("admin/itOrderImport")]
    public class ITOrderImportController : AdminPermissionController
    {
        [Route("", Name = "itOrderImportIndex")]
        [Function("订单数据导入", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITOrderImportIndex()
        {
            return View();
        }
    }
}