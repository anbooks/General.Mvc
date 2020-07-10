using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    //[Area("Admin")]
    //public class MainController : AdminAreaController
    public class MainController : PublicAdminController  //需要登录才可以查看，后台管理的主页面
    {
        public IActionResult Index()
        {
            var user = WorkContext.CurrentUser;
            return View();
        }
    }
}