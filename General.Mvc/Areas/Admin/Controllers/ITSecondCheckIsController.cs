using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.ImportTrans_main_recordService;
using General.Services.SysCustomizedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itSecondCheckIs")]
    public class ITSecondCheckIsController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        public ITSecondCheckIsController(IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itSecondCheckIs")]
        [Function("是否完成二检", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITSecondCheckIsIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _importTrans_main_recordService.searchListSecondCheck(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itSecondCheckIs", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("")]
        public ActionResult ITSecondCheckIsIndex(List<int> sysResource, string returnUrl = null)
        {
            string submit = Request.Form["submit"];
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itSecondCheckIs");

            if (submit.Equals("1"))
            {
                _importTrans_main_recordService.updateSecondCheck(sysResource, WorkContext.CurrentUser.Id);
                AjaxData.Status = true;
                AjaxData.Message = "确认二检成功";
            }
            else if (submit.Equals("2"))
            {
                _importTrans_main_recordService.saveSecondCheck(sysResource);
                AjaxData.Status = true;
                AjaxData.Message = "确认提交成功";
            }


            return Redirect(ViewBag.ReturnUrl);
            //return View();
        }
       

    }
}
