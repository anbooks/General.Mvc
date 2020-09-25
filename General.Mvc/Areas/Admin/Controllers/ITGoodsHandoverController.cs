using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itGoodsHandover")]
    public class ITGoodsHandoverController : AdminPermissionController
    {
        [Route("", Name = "itGoodsHandover")]
        [Function("填写货物交接信息", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 10)]

        public IActionResult ITGoodsHandoverIndex()
        {
            return View();
        }
    }
}