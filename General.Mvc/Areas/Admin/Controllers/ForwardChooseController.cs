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
using General.Services.ForwardChoose;
using General.Services.SysRole;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/ForwardChoose")]
    public class ForwardChooseController : AdminPermissionController
    {
        private IForwardChooseService _sysForwardChooseService;

        private ISysRoleService _sysRoleService;
        public ForwardChooseController(IForwardChooseService sysForwardChooseService)
        {
 
            this._sysForwardChooseService = sysForwardChooseService;
        }  
        [Function("口岸报关行选择规则", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 4)]
        [Route("ForwardChooseIndex", Name = "ForwardChooseIndex")]
        public IActionResult ForwardChooseIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _sysForwardChooseService.searchForwardChoose(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ForwardChoose, SysCustomizedListSearchArg>("ForwardChooseIndex", arg);

            return View(dataSource);
        }
        [HttpGet]
        [Route("editForwardChoose", Name = "editForwardChoose")]
        [Function("编辑口岸报关行选择规则", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ForwardChooseController.ForwardChooseIndex")]
        public IActionResult EditForwardChoose(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("ForwardChooseIndex");
            if (id != null)
            {
                var model = _sysForwardChooseService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("editForwardChoose")]
        public ActionResult EditForwardChoose(Entities.ForwardChoose model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("ForwardChooseIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (!String.IsNullOrEmpty(model.Forward))
                model.Forward = model.Forward;
           if (!String.IsNullOrEmpty(model.Dest))
             model.Dest = model.Dest.Trim();
           // if (!String.IsNullOrEmpty(model.Description))
          //      model.Description = model.Description.Trim();

            if (model.Id == 0)
            {
                var a = _sysForwardChooseService.existAccount(model.Dest,model.Forward);
                if (a!=true)
                {
                    _sysForwardChooseService.insertForwardChoose(model);
                }
                else
                {
                    Response.WriteAsync("<script>alert('规则重复!');window.location.href ='ForwardChooseIndex'</script>", Encoding.GetEncoding("GB2312"));
                }
                
            }
            else
            {
                var b = _sysForwardChooseService.getCount(model.Dest, model.Forward);
                if (b.Count <1 )
                {
                    _sysForwardChooseService.updateForwardChoose(model);
                }
                else
                {
                    Response.WriteAsync("<script>alert('规则重复!');window.location.href ='ForwardChooseIndex'</script>", Encoding.GetEncoding("GB2312"));
                }
               
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}