using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("出口运输", true, "menu-icon fa fa-pencil-square-o", Sort = 5)]
    public class ExportTransportationController : AdminPermissionController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}