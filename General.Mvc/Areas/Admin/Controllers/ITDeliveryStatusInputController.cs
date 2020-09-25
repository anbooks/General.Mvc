using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itDeliveryStatusInput")]
    public class ITDeliveryStatusInputController : AdminPermissionController
    {
        [Route("", Name = "itDeliveryStatusInput")]
        [Function("录入运输状态", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 5)]

        public IActionResult ITDeliveryStatusInputIndex()
        {
            return View();
        }
    }
}