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
    [Route("admin/itDeliveryStatusInput")]
    public class ITDeliveryStatusInputController : AdminPermissionController
    {
          private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;

        public ITDeliveryStatusInputController(IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itDeliveryStatusInput")]
        [Function("录入运输状态", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 5)]
        [HttpGet]
        public IActionResult ITDeliveryStatusInputIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _importTrans_main_recordService.searchListDeliveryStatus(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itShipmentCreate", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("")]
        public ActionResult ITDeliveryStatusInputIndex(List<int> sysResource, List<string> sysResource2)
        {
            //string test = "sdasdad";
            _importTrans_main_recordService.saveDeliveryStatus(sysResource);
            AjaxData.Status = true;
            AjaxData.Message = "确认创建成功";
            return Json(AjaxData);
            //return View();
        }
        [HttpGet]
        [Route("edit", Name = "editITDeliveryStatusInput")]
        [Function("录入运输状态", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITDeliveryStatusInputController.ITDeliveryStatusInputIndex")]
        public IActionResult EditITDeliveryStatusInput(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itDeliveryStatusInput");
            var customizedList = _sysCustomizedListService.getByAccount("运输状态");
            ViewData["ShippingModelist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

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
        public ActionResult EditITDeliveryStatusInput(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itDeliveryStatusInput");
            if (!ModelState.IsValid)
                return View(model);

            // if (!String.IsNullOrEmpty(model.CustomizedValue))
            //    model.CustomizedValue = model.CustomizedValue.Trim();
            if (model.Id.Equals(0))
            {
                return Redirect(ViewBag.ReturnUrl);
            }
            else
            {
                model.Status = model.Status.Trim();
                model.DeliveryStatusInputTime = DateTime.Now;
                model.DeliveryStatusInputer = WorkContext.CurrentUser.Id;

                _importTrans_main_recordService.updateDeliveryStatus(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}
