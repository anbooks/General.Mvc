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

namespace General.Mvc.Areas.Admin.Controllers
{

    [Route("admin/itTransport")]
    public class ITTransportController : AdminPermissionController
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        public ITTransportController(ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itTransport")]
        [Function("运代（新）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]
        [HttpGet]
        public IActionResult ITTransportIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurr"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var customizedList2 = _sysCustomizedListService.getByAccount("运输状态");
            ViewData["Status"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");
            
            var pageList = _importTrans_main_recordService.searchListTransport(arg, page, size, WorkContext.CurrentUser.Co);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itTransport", arg);
            return View(dataSource);//sysImport
        }
        [Route("schedule", Name = "itTransportSchedule")]
        [Function("明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        [HttpGet]
        public IActionResult ITTransportScheduleIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.QX = WorkContext.CurrentUser.Co;
            int? ida = id;
            if (ida == 0)
            {
                ViewBag.Import = HttpContext.Session.GetInt32("import");
            }
            else
            {
                HttpContext.Session.SetInt32("import", id);
                ViewBag.Import = HttpContext.Session.GetInt32("import");
            }
            //Session["username"] = id.ToString(); 
            int importid = ViewBag.Import;
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _scheduleService.searchList(arg, page, size, importid);
            var item = _importTrans_main_recordService.getById(importid);
            if (pageList.Count > 0)
            {
                item.F_ShippingModeGiven = true;

                _importTrans_main_recordService.updateImportTransmain(item);
            }
            ViewBag.orderno = item.PoNo;
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itTransportSchedule", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("itTransportScheduleList", Name = "itTransportScheduleList")]
        [Function("明细表数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        public ActionResult ITTransportScheduleList(string kevin)
        {
            string test = kevin;
            List<Entities.Schedule> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Schedule>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                foreach (Entities.Schedule u in jsonlist)
                {
                    var model = _scheduleService.getById(u.Id);

                    model.PurchaseQuantity = u.PurchaseQuantity;
                    model.PurchaseUnit = u.PurchaseUnit;
                    model.UnitPrice = u.UnitPrice;
                    model.TotalPrice = u.TotalPrice;
                    model.ShipmentDate = u.ShipmentDate;
                    model.Consignor = u.Consignor;
                    model.Manufacturers = u.Manufacturers;
                    model.OriginCountry = u.OriginCountry;
                    model.Books = u.Books;
                    model.BooksItem = u.BooksItem;
                    model.Waybill = u.Waybill;
                    model.BatchNo = u.BatchNo;
                    model.RecordUnit = u.RecordUnit;
                    model.RecordUnitReducedPrice = u.RecordUnitReducedPrice;
                    model.LegalUnits = u.LegalUnits;
                    model.LegalUniteReducedPrice = u.LegalUniteReducedPrice;
                    model.Qualification = u.Qualification;
                    _scheduleService.updateSchedule(model);
                    //u就是jsonlist里面的一个实体类
                }
                AjaxData.Status = true;
                AjaxData.Message = "OK";
            }
            catch
            {
                AjaxData.Status = false;
                AjaxData.Message = "OK";
            }
            return Json(AjaxData);
        }
        [Route("downLoadmbl", Name = "downLoadmbl")]
        [Function("下载主单附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        public FileStreamResult DownLoadMbl(int? id)
        {
            var model = _importTrans_main_recordService.getById(id.Value);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\mbl\\";
            string sFileName = model.MblAttachment;
            FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
            FileStream fs = new FileStream(file.ToString(), FileMode.Create);
            return File(fs, "application/octet-stream", sFileName);
        }
        [Route("downLoadhbl", Name = "downLoadhbl")]
        [Function("下载分单附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        public FileStreamResult DownLoadHbl(int? id)
        {
            var model = _importTrans_main_recordService.getById(id.Value);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\hbl\\";
            string sFileName = model.HblAttachment;
            FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
            FileStream fs = new FileStream(file.ToString(), FileMode.Create);
            return File(fs, "application/octet-stream", sFileName);
        }
        [HttpPost]
        [Route("itTransportList", Name = "itTransportList")]
        [Function("运代数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        public ActionResult ITTransportList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            try
            {
                foreach (Entities.ImportTrans_main_record u in jsonlist)
                {
                    var model = _importTrans_main_recordService.getById(u.Id);
                    model.Incoterms = u.Incoterms;
                    model.CargoType = u.CargoType;
                    model.Invamou = u.Invamou;
                    if (u.Invcurr != "") { model.Invcurr = u.Invcurr; }
                   
                    model.RealReceivingDate = u.RealReceivingDate;
                    model.Pcs = u.Pcs;
                    model.Gw = u.Gw;
                    if (u.Status != "") { model.Status = u.Status; }
                    model.FlighVessel = u.FlighVessel;
                    model.Origin = u.Origin;
                    model.Dest = u.Dest;
                    model.Mbl = u.Mbl;
                    model.Hbl = u.Hbl;
                    model.Measurement = u.Measurement;
                    model.MeasurementUnit = u.MeasurementUnit;
                    model.Ata = u.Ata;
                    model.Atd = u.Atd;
                    _importTrans_main_recordService.updateImportTransmain(model);
                }
                AjaxData.Status = true;
                AjaxData.Message = "OK";
            }
            catch
            {
                AjaxData.Status = false;
                AjaxData.Message = "OK";
            }
            return Json(AjaxData);
        }
        [HttpGet]
        [Route("edit", Name = "editITTransport")]
        [Function("运代编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        public IActionResult EditITTransport(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itTransport");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            if (id != null)
            {
                var model = _importTrans_main_recordService.getById(id.Value);
                ViewBag.mbl = model.MblAttachment;
                ViewBag.hbl = model.HblAttachment;
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            else
            {
                ViewBag.FJ = 1;
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditITTransport(Entities.ImportTrans_main_record model, IFormFile mblfile, IFormFile hblfile, string returnUrl = null)
        {
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itTransport");
            if (!ModelState.IsValid)
                return View(model);
            if (!String.IsNullOrEmpty(model.Invcurr))
                model.Invcurr = model.Invcurr.Trim();
            if (!String.IsNullOrEmpty(model.Shipper))
                model.Shipper = model.Shipper.Trim();
            if (!String.IsNullOrEmpty(model.Itemno))
                model.Itemno = model.Itemno.Trim();
            if (!String.IsNullOrEmpty(model.PoNo))
                model.PoNo = model.PoNo.Trim();
            if (!String.IsNullOrEmpty(model.PoNo))
                model.Buyer = model.PoNo.Substring(1, 2);
            if (model.Id.Equals(0))
            {
                model.CreationTime = DateTime.Now;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                model.Creator = WorkContext.CurrentUser.Id;
                _importTrans_main_recordService.insertImportTransmain(model);
            }
            else
            {
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                var fileProfilem = sWebRootFolder + "\\Files\\mbl\\";
                var fileProfileh = sWebRootFolder + "\\Files\\hbl\\";
                string sFileNamem = model.Id + "-" + $"{DateTime.Now.ToString("yyMMdd")}" + mblfile.FileName;
                string sFileNameh = model.Id + "-" + $"{DateTime.Now.ToString("yyMMdd")}" + hblfile.FileName;
                FileInfo filem = new FileInfo(Path.Combine(fileProfilem, sFileNamem));
                FileInfo fileh = new FileInfo(Path.Combine(fileProfileh, sFileNameh));
                using (FileStream fsm = new FileStream(filem.ToString(), FileMode.Create))
                {
                    mblfile.CopyTo(fsm);
                    fsm.Flush();
                }
                using (FileStream fsh = new FileStream(fileh.ToString(), FileMode.Create))
                {
                    mblfile.CopyTo(fsh);
                    fsh.Flush();
                }
                model.MblAttachment = sFileNamem;
                model.HblAttachment = sFileNameh;
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit2", Name = "editTransportSchedule")]
        [Function("编辑明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportScheduleIndex")]
        public IActionResult EditTransportSchedule(int? id, string returnUrl = null)
        {//页面跳转未完成
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itTransport");
            if (id != null)
            {
                var model = _scheduleService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit2")]
        public ActionResult EditTransportSchedule(Entities.Schedule model, string returnUrl = null)
        {//页面跳转未完成
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itTransport");
            if (!ModelState.IsValid)
                return View(model);
            //if (!String.IsNullOrEmpty(model.InvoiceNo))
            //     model.InvoiceNo = model.InvoiceNo.Trim();
            // if (!String.IsNullOrEmpty(model.MaterielNo))
            //      model.MaterielNo = model.MaterielNo.Trim();
            // if (!String.IsNullOrEmpty(model.PurchasingDocuments))
            //     model.PurchasingDocuments = model.PurchasingDocuments.Trim();
            if (model.Id.Equals(0))
            {
                model.CreationTime = DateTime.Now;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                model.Creator = WorkContext.CurrentUser.Id;
                _scheduleService.insertSchedule(model);
            }
            else
            {
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _scheduleService.updateSchedule(model);
            }
            int mid = model.MainId;
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}