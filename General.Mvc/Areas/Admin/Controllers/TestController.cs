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
        [Route("testFr", Name = "testFr")]  
        [Function("测试Finereport", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestFr()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("testWebservice", Name = "testWebservice")]
        [Function("测试Webservice", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestWebservice()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("nonparam", Name = "nonparamReport")]
       // [Function("新增、修改角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.RoleController.RoleIndex")]
        public IActionResult NonparamReport()
        {
            // http://localhost:8075/WebReport/ReportServer?reportlet=SysUser.cpt

         //  return Redirect("http://192.168.14.107:8075/WebReport/ReportServer?reportlet=SysUser.cpt");  //临时重定向
            return Content("来了老弟");
          
        }



    }
}