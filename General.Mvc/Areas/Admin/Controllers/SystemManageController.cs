using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using General.Framework.Menu;
using General.Framework.Controllers.Admin;
using Microsoft.EntityFrameworkCore;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("系统管理", true, "menu-icon fa fa-desktop")]
    public class SystemManageController : AdminPermissionController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}