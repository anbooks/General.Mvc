using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itCheckAndPass")]
    public class ITCheckAndPassController : AdminPermissionController
    {
        [Route("", Name = "itCheckAndPass")]
        [Function("核放", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 11)]

        public IActionResult ITCheckAndPassIndex()
        {
            return View();
        }
    }
}