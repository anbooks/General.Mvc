using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Function("数据导入", true, "menu-icon fa fa-pencil-square-o")]
    public class DataImportController : AdminPermissionController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}