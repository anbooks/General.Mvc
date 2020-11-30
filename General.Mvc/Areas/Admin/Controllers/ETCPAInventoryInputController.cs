using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
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
    [Route("admin/etCPAInventoryInput")]
    public class ETCPAInventoryInputController : AdminPermissionController
    {
        private IExportTransportationService _exportTransportationService;
        private ISysCustomizedListService _sysCustomizedListService;
        public ETCPAInventoryInputController(IExportTransportationService exportTransportationService, ISysCustomizedListService sysCustomizedListService)
        {

            this._exportTransportationService = exportTransportationService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "etCPAInventoryInput")]
        [Function("综保区填写核注清单(新)", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]
      
        public IActionResult ETCPAInventoryInputIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("付费方式");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            string flag = "综保区填写核注清单";
            var pageList = _exportTransportationService.searchList(arg, page, size, flag);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ExportTransportation, SysCustomizedListSearchArg>("etCPAInventoryInput", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("etCPAInventoryInputList", Name = "etCPAInventoryInputList")]
        public ActionResult ETCPAInventoryInputList(string kevin)
        {
            string test = kevin;
            List<Entities.ExportTransportation> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ExportTransportation>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            foreach (Entities.ExportTransportation u in jsonlist)
            {
                var model = _exportTransportationService.getById(u.Id);
                model.NuclearNote = u.NuclearNote;
                model.LicensePlateNo = u.LicensePlateNo;
                model.ManufactureDate = u.ManufactureDate;
               
                //model.Project = u.Project;
                _exportTransportationService.updateExportTransportation(model);
                //u就是jsonlist里面的一个实体类
            }
            AjaxData.Status = true;
            AjaxData.Message = "OK";
            return Json(AjaxData);
        }
        [HttpGet]
        [Route("edit", Name = "editetCPAInventoryInput")]
        [Function("综保区填写核注清单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ETCPAInventoryInputController.ETCPAInventoryInputIndex")]
        public IActionResult EditETCPAInventoryInput(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etCPAInventoryInput");
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
        public ActionResult EditETCPAInventoryInput(Entities.ExportTransportation model, string returnUrl = null)
        {
            ModelState.Remove("Id");

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etCPAInventoryInput");
            if (!ModelState.IsValid)
                return View(model);
            string flag = "综保区填写核注清单";
            if (!String.IsNullOrEmpty(model.LicensePlateNo))
                model.LicensePlateNo = model.LicensePlateNo.Trim();
            if (!String.IsNullOrEmpty(model.NuclearNote))
                model.NuclearNote = model.NuclearNote.Trim();
            if (model.Id.Equals(0))
            {
            }
            else
            {

                model.LicensePlateTime = DateTime.Now;
                model.LicensePlater = WorkContext.CurrentUser.Id;
                _exportTransportationService.updateExportTransportation(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}
