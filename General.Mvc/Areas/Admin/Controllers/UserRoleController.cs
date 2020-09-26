using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Filters;
using General.Framework.Menu;
using General.Services.SysUserRole;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    public class UserRoleController : Controller
    {
        private ISysUserRoleService _sysUserRoleService;

        public UserRoleController(ISysUserRoleService sysUserRoleService)
        {
            this._sysUserRoleService = sysUserRoleService;
        }
       
    }
}