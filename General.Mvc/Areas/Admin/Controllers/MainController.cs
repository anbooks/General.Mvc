using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Security.Admin;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    //    //[Area("Admin")]
    //    //public class MainController : AdminAreaController
    //    [Route("admin/main")]
    //    public class MainController : PublicAdminController  //需要登录才可以查看，后台管理的主页面
    //    {
    //        private IAdminAuthService _adminAuthService;

    //        public MainController(IAdminAuthService adminAuthService)
    //        {
    //            this._adminAuthService = adminAuthService;
    //        }

    //        [Route("",Name ="mainIndex")]
    //        public IActionResult Index()
    //        {
    //            //var user = WorkContext.CurrentUser;

    //            _adminAuthService.getCurrentUser();
    //            return View();
    //        }

    //        [Route("out", Name = "signOut")]
    //        public IActionResult SignOut()
    //        {
    //            _adminAuthService.signOut();
    //            return RedirectToRoute("adminLogin");
    //        }


    //    }
    //}
    [Area("Admin")]
    [Route("admin/main")]
    public class MainController : PublicAdminController   //这样Controller一集成就得登录后才能用了
    {
        private IAdminAuthService _adminAuthService;

        public MainController(IAdminAuthService adminAuthService)
        {
            this._adminAuthService = adminAuthService;
        }

        [Route("", Name = "mainIndex")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("out", Name = "signOut")]
        public IActionResult SignOut()
        {
            _adminAuthService.signOut();
            return RedirectToRoute("adminLogin");
        }

    }
}