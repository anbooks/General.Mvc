using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etDeliveryStatusInput")]
    public class ETDeliveryStatusInputController : AdminPermissionController
    {
        [Route("", Name = "etDeliveryStatusInput")]
        [Function("填写出口运输状态", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETDeliveryStatusInputIndex()
        {
            return View();
        }
    }
}