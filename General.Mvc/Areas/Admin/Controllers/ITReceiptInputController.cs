using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itReceiptInput")]
    public class ITReceiptInputController : AdminPermissionController
    {
        [Route("", Name = "itReceiptInput")]
        [Function("录入签收单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 12)]

        public IActionResult ITReceiptInputIndex()
        {
            return View();
        }
    }
}