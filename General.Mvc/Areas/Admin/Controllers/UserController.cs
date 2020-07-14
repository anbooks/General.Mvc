using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.SysUser;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/users")]
    public class UserController : AdminPermissionController
    {

        private ISysUserService _sysUserService;

        public UserController(ISysUserService sysUserService)
        {
            this._sysUserService = sysUserService;
        }

        //[Function("系统用户", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        //[Route("", Name = "userIndex")]
        //public IActionResult UserIndex()
        //{
        //    return View();
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Function("系统用户", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        [Route("", Name = "userIndex")]
        public IActionResult UserIndex(SysUserSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _sysUserService.searchUser(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.SysUser, SysUserSearchArg>("userIndex", arg);
        
            return View(dataSource);
        }







    }
}