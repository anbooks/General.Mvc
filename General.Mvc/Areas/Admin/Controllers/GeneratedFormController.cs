using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("表单生成", true, "menu-icon fa fa-pencil-square-o", Sort = 6)]
    public class GeneratedFormController : AdminPermissionController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}