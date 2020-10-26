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
    [Route("admin/itCustomsBrokerSelect")]
    public class ITCustomsBrokerSelectController : AdminPermissionController
    {
          private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;

        public ITCustomsBrokerSelectController(IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itCustomsBrokerSelect")]
        [Function("选择报关行5", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 6)]
        [HttpGet]
        public IActionResult ITCustomsBrokerSelectIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _importTrans_main_recordService.searchListCustomsBrokerSelect(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itCustomsBrokerSelect", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("")]
        public ActionResult ITCustomsBrokerSelectIndex(List<int> sysResource, List<string> sysResource2)
        {
            //string test = "sdasdad";
            _importTrans_main_recordService.saveCustomsBrokerSelect(sysResource);
            AjaxData.Status = true;
            AjaxData.Message = "确认创建成功";
            return Json(AjaxData);
            //return View();
        }
        [HttpGet]
        [Route("edit", Name = "edititCustomsBrokerSelect")]
        [Function("选择报关行", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITCustomsBrokerSelectController.ITCustomsBrokerSelectIndex")]
        public IActionResult EditITCustomsBrokerSelect(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itCustomsBrokerSelect");
            var customizedList = _sysCustomizedListService.getByAccount("报关行");
            ViewData["Forwarderlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            if (id != null)
            {
                var model = _importTrans_main_recordService.getById(id.Value);

                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditITCustomsBrokerSelect(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itCustomsBrokerSelect");
            if (!ModelState.IsValid)
                return View(model);

             if (!String.IsNullOrEmpty(model.Forwarder))
               model.Forwarder = model.Forwarder.Trim();
            if (model.Id.Equals(0))
            {
                return Redirect(ViewBag.ReturnUrl);
            }
            else
            {
                model.Forwarder = model.Forwarder.Trim();
                model.CustomsBrokerSelectTime = DateTime.Now;
                model.CustomsBrokerSelecter = WorkContext.CurrentUser.Id;

                _importTrans_main_recordService.updateCustomsBrokerSelect(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}
