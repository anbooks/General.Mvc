﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{

    [Route("admin/itShipmentCreate")]
    public class ITShipmentCreateController : AdminPermissionController
    {
        [Route("", Name = "itShipmentCreate")]
        [Function("创建发运条目", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITShipmentCreateIndex()
        {
            return View();
        }
    }
}