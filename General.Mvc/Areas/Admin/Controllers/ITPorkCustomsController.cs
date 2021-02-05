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
using General.Services.Attachment;
using Microsoft.AspNetCore.StaticFiles;
using NPOI.SS.Util;

namespace General.Mvc.Areas.Admin.Controllers
{

    [Route("admin/itPorkCustoms")]
    public class ITPorkCustomsController : AdminPermissionController
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        private IAttachmentService _attachmentService;
        public ITPorkCustomsController(IAttachmentService attachmentService, ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
            this._attachmentService = attachmentService;
        }
        [Route("", Name = "itPorkCustoms")]
        [Function("口岸报关行（新）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 4)]
        [HttpGet]
        public IActionResult ITPorkCustomsIndex(List<int> sysResource,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("自行送货或外部提货");
            ViewData["ChooseDelivery"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var customizedList2 = _sysCustomizedListService.getByAccount("是否调用明细表生成二检明细");
            ViewData["DeliveryReceipt"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");
            var customizedList3 = _sysCustomizedListService.getByAccount("是否破损");
            ViewData["BrokenRecord"] = new SelectList(customizedList3, "CustomizedValue", "CustomizedValue");
            //var customizedList2 = _sysCustomizedListService.getByAccount("运输状态");
            //ViewData["Status"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");
            
            var pageList = _importTrans_main_recordService.searchListPortCustomerBroker(arg, page, size, WorkContext.CurrentUser.Port);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itPorkCustoms", arg);
            return View(dataSource);//sysImport
        }
        [Route("PorkAttachment", Name = "PorkAttachment")]
        [Function("破损记录", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        [HttpGet]
        public IActionResult ITPorkCustomsAttachmentIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _attachmentService.searchPorkAttachment(arg, page, size, id);
            ViewBag.Arg = arg;//传参数ITTransportAttachmentIndex
            var dataSource = pageList.toDataSourceResult<Entities.Attachment, SysCustomizedListSearchArg>("itPorkCustoms", arg);
            return View(dataSource);//sysImport
        }
        [Route("downLoadPork", Name = "downLoadPork")]
        [Function("下载破损记录", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        public IActionResult Download(int? id)
        {
            string load = "";

            var model = _attachmentService.getById(id.Value);
                load = "\\Files\\notefile\\";
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + load + model.AttachmentLoad;
            string sFileName = model.AttachmentLoad;
            var addrUrl = sFileName;
            var stream = System.IO.File.OpenRead(fileProfile); //Path.GetExtension
            string fileExt = Path.GetExtension(sFileName);
            //获取文件的ContentType
            var provider = new FileExtensionContentTypeProvider();
            var memi = provider.Mappings[fileExt];
            // var downloadName = Path.GetFileName(addrUrl);
            return File(stream, memi, model.Name);
        }
        [Route("PorkAttachment", Name = "deleLoadPork")]
        [Function("删除破损记录", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        public ActionResult Deleload(int? id)
        {
            var model = _attachmentService.getById(id.Value);
            model.IsDelet = true;
            _attachmentService.updateAttachment(model);
            return View();
        }
        [Route("schedule", Name = "itPorkCustomsSchedule")]
        [Function("口岸报关行明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        [HttpGet]
        public IActionResult ITPorkCustomsScheduleIndex( int id ,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itPorkCustomsSchedule", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("itPorkCustomsScheduleList", Name = "itPorkCustomsScheduleList")]
        [Function("口岸报关行明细表数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        public ActionResult ITPorkCustomsScheduleList(string kevin)
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
        [Route("excelimportCon", Name = "excelimportCon")]
        [Function("生成货物交接单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        public IActionResult Export2()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var path = Path.Combine(sWebRootFolder, "Files", "abc.xlsx");
            string sFileName = "明细表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            using (var fs = System.IO.File.Open(path, FileMode.Open, FileAccess.Read))
            using (ExcelPackage package = new ExcelPackage(fs))
            {
                var worksheet = package.Workbook.Worksheets["sheet1"];

                var sc = worksheet.Dimension.Start.Column;
                var ec = worksheet.Dimension.End.Column;
                var sr = worksheet.Dimension.Start.Row;
                var er = worksheet.Dimension.End.Row;
                var value = worksheet.Cells[sc, sr + 1].Value;
                worksheet.Cells[2, 3].Value = "采购员";
                package.Save();
            }
           
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        [Route("itPorkCustomsList", Name = "itPorkCustomsList")]
        [Function("口岸报关行数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        public ActionResult ITPorkCustomsList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            foreach (Entities.ImportTrans_main_record u in jsonlist)
            {
                var model = _importTrans_main_recordService.getById(u.Id);
                if (u.ChooseDelivery != "") { model.ChooseDelivery = u.ChooseDelivery; }
                model.BlDate = u.BlDate;
                model.DeclarationDate = u.DeclarationDate;
                model.ReleaseDate = u.ReleaseDate;
                model.CustomsDeclarationNo = u.CustomsDeclarationNo;
                model.InspectionLotNo = u.InspectionLotNo;
                model.ActualDeliveryDate = u.ActualDeliveryDate;
                if (u.DeliveryReceipt != "") { model.DeliveryReceipt = u.DeliveryReceipt; }
                if (u.BrokenRecord != "") { model.BrokenRecord = u.BrokenRecord; }
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            AjaxData.Status = true;
            AjaxData.Message = "OK";
            return Json(AjaxData);
        }
        [HttpGet]
        [Route("edit", Name = "editITPorkCustoms")]
        [Function("口岸报关行编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
        public IActionResult EditITPorkCustoms(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            if (id != null)
            {
                var model = _importTrans_main_recordService.getById(id.Value);
                ViewBag.note = model.Note;
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
        public ActionResult EditITPorkCustoms(Entities.ImportTrans_main_record model, IFormFile notefile, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
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
                model.Buyer= model.PoNo.Substring(1, 2);
            if (model.Id.Equals(0)) {
                model.CreationTime = DateTime.Now;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                model.Creator = WorkContext.CurrentUser.Id;
            _importTrans_main_recordService.insertImportTransmain(model);
            }
            else
            {
                var modela = _importTrans_main_recordService.getById(model.Id);
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                if (notefile != null)
                {
                    var fileProfilef = sWebRootFolder + "\\Files\\notefile\\";
                    string f = Guid.NewGuid().ToString("N");
                    string sFileNamef = f+ notefile.FileName;
                    FileInfo filef = new FileInfo(Path.Combine(fileProfilef, sFileNamef));
                    using (FileStream fsf = new FileStream(filef.ToString(), FileMode.Create))
                    {
                        notefile.CopyTo(fsf);
                        fsf.Flush();
                    }
                    Attachment attachmentf = new Attachment();
                    attachmentf.AttachmentLoad = sFileNamef;
                    attachmentf.Name = notefile.FileName;
                    attachmentf.Type = "破损记录";
                    attachmentf.ImportId = model.Id;
                    attachmentf.Creator = WorkContext.CurrentUser.Account;
                    attachmentf.CreationTime = DateTime.Now;
                    modela.Note =sFileNamef;
                    _attachmentService.insertAttachment(attachmentf);
                }
                
                _importTrans_main_recordService.updateImportTransmain(modela);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit2", Name = "editPorkCustomsSchedule")]
        [Function("口岸报关行编辑明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsScheduleIndex")]
        public IActionResult EditPorkCustomsSchedule(int? id, string returnUrl = null)
        {//页面跳转未完成
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
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
        public ActionResult EditPorkCustomsSchedule(Entities.Schedule model, string returnUrl = null)
        {//页面跳转未完成
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
            if (!ModelState.IsValid)
                return View(model);

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