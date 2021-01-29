using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("进口运输", true, "menu-icon fa fa-pencil-square-o", Sort = 1)]
    public class ImportTransportationController : AdminPermissionController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}