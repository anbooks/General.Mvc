using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.ExportTransportationService;
using General.Services.SysCustomizedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/etShippingApplicationInput")]
    public class ETShippingApplicationInputController : AdminPermissionController
    {
        private IExportTransportationService _exportTransportationService;
        private ISysCustomizedListService _sysCustomizedListService;
        public ETShippingApplicationInputController(IExportTransportationService exportTransportationService, ISysCustomizedListService sysCustomizedListService)
        {

            this._exportTransportationService = exportTransportationService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "etShippingApplicationInput")]
        [Function("出口运输申请", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETShippingApplicationInputIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("付费方式");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            string flag = "出口运输申请";
            var pageList = _exportTransportationService.searchList(arg, page, size,flag);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ExportTransportation, SysCustomizedListSearchArg>("etShippingApplicationInput", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("")]
        public ActionResult ETShippingApplicationInputIndex(List<int> sysResource, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etShippingApplicationInput");
            string flag = "出口运输申请";
            _exportTransportationService.saveExportTransportation(sysResource,flag);
            AjaxData.Status = true;
            AjaxData.Message = "确认创建成功";
            return Redirect(ViewBag.ReturnUrl);
            //return View();
        }
        [HttpGet]
        [Route("edit", Name = "editETShippingApplicationInput")]
        [Function("选择报关行", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ETShippingApplicationInputController.ETShippingApplicationInputIndex")]
        public IActionResult EditETShippingApplicationInput(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etShippingApplicationInput");
            var customizedList = _sysCustomizedListService.getByAccount("发运情况");
            ViewData["DeliverySituationlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var customizedList2 = _sysCustomizedListService.getByAccount("付费方式");
            ViewData["PaymentMethodlist"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");

            if (id != null)
            {
                var model = _exportTransportationService.getById(id.Value);

                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditETShippingApplicationInput(Entities.ExportTransportation model, string returnUrl = null)
        {
            ModelState.Remove("Id");

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etShippingApplicationInput");
            if (!ModelState.IsValid)
                return View(model);
            string flag = "出口运输申请";
            if (!String.IsNullOrEmpty(model.PaymentMethod))
                model.PaymentMethod = model.PaymentMethod.Trim();
            if (!String.IsNullOrEmpty(model.ImportItem))
                model.ImportItem = model.ImportItem.Trim();
            if (model.Id.Equals(0))
            {
                model.ApplyTime = DateTime.Now;
                model.Applier = WorkContext.CurrentUser.Id;

                _exportTransportationService.insertExportTransportation(model);
            }
            else
            {
                  
        
                _exportTransportationService.updateExportTransportation(model,flag);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}
