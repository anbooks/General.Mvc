using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Ordinary;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Ordinary.Controllers
{
 
    [Route("ordinary/styles")]
    public class StyleController : OrdinaryPermissionController
    {
        /// <summary>
        /// 样式的列表
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "o_styleIndex")]
        [Function("样式列表", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Ordinary.Controllers.CodingLibraryController", Sort = 1)]

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