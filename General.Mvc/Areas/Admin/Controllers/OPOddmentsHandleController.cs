using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/opOddmentsHandle")]
    public class OPOddmentsHandleController : AdminPermissionController
    {
        [Route("", Name = "opOddmentsHandle")]
        [Function("余料处理", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.OutwardProcessingController", Sort = 5)]

        public IActionResult OPOddmentsHandleIndex()
        {
            return View();
        }
    }
}