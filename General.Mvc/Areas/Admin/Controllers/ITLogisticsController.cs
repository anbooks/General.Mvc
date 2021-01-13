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

    [Route("admin/itLogistics")]
    public class ITLogisticsController : AdminPermissionController
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        public ITLogisticsController(ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itLogistics")]
        [Function("物流员（新）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]
        [HttpGet]
        public IActionResult ITLogisticsIndex(List<int> sysResource,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("运输方式");
            ViewData["ShippingMode"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            //var customizedList2 = _sysCustomizedListService.getByAccount("运输状态");
             //ViewData["Status"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _importTrans_main_recordService.searchListLogistics(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itLogistics", arg);
            return View(dataSource);//sysImport
        }
        [Route("schedule", Name = "itLogisticsSchedule")]
        [Function("物流员明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITLogisticsController.ITLogisticsIndex")]
        [HttpGet]
        public IActionResult ITLogisticsScheduleIndex( int id ,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itLogisticsSchedule", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("itLogisticsScheduleList", Name = "itLogisticsScheduleList")]
        [Function("物流员明细表数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITLogisticsController.ITLogisticsIndex")]
        public ActionResult ITConfirmedCustomsScheduleList(string kevin)
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
        //[Route("excelLogistics", Name = "excelLogistics")]
        //public FileResult Excel()
        //{
        //    var list = _importTrans_main_recordService.getAll();
        //    NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
        //    //添加一个sheet
        //    NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
        //    //给sheet1添加第一行的头部标题
        //    NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
        //    row1.CreateCell(0).SetCellValue("ID");
        //    row1.CreateCell(1).SetCellValue("编号");
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
        //        rowtemp.CreateCell(0).SetCellValue(list[i].Id.ToString());
        //        rowtemp.CreateCell(1).SetCellValue(list[i].Itemno);
        //    }
        //    // 写入到客户端 
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    book.Write(ms);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    string sFileName = $"{DateTime.Now}.xls";
        //    return File(ms, "application/vnd.ms-excel", sFileName);
        //}
        //[HttpPost]
        //[Route("importLogistics", Name = "importLogistics")]
        //public ActionResult Import(IFormFile excelfile, Entities.ImportTrans_main_record model, string returnUrl = null)
        //{
        //    ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itLogistics");
        //    string sWebRootFolder = _hostingEnvironment.WebRootPath;
        //    var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
        //    string sFileName = $"{Guid.NewGuid()}.xlsx";
        //    FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
        //    using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
        //    {
        //        excelfile.CopyTo(fs);
        //        fs.Flush();
        //    }
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
        //        int rowCount = worksheet.Dimension.Rows;
        //        int ColCount = worksheet.Dimension.Columns;
        //        for (int row = 2; row <= rowCount; row++)
        //        {

        //            model.Itemno = worksheet.Cells[row, 1].Value.ToString();
        //            model.Shipper = worksheet.Cells[row, 2].Value.ToString();
        //            model.PoNo = worksheet.Cells[row, 3].Value.ToString();
        //            if (model.PoNo!=null)
        //            {
        //                model.Buyer = model.PoNo.Substring(1, 2);
        //            }             
        //            model.Incoterms = worksheet.Cells[row, 4].Value.ToString();
        //            model.CargoType = worksheet.Cells[row, 5].Value.ToString();
        //            model.Invamou = worksheet.Cells[row, 6].Value.ToString();
        //            model.Invcurr = worksheet.Cells[row, 7].Value.ToString();
        //            model.CreationTime = DateTime.Now;
        //            model.Creator = WorkContext.CurrentUser.Id;
        //            try
        //            {
        //            }
        //            catch (Exception e)
        //            {
        //            }
        //            _importTrans_main_recordService.insertImportTransmain(model);
        //        }
        //        return Redirect(ViewBag.ReturnUrl);
        //    }
        //}
        [HttpPost]
        [Route("itLogisticsList", Name = "itLogisticsList")]
        [Function("物流员数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITLogisticsController.ITLogisticsIndex")]
        public ActionResult ITLogisticsList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            try { 
            foreach (Entities.ImportTrans_main_record u in jsonlist)
            {
                var model = _importTrans_main_recordService.getById(u.Id);
                
              
                if (u.ShippingMode != "") { model.ShippingMode = u.ShippingMode; }
                model.DeliveryRequiredDate = u.DeliveryRequiredDate;
                if (WorkContext.CurrentUser.Co == "北京捷诚" || WorkContext.CurrentUser.Co == "辽宁北方")
                {
                    if (model.Dest == "PEK")
                    {
                        model.Forwarder = "北京捷诚";
                    }
                    else if (model.Dest == "SHE")
                    {
                        model.Forwarder = "辽宁北方";
                    }
                    else if (model.Dest == "DLC")
                    {
                        model.Forwarder = "大连环球";
                    }
                    else
                    {
                        model.Forwarder = "辽宁北方";
                    }
                }
                else if (WorkContext.CurrentUser.Co == "北京和合" || WorkContext.CurrentUser.Co == "大连环球")
                {
                    if (model.Dest == "PEK")
                    {
                        model.Forwarder = "北京捷诚";
                    }
                    else if (model.Dest == "SHE")
                    {
                        model.Forwarder = "辽宁北方";
                    }
                    else if (model.Dest == "DLG")
                    {
                        model.Forwarder = "大连环球";
                    }
                    else
                    {
                        model.Forwarder = "辽宁北方";
                    }
                }
                else
                {
                    model.Forwarder = u.Forwarder;
                }
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
        [Route("edit", Name = "editITLogistics")]
        [Function("物流员编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITLogisticsController.ITLogisticsIndex")]
        public IActionResult EditITLogistics(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itLogistics");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
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
        public ActionResult EditITLogistics(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itLogistics");
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
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit2", Name = "editLogisticsSchedule")]
        [Function("物流员编辑明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITLogisticsController.ITLogisticsScheduleIndex")]
        public IActionResult EditLogisticsSchedule(int? id, string returnUrl = null)
        {//页面跳转未完成
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itLogistics");
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
        public ActionResult EditLogisticsSchedule(Entities.Schedule model, string returnUrl = null)
        {//页面跳转未完成
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itLogistics");
            if (!ModelState.IsValid)
                return View(model);
          //  if (!String.IsNullOrEmpty(model.InvoiceNo))
          //      model.InvoiceNo = model.InvoiceNo.Trim();
           // if (!String.IsNullOrEmpty(model.MaterielNo))
           //     model.MaterielNo = model.MaterielNo.Trim();
           // if (!String.IsNullOrEmpty(model.PurchasingDocuments))
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