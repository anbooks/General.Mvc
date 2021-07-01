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
using General.Services.Supplier;
using General.Services.SysRole;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/Supplier")]
    public class SupplierController : AdminPermissionController
    {
        private ISupplierService _sysSupplierService;

        private ISysRoleService _sysRoleService;
        public SupplierController(ISupplierService sysSupplierService)
        {
 
            this._sysSupplierService = sysSupplierService;
        }  
        [Function("供应商列表", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 4)]
        [Route("SupplierIndex", Name = "SupplierIndex")]
        public IActionResult SupplierIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _sysSupplierService.searchSupplier(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Supplier, SysCustomizedListSearchArg>("SupplierIndex", arg);

            return View(dataSource);
        }
        [HttpGet]
        [Route("editSupplier", Name = "editSupplier")]
        [Function("编辑供应商列表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.SupplierController.SupplierIndex")]
        public IActionResult EditSupplier(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("SupplierIndex");
            if (id != null)
            {
                var model = _sysSupplierService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("editSupplier")]
        public ActionResult EditSupplier(Entities.Supplier model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("SupplierIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (!String.IsNullOrEmpty(model.SupplierCode))
                model.SupplierCode = model.SupplierCode;
           if (!String.IsNullOrEmpty(model.Describe))
             model.Describe = model.Describe.Trim();
           // if (!String.IsNullOrEmpty(model.Description))
          //      model.Description = model.Description.Trim();

            if (model.Id == 0)
            {
                var a = _sysSupplierService.existAccount(model.SupplierCode);
                if (a!=true)
                {
                    _sysSupplierService.insertSupplier(model);
                }
                else
                {
                    Response.WriteAsync("<script>alert('供应商代码重复!');window.location.href ='SupplierIndex'</script>", Encoding.GetEncoding("GB2312"));
                }
                
            }
            else
            {
                var b = _sysSupplierService.getCount(model.SupplierCode,model.Id);
                if (b.Count <1 )
                {
                    _sysSupplierService.updateSupplier(model);
                }
                else
                {
                    Response.WriteAsync("<script>alert('供应商代码重复!');window.location.href ='SupplierIndex'</script>", Encoding.GetEncoding("GB2312"));
                }
               
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}