using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using General.Services.Category;
using General.Services.SysPermission;
using General.Services.SysRole;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/role")]
    public class RoleController : AdminPermissionController
    {

        private ISysRoleService _sysRoleService;
        private ISysPermissionService _sysPermissionService;
        private ICategoryService _categoryService;

        public RoleController(ISysRoleService sysRoleService,
            ICategoryService categoryService,
            ISysPermissionService sysPermissionService)
        {
            this._categoryService = categoryService;
            this._sysRoleService = sysRoleService;
            this._sysPermissionService = sysPermissionService;
        }

     
        /// <summary>
        /// 角色列表
        /// </summary>
        /// <returns></returns>
        [Route("", Name = "roleIndex")]
        [Function("角色列表", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 1)]
        public IActionResult RoleIndex()
        {
            return View(_sysRoleService.getAllRoles());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("edit", Name = "editRole")]
        [Function("新增、修改角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.RoleController.RoleIndex")]
        public IActionResult EditRole(Guid? id = null)
        {
            Entities.SysRole model = null;
            if (id.HasValue && id.Value != Guid.Empty)
                model = _sysRoleService.getRole(id.Value);
            return View(model);
        }
        [Route("edit")]
        [HttpPost]
        public ActionResult EditRole(Entities.SysRole model)
        {
            ModelState.Remove("Id");
            if (!ModelState.IsValid)
                return View(model);
            model.Name = model.Name.Trim();
            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
                model.CreationTime = DateTime.Now;
                model.Creator = WorkContext.CurrentUser.Id;
                _sysRoleService.inserRole(model);
            }
            else
            {
                model.ModifiedTime = DateTime.Now;
                model.Modifier = WorkContext.CurrentUser.Id;
                _sysRoleService.updateRole(model);
            }
            return RedirectToRoute("roleIndex");
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("delete/{id}", Name = "deleteRole")]
        [Function("删除角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.RoleController.RoleIndex")]
        public JsonResult DeleteRole(Guid id)
        {
            _sysRoleService.deleteRole(id);
            AjaxData.Status = true;
            AjaxData.Message = "角色已删除成功";
            return Json(AjaxData);
        }

        /// <summary>
        /// 角色权限设置
        /// </summary>
        /// <returns></returns>
        [Route("permission", Name = "rolePermission")]
        [Function("角色权限设置", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.RoleController.RoleIndex")]
        [HttpGet]
        public ActionResult RolePermission(Guid id)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            model.CategoryList = _categoryService.getAll();
            var roleList = _sysRoleService.getAllRoles();
            if (roleList != null && roleList.Any())
            {
                model.Role = roleList.FirstOrDefault(o => o.Id == id);
                model.RoleList = roleList;
                model.Permissions = _sysPermissionService.getByRoleId(id);
            }
            return View(model);
        }

        [HttpPost]
        [Route("permission")]
        public JsonResult RolePermission(Guid id, List<int> sysResource)
        {
            _sysPermissionService.saveRolePermission(id, sysResource, WorkContext.CurrentUser.Id);
            AjaxData.Status = true;
            AjaxData.Message = "角色权限设置成功";
            return Json(AjaxData);
        }
    }
}