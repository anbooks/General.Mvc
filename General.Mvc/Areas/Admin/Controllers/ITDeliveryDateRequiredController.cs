using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itDeliveryDateRequired")]
    public class ITDeliveryDateRequiredController : AdminPermissionController
    {
        [Route("", Name = "itDeliveryDateRequired")]
        [Function("要求送货日期", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 9)]

        public IActionResult ITDeliveryDateRequiredIndex()
        {
            return View();
        }
    }
}