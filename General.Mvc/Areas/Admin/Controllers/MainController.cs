using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Security.Admin;
using General.Services.ImportTrans_main_recordService;
using General.Services.SysUserRole;
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
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysUserRoleService _sysUserRoleService;
        private IAdminAuthService _adminAuthService;

        public MainController(IImportTrans_main_recordService importTrans_main_recordService, IAdminAuthService adminAuthService, ISysUserRoleService sysUserRoleService)
        {
            this._adminAuthService = adminAuthService;
            this._sysUserRoleService = sysUserRoleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
        }

        [Route("", Name = "mainIndex")]
        public IActionResult Index()
        {
            var userrole = _sysUserRoleService.getById(WorkContext.CurrentUser.Id);
            if (userrole!=null) {
                var list = _importTrans_main_recordService.getCount(WorkContext.CurrentUser.Co, userrole.RoleName);
                if (list != null)
                {
                    ViewBag.Count = "当前有" + list.Count + "个任务";
                }
            }
            return View();
        }
        [Route("pas", Name = "passworda")]
        public IActionResult password()
        {
            //_adminAuthService.signOut();
            return RedirectToRoute("password");
        }
        [Route("userme", Name = "usermessage")]
        public IActionResult usermessage()
        {
            //_adminAuthService.signOut();
            return RedirectToRoute("usermessages");
        }
        [Route("out", Name = "signOut")]
        public IActionResult SignOut()
        {
            _adminAuthService.signOut();
            return RedirectToRoute("adminLogin");
        }

    }
}