using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;
using General.Services.ImportTrans_main_recordService;
using General.Entities;
using General.Framework.Datatable;
using General.Services.SysCustomizedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using General.Services.ScheduleService;
using General.Services.SysUser;
using General.Services.SysRole;
using General.Services.SysUserRole;
using General.Core.Librs;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using General.Services.Order;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itDetail")]
    public class ITDetailController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private ISysUserRoleService _sysUserRoleService;
        public ITDetailController(ISysUserRoleService sysUserRoleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysUserRoleService = sysUserRoleService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itDetail")]
        [Function("查询界面(新)", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]
        [HttpGet]
        public IActionResult ITDetailIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var USER = _sysUserRoleService.getById(WorkContext.CurrentUser.Id);
            ViewBag.Role = USER.RoleName;
            var pageList = _importTrans_main_recordService.searchListSecondCheck(arg, page, size, USER.RoleName);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itDetail", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("")]
        public ActionResult ITDetailIndex(List<int> sysResource, string returnUrl = null)
        {
            string submit = Request.Form["submit"];
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itDetail");

            //if (submit.Equals("1"))
            //{
            //    _importTrans_main_recordService.updateSecondCheck(sysResource, WorkContext.CurrentUser.Id);
            //    AjaxData.Status = true;
            //    AjaxData.Message = "确认二检成功";
            //}
            //else if (submit.Equals("2"))
            //{
            //    _importTrans_main_recordService.saveSecondCheck(sysResource);
            //    AjaxData.Status = true;
            //    AjaxData.Message = "确认提交成功";
            //}


            return Redirect(ViewBag.ReturnUrl);
            //return View();
        }
       

    }
}
