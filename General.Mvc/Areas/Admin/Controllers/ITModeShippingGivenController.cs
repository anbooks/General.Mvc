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
    [Route("admin/iTModeShippingGiven")]
    public class ITModeShippingGivenController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;

        public ITModeShippingGivenController(IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itModeShippingGiven")]
        [Function("填写运输方式", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 4)]
        [HttpGet]
        public IActionResult ITModeShippingGivenIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
            {
                RolePermissionViewModel model = new RolePermissionViewModel();
                var customizedList = _sysCustomizedListService.getByAccount("货币类型");
                ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
                var pageList = _importTrans_main_recordService.searchListShipModel(arg, page, size);
                ViewBag.Arg = arg;//传参数
                var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itShipmentCreate", arg);
                return View(dataSource);//sysImport
            }

        [HttpPost]
        [Route("")]
        public ActionResult ITModeShippingGivenIndex(List<int> sysResource, List<string> sysResource2)
        {
            //string test = "sdasdad";
            _importTrans_main_recordService.saveShippingMode(sysResource);
            AjaxData.Status = true;
            AjaxData.Message = "确认创建成功";
            return Json(AjaxData);
            //return View();
        }
        [HttpGet]
        [Route("edit", Name = "editITModeShippingGiven")]
        [Function("编辑运输方式", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITModeShippingGivenController.ITModeShippingGivenIndex")]
        public IActionResult EditITModeShippingGiven(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itModeShippingGiven");
            var customizedList = _sysCustomizedListService.getByAccount("运输方式");
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
        public ActionResult EditITModeShippingGiven(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itModeShippingGiven");
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
                model.ShippingMode = model.ShippingMode.Trim();
                model.ShippingModeGivenTime = DateTime.Now;
                model.ShippingModeGiver = WorkContext.CurrentUser.Id;

                _importTrans_main_recordService.updateShippingMode(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}
