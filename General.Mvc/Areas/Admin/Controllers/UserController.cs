using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/users")]
    public class UserController : AdminPermissionController
    {

        //private ISysUserService _sysUserService;

        public UserController()
        {
          // this._sysUserService = sysUserService;
        }

        [Function("系统用户", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        [Route("", Name = "userIndex")]
        public IActionResult UserIndex()
        {
            return View();
        }
    }
}