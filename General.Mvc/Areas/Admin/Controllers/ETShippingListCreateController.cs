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
    [Route("admin/etShippingListCreate")]
    public class ETShippingListCreateController : AdminPermissionController
    {
        private IExportTransportationService _exportTransportationService;
        private ISysCustomizedListService _sysCustomizedListService;
        public ETShippingListCreateController(IExportTransportationService exportTransportationService, ISysCustomizedListService sysCustomizedListService)
        {

            this._exportTransportationService = exportTransportationService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "etShippingListCreate")]
        [Function("物流员（新）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ExportTransportationController", Sort = 1)]

        public IActionResult ETShippingListCreateIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("付费方式");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            string flag = "创建发运清单";
            var pageList = _exportTransportationService.searchList(arg, page, size, flag);
            ViewBag.Arg = arg;
            var dataSource = pageList.toDataSourceResult<Entities.ExportTransportation, SysCustomizedListSearchArg>("etShippingListCreate", arg);
            return View(dataSource);
        }
        [HttpPost]
        [Route("etShippingListCreateList", Name = "etShippingListCreateList")]
        public ActionResult ETShippingListCreateList(string kevin)
        {
            string test = kevin;
            List<Entities.ExportTransportation> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ExportTransportation>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            foreach (Entities.ExportTransportation u in jsonlist)
            {
                var model = _exportTransportationService.getById(u.Id);
                model.Project = u.Project;
                model.Applier = u.Applier;
                model.OfGoods = u.OfGoods;
                model.TotalGw = u.TotalGw;
                model.TotalNo = u.TotalNo;
                model.TradeTerms = u.TradeTerms;
                model.Carrier = u.Carrier;
                model.PickDriver = u.PickDriver;
                model.DriverCard = u.DriverCard;
                //model.Project = u.Project;
                _exportTransportationService.updateExportTransportation(model);
                //u就是jsonlist里面的一个实体类
            }
            AjaxData.Status = true;
            AjaxData.Message = "OK";
            return Json(AjaxData);
        }
        //[HttpPost]
        //[Route("")]
        //public ActionResult ETShippingListCreateIndex(List<int> sysResource, string returnUrl = null)
        //{
        //    ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etShippingListCreate");
        //    string flag = "创建发运清单";
        //    _exportTransportationService.saveExportTransportation(sysResource, flag);
        //    AjaxData.Status = true;
        //    AjaxData.Message = "确认创建成功";
        //    return Redirect(ViewBag.ReturnUrl);
        //    //return View();
        //}
        [HttpGet]
        [Route("edit", Name = "editetShippingListCreate")]
        [Function("创建发运清单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ETShippingListCreateController.ETShippingListCreateIndex")]
        public IActionResult EditETShippingListCreate(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etShippingListCreate");
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
        public ActionResult EditETShippingListCreate(Entities.ExportTransportation model, string returnUrl = null)
        {
            ModelState.Remove("Id");

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("etShippingListCreate");
            if (!ModelState.IsValid)
                return View(model);
   
            if (!String.IsNullOrEmpty(model.ItemNo))
                model.ItemNo = model.ItemNo.Trim();
            if (!String.IsNullOrEmpty(model.Project))
                model.Project = model.Project.Trim();
            if (!String.IsNullOrEmpty(model.OfGoods))
                model.OfGoods = model.OfGoods.Trim();
            if (model.Id.Equals(0))
            {
               
            }
            else
            {
                model.CreationTime = DateTime.Now;
                model.Creator = WorkContext.CurrentUser.Id;
                _exportTransportationService.updateExportTransportation(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}
