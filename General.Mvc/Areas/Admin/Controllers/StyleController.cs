using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/styles")]
    public class StyleController : AdminPermissionController
    {
        /// <summary>
        /// 样式的列表
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "styleIndex")]
        [Function("样式列表", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.CodingLibraryController", Sort = 1)]

        public IActionResult StyleIndex()
        {
            //http://localhost:50491/ace-master/index.html
            //XXXXXXX---return View("../../ace-master/index.html");
            //Redirect是让浏览器重定向到新的地址
            //建议创建在wwwroot项目文件下
            return Redirect("/ace-master/index.html");
            // return View();
        }




    }
}