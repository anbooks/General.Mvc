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

    [Route("admin/itPorkCustoms")]
    public class ITPorkCustomsController : AdminPermissionController
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        public ITPorkCustomsController(ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itPorkCustoms")]
        [Function("口岸报关行（新）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]
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
            
            var pageList = _importTrans_main_recordService.searchListPortCustomerBroker(arg, page, size, WorkContext.CurrentUser.Co);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itPorkCustoms", arg);
            return View(dataSource);//sysImport
        }
        [Route("schedule", Name = "itPorkCustomsSchedule")]
        [Function("明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
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
        [Function("明细表数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
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
        [Route("excelPorkCustoms", Name = "excelPorkCustoms")]
        public FileResult Excel()
        {
            var list = _importTrans_main_recordService.getAll();
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("ID");
            row1.CreateCell(1).SetCellValue("编号");
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Id.ToString());
                rowtemp.CreateCell(1).SetCellValue(list[i].Itemno);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            string sFileName = $"{DateTime.Now}.xls";
            return File(ms, "application/vnd.ms-excel", sFileName);
        }
        [HttpPost]
        [Route("importPorkCustoms", Name = "importPorkCustoms")]
        public ActionResult Import(IFormFile excelfile, Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
            string sFileName = $"{Guid.NewGuid()}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
            using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
            {
                excelfile.CopyTo(fs);
                fs.Flush();
            }
            using (ExcelPackage package = new ExcelPackage(file))
            {
                StringBuilder sb = new StringBuilder();
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                for (int row = 2; row <= rowCount; row++)
                {
                    //string s = "abcdeabcdeabcde";
                    //string[] sArray = s.Split('c');
                    //foreach (string i in sArray)
                    //Console.WriteLine(i.ToString());
                    model.Itemno = worksheet.Cells[row, 1].Value.ToString();
                    model.Shipper = worksheet.Cells[row, 2].Value.ToString();
                    model.PoNo = worksheet.Cells[row, 3].Value.ToString();
                    if (model.PoNo!=null)
                    {
                        model.Buyer = model.PoNo.Substring(1, 2);
                    }             
                    model.Incoterms = worksheet.Cells[row, 4].Value.ToString();
                    model.CargoType = worksheet.Cells[row, 5].Value.ToString();
                    model.Invamou = worksheet.Cells[row, 6].Value.ToString();
                    model.Invcurr = worksheet.Cells[row, 7].Value.ToString();
                    model.CreationTime = DateTime.Now;
                    model.Creator = WorkContext.CurrentUser.Id;
                    try
                    {
                    }
                    catch (Exception e)
                    {
                    }
                    _importTrans_main_recordService.insertImportTransmain(model);
                }
                return Redirect(ViewBag.ReturnUrl);
            }
        }
        [HttpPost]
        [Route("itPorkCustomsList", Name = "itPorkCustomsList")]
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
        [Function("编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsIndex")]
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
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                var fileProfile = sWebRootFolder + "\\Files\\notefile\\";
                string sFileName = model.Id + "-" + $"{DateTime.Now.ToString("yyMMdd")}" + notefile.FileName;
                FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
                using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                {
                    notefile.CopyTo(fs);
                    fs.Flush();
                }
                model.Note = sFileName;
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit2", Name = "editPorkCustomsSchedule")]
        [Function("编辑明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITPorkCustomsController.ITPorkCustomsScheduleIndex")]
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
          //  if (!String.IsNullOrEmpty(model.InvoiceNo))
          //      model.InvoiceNo = model.InvoiceNo.Trim();
          //  if (!String.IsNullOrEmpty(model.MaterielNo))
          //      model.MaterielNo = model.MaterielNo.Trim();
          //  if (!String.IsNullOrEmpty(model.PurchasingDocuments))
          //      model.PurchasingDocuments = model.PurchasingDocuments.Trim();
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