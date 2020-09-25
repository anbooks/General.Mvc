using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itCustomsBrokerSelect")]
    public class ITCustomsBrokerSelectController : AdminPermissionController
    {
        [Route("", Name = "itCustomsBrokerSelect")]
        [Function("选择报关行", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 6)]

        public IActionResult ITCustomsBrokerSelectIndex()
        {
            return View();
        }
    }
}