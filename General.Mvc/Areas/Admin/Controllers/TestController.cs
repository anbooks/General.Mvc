using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/test")]
    public class TestController : AdminPermissionController
    {
    

        [Route("", Name = "testIndex")]
        [Function("测试入口", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.CodingLibraryController", Sort = 2)]
        public IActionResult TestIndex()
        {
            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("test", Name = "testFr")]
        [Function("测试Finereport", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestFr()
        {
            return View();
        }


    }
}