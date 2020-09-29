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
using General.Services.SysUser;
using General.Services.SysCustomizedList;
using General.Services.SysRole;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/customizedList")]
    public class CustomizedListController : AdminPermissionController
    {
        private ISysCustomizedListService _sysCustomizedListService;

        private ISysRoleService _sysRoleService;
        public CustomizedListController(ISysCustomizedListService sysCustomizedListService)
        {
 
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Function("下拉列表", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 0)]
        [Route("", Name = "customizedListIndex")]
        public IActionResult CustomizedListIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _sysCustomizedListService.searchList(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.SysCustomizedList, SysCustomizedListSearchArg>("customizedListIndex", arg);

            return View(dataSource);
        }
        [HttpGet]
        [Route("edit", Name = "editlist")]
        [Function("编辑下拉列表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.SysCustomizedListController.CustomizedListIndex")]
        public IActionResult EditList(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("customizedListIndex");
            if (id != null)
            {
                var model = _sysCustomizedListService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditList(Entities.SysCustomizedList model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("customizedListIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (!String.IsNullOrEmpty(model.CustomizedClassify))
                model.CustomizedClassify = model.CustomizedClassify;
            if (!String.IsNullOrEmpty(model.CustomizedValue))
                model.CustomizedValue = model.CustomizedValue.Trim();
            if (!String.IsNullOrEmpty(model.Description))
                model.Description = model.Description.Trim();
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.Sort = 0;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                
                model.Creator = WorkContext.CurrentUser.Id;
                _sysCustomizedListService.insertSysCustomized(model);
            }
            else
            {
                model.ModifiedTime = DateTime.Now;
                model.Modifier = WorkContext.CurrentUser.Id;
                _sysCustomizedListService.updateSysCustomized(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}