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
using General.Services.Order;
using General.Services.Material;
using General.Services.Project;
using General.Services.Supplier;
using General.Services.Inspection;
using General.Services.OrderMain;

using General.Services.InspecationMain;
using General.Services.Attachment;
using Microsoft.AspNetCore.StaticFiles;
//using BLL;
namespace General.Mvc.Areas.Admin.Controllers
{

    [Route("admin/itBuyer")]
    public class ITBuyerController : AdminPermissionController
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        private ISysUserService _sysUserService;
        private IOrderService _sysOrderService;
        private IOrderMainService _sysOrderMainService;
        private IMaterialService _sysMaterialService;
        private IProjectService _sysProjectService;
        private ISupplierService _sysSupplierService;
        private IInspectionService _sysInspectionService;
        private IInspecationMainService _sysInspectionMainService;
        private IAttachmentService _attachmentService;
        public ITBuyerController(IAttachmentService attachmentService, ISysUserService sysUserService,IInspecationMainService sysInspectionMainService, IOrderMainService sysOrderMainService, IInspectionService sysInspectionService, ISupplierService sysSupplierService, IProjectService sysProjectService, IMaterialService sysMaterialService, ISysUserRoleService sysUserRoleService, IOrderService sysOrderService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._attachmentService = attachmentService;
            this._sysInspectionMainService = sysInspectionMainService;
            this._sysInspectionService = sysInspectionService;
            this._sysProjectService = sysProjectService;
            this._sysOrderMainService = sysOrderMainService;
            this._sysMaterialService = sysMaterialService;
            this._sysOrderService = sysOrderService;
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._sysUserService = sysUserService;
            this._scheduleService = scheduleService;
            this._sysSupplierService = sysSupplierService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itBuyer")]
        [Function("待办/在途（采购员）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 2)]
        [HttpGet]
        public IActionResult ITBuyerIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            // var USER = _sysUserRoleService.getById(WorkContext.CurrentUser.Id);
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _importTrans_main_recordService.searchListBuyer(arg, page, size, WorkContext.CurrentUser.Name);
            string date = DateTime.Now.ToString("yyMMdd");
            var serial = _sysInspectionMainService.getByDate(date);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itBuyer", arg);
            return View(dataSource);//sysImport
        }
        [Route("schedule", Name = "itBuyerSchedule")]
        [Function("采购员明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        [HttpGet]
        public IActionResult ITBuyerScheduleIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            if (pageList.Count > 0&& item.F_ShippingModeGiven!=true)
            {
                item.F_ShippingModeGiven = true;

                _importTrans_main_recordService.updateImportTransmain(item);
            }
            ViewBag.orderno = item.PoNo;
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itBuyerSchedule", arg);
            return View(dataSource);//sysImport
        }
        [HttpGet]
        [Route("Order", Name = "itOrderBuyerIndex")]
        [Function("订单数据导入明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult ITOrderBuyerIndex(string orderno, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string  ida = orderno;
            if (ida == null)
            {
                ida = HttpContext.Session.GetString("orderno");
                
            }
            else
            {
                HttpContext.Session.SetString("orderno", orderno);
                ida = orderno;
            }
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var pageList = _sysOrderService.searchOrderD(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Order, SysCustomizedListSearchArg>("itOrderBuyerIndex", arg);
            return View(dataSource);//sysImport
        }
        [Route("excelimportasj", Name = "excelimportasj")]
        [Function("送检单生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export()
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var list = _scheduleService.getAll(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "送检单" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\sjdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("送检单");
                //添加头
                worksheet.Cells[1, 1].Value = "采购员";
                worksheet.Cells[1, 2].Value = "订单号";
                worksheet.Cells[1, 3].Value = "索引号";
                worksheet.Cells[1, 4].Value = "物料代码";
                worksheet.Cells[1, 5].Value = "品名";
                worksheet.Cells[1, 6].Value = "牌号";
                worksheet.Cells[1, 7].Value = "规范";
                worksheet.Cells[1, 8].Value = "厚度";
                worksheet.Cells[1, 9].Value = "长";
                worksheet.Cells[1, 10].Value = "宽";
                worksheet.Cells[1, 11].Value = "采购数量";
                worksheet.Cells[1, 12].Value = "采购单位";
                worksheet.Cells[1, 13].Value = "制造商";
                worksheet.Cells[1, 14].Value = "炉批号";
                worksheet.Cells[1, 15].Value = "运单号";
                worksheet.Cells[1, 16].Value = "备注1";
                worksheet.Cells[1, 17].Value = "备注2";
                worksheet.Cells[1, 18].Value = "备注3";
                //添加值
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    if (list[i].Buyer.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 1].Value = list[i].Buyer.ToString();
                    }
                    if (list[i].OrderNo.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 2].Value = list[i].OrderNo.ToString();
                    }
                    if (list[i].ReferenceNo.ToString() != null)
                    { worksheet.Cells[i + 2, 3].Value = list[i].ReferenceNo.ToString(); }
                    if (list[i].MaterialCode.ToString() != null)
                    { worksheet.Cells[i + 2, 4].Value = list[i].MaterialCode.ToString(); }
                    if (list[i].Description.ToString() != null)
                    { worksheet.Cells[i + 2, 5].Value = list[i].Description.ToString(); }
                    if (list[i].Type.ToString() != null)
                    { worksheet.Cells[i + 2, 6].Value = list[i].Type.ToString(); }
                    if (list[i].Specification.ToString() != null)
                    { worksheet.Cells[i + 2, 7].Value = list[i].Specification.ToString(); }
                    if (list[i].Thickness.ToString() != null)
                    { worksheet.Cells[i + 2, 8].Value = list[i].Thickness; }
                    if (list[i].Length.ToString() != null)
                    { worksheet.Cells[i + 2, 9].Value = Convert.ToInt32(list[i].Length.ToString()); }
                    if (list[i].Width.ToString() != null)
                    { worksheet.Cells[i + 2, 10].Value = Convert.ToInt32(list[i].Width.ToString()); }
                    if (list[i].PurchaseQuantity.ToString() != null)
                    { worksheet.Cells[i + 2, 11].Value = list[i].PurchaseQuantity.ToString(); }
                    if (list[i].PurchaseUnit.ToString() != null)
                    { worksheet.Cells[i + 2, 12].Value = list[i].PurchaseUnit.ToString(); }
                    if (list[i].Manufacturers.ToString() != null)
                    { worksheet.Cells[i + 2, 13].Value = list[i].Manufacturers.ToString(); }
                    if (list[i].BatchNo.ToString() != null)
                    { worksheet.Cells[i + 2, 14].Value = list[i].BatchNo.ToString(); }
                    if (list[i].Waybill.ToString() != null)
                    { worksheet.Cells[i + 2, 15].Value = list[i].Waybill.ToString(); }

                }
                package.Save();
            }
            return File("\\Files\\sjdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [Route("excelimportBuyer", Name = "excelimportBuyer")]
        [Function("明细表导出", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export2()
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var list = _scheduleService.getAll(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "明细表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("明细表");
                //添加头 
                worksheet.Cells[1, 1].Value = "编号";
                worksheet.Cells[1, 2].Value = "供应商";
                worksheet.Cells[1, 3].Value = "订单号";
                worksheet.Cells[1, 4].Value = "打码号";
                worksheet.Cells[1, 5].Value = "订单行号";
                worksheet.Cells[1, 6].Value = "索引号";
                worksheet.Cells[1, 7].Value = "物料代码";
                worksheet.Cells[1, 8].Value = "品名";
                worksheet.Cells[1, 9].Value = "牌号";
                worksheet.Cells[1, 10].Value = "规范";
                worksheet.Cells[1, 11].Value = "规格1";
                worksheet.Cells[1, 12].Value = "规格2";
                worksheet.Cells[1, 13].Value = "规格3";
                worksheet.Cells[1, 14].Value = "炉批号";
                worksheet.Cells[1, 15].Value = "包装规格";
                worksheet.Cells[1, 16].Value = "采购数量";
                worksheet.Cells[1, 17].Value = "采购单位";
                worksheet.Cells[1, 18].Value = "单价";
                worksheet.Cells[1, 19].Value = "总价";
                worksheet.Cells[1, 20].Value = "发票号";
                worksheet.Cells[1, 21].Value = "计划单位";
                worksheet.Cells[1, 22].Value = "折算关系";
                worksheet.Cells[1, 23].Value = "实际提/送货日期";
                worksheet.Cells[1, 24].Value = "制造商";
                worksheet.Cells[1, 25].Value = "原产国";
                worksheet.Cells[1, 26].Value = "运单号";
                worksheet.Cells[1, 27].Value = "账册号";
                worksheet.Cells[1, 28].Value = "账册项号";
                worksheet.Cells[1, 29].Value = "申报单位";
                worksheet.Cells[1, 30].Value = "法定单位1";
                worksheet.Cells[1, 31].Value = "法定单位2";
                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 27; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }
                //添加值
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    if (list[i].Id != null)
                    {
                        worksheet.Cells[i + 2, 1].Value = list[i].Id;
                    }
                    if (list[i].Consignor != null)
                    {
                        worksheet.Cells[i + 2, 2].Value = list[i].Consignor.ToString();
                    }
                    if (list[i].OrderNo != null)
                    {
                        worksheet.Cells[i + 2, 3].Value = list[i].OrderNo.ToString();
                    }
                    if (list[i].CodeNo != null)
                    {
                        worksheet.Cells[i + 2, 4].Value = list[i].CodeNo.ToString();
                    }
                    if (list[i].OrderLine != null)
                    {
                        worksheet.Cells[i + 2, 5].Value = list[i].OrderLine.ToString();
                    }
                    if (list[i].ReferenceNo != null)
                    { worksheet.Cells[i + 2, 6].Value = list[i].ReferenceNo.ToString(); }
                    if (list[i].MaterialCode != null)
                    { worksheet.Cells[i + 2, 7].Value = list[i].MaterialCode.ToString(); }
                    if (list[i].Description != null)
                    { worksheet.Cells[i + 2, 8].Value = list[i].Description.ToString(); }
                    if (list[i].Type != null)
                    { worksheet.Cells[i + 2, 9].Value = list[i].Type.ToString(); }
                    if (list[i].Specification != null)
                    { worksheet.Cells[i + 2, 10].Value = list[i].Specification.ToString(); }
                    if (list[i].Thickness != null)
                    { worksheet.Cells[i + 2, 11].Value = Convert.ToDouble(list[i].Thickness); }
                    if (list[i].Length != null)
                    { worksheet.Cells[i + 2, 12].Value = Convert.ToDouble(list[i].Length.ToString()); }
                    if (list[i].Width != null)
                    { worksheet.Cells[i + 2, 13].Value = Convert.ToDouble(list[i].Width.ToString()); }
                    if (list[i].BatchNo != null)
                    { worksheet.Cells[i + 2, 14].Value = list[i].BatchNo.ToString(); }
                    if (list[i].Package != null)
                    { worksheet.Cells[i + 2, 15].Value = list[i].Package.ToString(); }
                    if (list[i].PurchaseQuantity != null)
                    { worksheet.Cells[i + 2, 16].Value = list[i].PurchaseQuantity.ToString(); }
                    if (list[i].PurchaseUnit != null)
                    { worksheet.Cells[i + 2, 17].Value = list[i].PurchaseUnit.ToString(); }
                    if (list[i].UnitPrice != null)
                    { worksheet.Cells[i + 2, 18].Value = list[i].UnitPrice.ToString(); }
                    if (list[i].TotalPrice != null)
                    { worksheet.Cells[i + 2, 19].Value = list[i].TotalPrice.ToString(); }
                    if (list[i].InvoiceNo != null)
                    { worksheet.Cells[i + 2, 20].Value = list[i].InvoiceNo.ToString(); }
                    if (list[i].PlanNo != null)
                    { worksheet.Cells[i + 2, 21].Value = list[i].PlanUnit.ToString(); }
                    if (list[i].Reduced != null)
                    { worksheet.Cells[i + 2, 22].Value = list[i].Reduced.ToString(); }

                    if (list[i].ShipmentDate != null)
                    { worksheet.Cells[i + 2, 23].Value = list[i].ShipmentDate.ToString(); }
               
                    if (list[i].Manufacturers != null)
                    { worksheet.Cells[i + 2, 24].Value = list[i].Manufacturers.ToString(); }
                    if (list[i].OriginCountry != null)
                    { worksheet.Cells[i + 2, 25].Value = list[i].OriginCountry.ToString(); }
                  
                    if (list[i].Waybill != null)
                    { worksheet.Cells[i + 2, 26].Value = list[i].Waybill.ToString(); }
                    if (list[i].Books != null)
                    { worksheet.Cells[i + 2, 27].Value = list[i].Books.ToString(); }
                    if (list[i].BooksItem != null)
                    { worksheet.Cells[i + 2, 28].Value = list[i].BooksItem.ToString(); }
                    if (list[i].RecordUnit != null)
                    { worksheet.Cells[i + 2, 29].Value = list[i].RecordUnit.ToString(); }
                    if (list[i].RecordUnitReducedPrice != null)
                    { worksheet.Cells[i + 2, 30].Value = list[i].RecordUnitReducedPrice.ToString(); }
                    if (list[i].LegalUnits != null)
                    { worksheet.Cells[i + 2, 31].Value = list[i].LegalUnits.ToString(); }

                }
                package.Save();
            }
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [Route("excelimportinsertBuyer", Name = "excelimportinsertBuyer")]
        [Function("明细表导出2", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export3()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "明细表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("明细表");
                //添加头 
               
                worksheet.Cells[1, 1].Value = "供应商";
                worksheet.Cells[1, 2].Value = "订单号";
                worksheet.Cells[1, 3].Value = "打码号";
                worksheet.Cells[1, 4].Value = "订单行号";
                worksheet.Cells[1, 5].Value = "索引号";
                worksheet.Cells[1, 6].Value = "物料代码";
                worksheet.Cells[1, 7].Value = "品名";
                worksheet.Cells[1, 8].Value = "牌号";
                worksheet.Cells[1, 9].Value = "规范";
                worksheet.Cells[1, 10].Value = "规格1";
                worksheet.Cells[1, 11].Value = "规格2";
                worksheet.Cells[1, 12].Value = "规格3";
                worksheet.Cells[1, 13].Value = "炉批号";
                worksheet.Cells[1, 14].Value = "包装规格";
                worksheet.Cells[1, 15].Value = "采购数量";
                worksheet.Cells[1, 16].Value = "采购单位";
                worksheet.Cells[1, 17].Value = "单价";
                worksheet.Cells[1, 18].Value = "总价";
                worksheet.Cells[1, 19].Value = "发票号";
                worksheet.Cells[1, 20].Value = "计划数量";
                worksheet.Cells[1, 21].Value = "计划单位";
                worksheet.Cells[1, 22].Value = "折算关系";
                worksheet.Cells[1, 23].Value = "实际提/送货日期";
                worksheet.Cells[1, 24].Value = "制造商";
                worksheet.Cells[1, 25].Value = "原产国";
                worksheet.Cells[1, 26].Value = "运单号";
                worksheet.Cells[1, 27].Value = "账册号";
                worksheet.Cells[1, 28].Value = "账册项号";
                worksheet.Cells[1, 29].Value = "申报单位";
                worksheet.Cells[1, 30].Value = "法定单位1";
                worksheet.Cells[1, 31].Value = "法定单位2";
                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 31; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }
                //添加值
               
                
                package.Save();
            }
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        [Route("importexcelschedule", Name = "importexcelschedule")]
        [Function("明细表数据导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult Import(Entities.Schedule modelschedule, IFormFile excelfile, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyerSchedule");
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
            string sFileName = excelfile.FileName;
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
                int buyer = 0;//采购员
                int orderno = 0;//订单号
                int orderline = 0;//订单行号
                int referenceno = 0;//索引号
                int materialcode = 0;//物料代码
                int description = 0;//品名
                int type = 0;//牌号
                int specification = 0;//规范
                int thickness = 0;//厚度
                int length = 0;//长
                int width = 0;//宽
                int purchasequantity = 0;//采购数量
                int purchaseunit = 0;//采购单位
                int unitprice = 0;//单价
                int totalprice = 0;//总价
                int shipmentdate = 0;//发运日期
                int consignor = 0;//供应商名称
                int manufacturers = 0;//制造商
                int origincountry = 0;//原产国
                int batchno = 0;//炉批号（质量编号）
                int waybill = 0;//运单号
                int books = 0;//账册号
                int booksitem = 0;//账册项号
                int recordunit = 0;//申报单位
                int recordunitreducedprice = 0;//按申报单位折算单价
                int legalunits = 0;//法定单位
                int legalunitereducedprice = 0;//按法定单位折算单价
                int qualification = 0;//合格证号 
                int planno = 0;//合格证号
                int reduced = 0;//合格证号
                int planunit = 0;
                int codeno = 0; 
                for (int columns = 1; columns <= ColCount; columns++)
                {
                    //Entities.Order model = new Entities.Order();
                    if (worksheet.Cells[1, columns].Value != null) {
                    if (worksheet.Cells[1, columns].Value.ToString() == "供应商") { buyer = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单号") { orderno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单行号") { orderline = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "索引号") { referenceno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "物料代码") { materialcode = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "品名") { description = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "牌号") { type = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规范") { specification = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格1") { thickness = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格2") { length = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格3") { width = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购数量") { purchasequantity = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购单位") { purchaseunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "单价") { unitprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "总价") { totalprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "实际提/送货日期") { shipmentdate = columns; }
                    //if (worksheet.Cells[1, columns].Value.ToString() == "供应商名称") { consignor = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "制造商") { manufacturers = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "原产国") { origincountry = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "炉批号") { batchno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "运单号") { waybill = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "账册号") { books = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "账册项号") { booksitem = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "申报单位") { recordunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "法定单位1") { recordunitreducedprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "法定单位2") { legalunits = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "包装规格") { qualification = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "发票号") { legalunitereducedprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "计划数量") { planno = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "计划单位") { planunit = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "折算关系") { reduced = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "打码号") { codeno = columns; }
                    }
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        Entities.Schedule model = new Entities.Schedule();
                        ViewBag.mainid = HttpContext.Session.GetInt32("import");
                        model.MainId = ViewBag.mainid;
                        if (buyer>0&&worksheet.Cells[row, buyer].Value != null)
                        {
                            model.Consignor = worksheet.Cells[row, buyer].Value.ToString();//供应商
                        }
                        if (orderno > 0 && worksheet.Cells[row, orderno].Value != null)
                        {
                            model.OrderNo = worksheet.Cells[row, orderno].Value.ToString();//订单号
                        }
                        if (orderline > 0 && worksheet.Cells[row, orderline].Value != null)
                        {
                            model.OrderLine = worksheet.Cells[row, orderline].Value.ToString();//订单行号
                        }
                        if (referenceno > 0 && worksheet.Cells[row, referenceno].Value != null)
                        {
                            model.ReferenceNo = worksheet.Cells[row, referenceno].Value.ToString();//索引号
                        }
                        if (materialcode > 0 && worksheet.Cells[row, materialcode].Value != null)
                        {
                            model.MaterialCode = worksheet.Cells[row, materialcode].Value.ToString();//物料代码
                        }
                        if (description > 0 && worksheet.Cells[row, description].Value != null)
                        {
                            model.Description = worksheet.Cells[row, description].Value.ToString();//品名
                        }
                        if (type > 0 && worksheet.Cells[row, type].Value != null)
                        {
                            model.Type = worksheet.Cells[row, type].Value.ToString();//牌号
                        }
                        if (specification > 0 && worksheet.Cells[row, specification].Value != null)
                        {
                            model.Specification = worksheet.Cells[row, specification].Value.ToString();//规范
                        }
                        if (thickness > 0 && worksheet.Cells[row, thickness].Value != null)
                        {
                            model.Thickness = worksheet.Cells[row, thickness].Value.ToString();//厚度
                        }
                        if (length > 0 && worksheet.Cells[row, length].Value != null)
                        {
                            model.Length = worksheet.Cells[row, length].Value.ToString();//长
                        }
                        if (width > 0 && worksheet.Cells[row, width].Value != null)
                        {
                            model.Width = worksheet.Cells[row, width].Value.ToString();//宽
                        }
                        if (purchasequantity > 0 && worksheet.Cells[row, purchasequantity].Value != null)
                        {
                            model.PurchaseQuantity = worksheet.Cells[row, purchasequantity].Value.ToString();//采购数量
                        }
                        if (purchaseunit > 0 && worksheet.Cells[row, purchaseunit].Value != null)
                        {
                            model.PurchaseUnit = worksheet.Cells[row, purchaseunit].Value.ToString();//采购单位
                        }
                        if (unitprice > 0 && worksheet.Cells[row, unitprice].Value != null)
                        {
                            model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();//单价
                        }
                        if (totalprice > 0 && worksheet.Cells[row, totalprice].Value != null)
                        {
                            model.TotalPrice = worksheet.Cells[row, totalprice].Value.ToString();//总价
                        }
                        if (shipmentdate > 0 && worksheet.Cells[row, shipmentdate].Value != null)
                        {
                            model.ShipmentDate = Convert.ToDateTime(worksheet.Cells[row, shipmentdate].Value.ToString());//发运日期
                        }
                       
                        if (manufacturers > 0 && worksheet.Cells[row, manufacturers].Value != null)
                        {
                            model.Manufacturers = worksheet.Cells[row, manufacturers].Value.ToString();//制造商
                        }
                        if (origincountry > 0 && worksheet.Cells[row, origincountry].Value != null)
                        {
                            model.OriginCountry = worksheet.Cells[row, origincountry].Value.ToString();//原产国
                        }
                        if (batchno > 0 && worksheet.Cells[row, batchno].Value != null)
                        {
                            model.BatchNo = worksheet.Cells[row, batchno].Value.ToString();//炉批号（质量编号）
                        }
                        if (waybill > 0 && worksheet.Cells[row, waybill].Value != null)
                        {
                            model.Waybill = worksheet.Cells[row, waybill].Value.ToString();//运单号
                        }
                        if (books > 0 && worksheet.Cells[row, books].Value != null)
                        {
                            model.Books = worksheet.Cells[row, books].Value.ToString();//账册号
                        }
                        if (booksitem > 0 && worksheet.Cells[row, booksitem].Value != null)
                        {
                            model.BooksItem = worksheet.Cells[row, booksitem].Value.ToString();//账册项号
                        }
                        if (recordunit > 0 && worksheet.Cells[row, recordunit].Value != null)
                        {
                            model.RecordUnit = worksheet.Cells[row, recordunit].Value.ToString();//申报单位
                        }
                        if (recordunitreducedprice > 0 && worksheet.Cells[row, recordunitreducedprice].Value != null)
                        {
                            model.RecordUnitReducedPrice = worksheet.Cells[row, recordunitreducedprice].Value.ToString();//按申报单位折算单价
                        }
                        if (legalunits > 0 && worksheet.Cells[row, legalunits].Value != null)
                        {
                            model.LegalUnits = worksheet.Cells[row, legalunits].Value.ToString();//法定单位
                        }
                        if (legalunitereducedprice > 0 && worksheet.Cells[row, legalunitereducedprice].Value != null)
                        {
                            model.LegalUniteReducedPrice = worksheet.Cells[row, legalunitereducedprice].Value.ToString();//按法定单位折算单价
                        }
                        if (qualification > 0 && worksheet.Cells[row, qualification].Value != null)
                        {
                            model.InvoiceNo = worksheet.Cells[row, qualification].Value.ToString();//按法定单位折算单价
                        }
                        if (planno > 0 && worksheet.Cells[row, planno].Value != null)
                        {
                            model.PlanNo = Convert.ToDouble(worksheet.Cells[row, planno].Value);//按法定单位折算单价
                        }
                        if (codeno > 0 && worksheet.Cells[row, codeno].Value != null)
                        {
                            model.CodeNo = worksheet.Cells[row, codeno].Value.ToString();//按法定单位折算单价
                        }
                        if (reduced > 0 && worksheet.Cells[row, reduced].Value != null)
                        {
                            model.Reduced = Convert.ToDouble(worksheet.Cells[row, reduced].Value);//按法定单位折算单价
                        }
                        if (planunit > 0 && worksheet.Cells[row, planunit].Value != null)
                        {
                            model.PlanUnit =worksheet.Cells[row, planunit].Value.ToString();//按法定单位折算单价
                        }
                        model.Creator = WorkContext.CurrentUser.Id;
                        model.CreationTime = DateTime.Now;
                        _scheduleService.insertSchedule(model);
                    }
                    catch (Exception e)
                    {
                        ViewData["IsShowAlert"] = "True";
                    }
                }
                return Redirect(ViewBag.ReturnUrl);
            }
        }
        [HttpPost]
        [Route("importexcelupdateschedule", Name = "importexcelupdateschedule")]
        [Function("明细表数据修改", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult Impor2t(Entities.Schedule modelschedule, IFormFile excelfile, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyerSchedule");
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
            string sFileName = excelfile.FileName;
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
                int id = 0;//采购员
                int buyer = 0;//采购员
                int orderno = 0;//订单号
                int orderline = 0;//订单行号
                int referenceno = 0;//索引号
                int materialcode = 0;//物料代码
                int description = 0;//品名
                int type = 0;//牌号
                int specification = 0;//规范
                int thickness = 0;//厚度
                int length = 0;//长
                int width = 0;//宽
                int purchasequantity = 0;//采购数量
                int purchaseunit = 0;//采购单位
                int unitprice = 0;//单价
                int totalprice = 0;//总价
                int shipmentdate = 0;//发运日期
                int consignor = 0;//供应商名称
                int manufacturers = 0;//制造商
                int origincountry = 0;//原产国
                int batchno = 0;//炉批号（质量编号）
                int waybill = 0;//运单号
                int books = 0;//账册号
                int booksitem = 0;//账册项号
                int recordunit = 0;//申报单位
                int recordunitreducedprice = 0;//按申报单位折算单价
                int legalunits = 0;//法定单位
                int legalunitereducedprice = 0;//按法定单位折算单价
                int qualification = 0;//合格证号 
                int planno = 0;//合格证号
                int reduced = 0;//合格证号
                int codeno = 0;
                int planunit = 0;
                
                for (int columns = 1; columns <= ColCount; columns++)
                {
                    //Entities.Order model = new Entities.Order();
                    if (worksheet.Cells[1, columns].Value== null) {
                    if (worksheet.Cells[1, columns].Value.ToString() == "编号") { id = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "供应商") { buyer = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单号") { orderno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单行号") { orderline = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "索引号") { referenceno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "物料代码") { materialcode = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "品名") { description = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "牌号") { type = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规范") { specification = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格1") { thickness = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格2") { length = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格3") { width = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购数量") { purchasequantity = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购单位") { purchaseunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "单价") { unitprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "总价") { totalprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "实际提/送货日期") { shipmentdate = columns; }
                    // if (worksheet.Cells[1, columns].Value.ToString() == "供应商名称") { consignor = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "制造商") { manufacturers = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "原产国") { origincountry = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "炉批号") { batchno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "运单号") { waybill = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "账册号") { books = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "账册项号") { booksitem = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "申报单位") { recordunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "法定单位1") { recordunitreducedprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "法定单位2") { legalunits = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "包装规格") { qualification = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "发票号") { legalunitereducedprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "计划数量") { planno = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "计划单位") { planunit = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "折算关系") { reduced = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "打码号") { codeno = columns; }
                    }
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    if (id==0)
                    {
                        Response.WriteAsync("<script>alert('无明细编号');window.location.href ='itBuyerSchedule'</script>", Encoding.GetEncoding("GB2312"));
                        return Redirect(ViewBag.ReturnUrl);
                    }
                    try
                    {
                        var model = _scheduleService.getById(Convert.ToInt32(worksheet.Cells[row, id].Value));
                        //Entities.Schedule model = new Entities.Schedule();
                       // ViewBag.mainid = HttpContext.Session.GetInt32("import");
                       // model.MainId = ViewBag.mainid;
                        if (buyer > 0 && worksheet.Cells[row, buyer].Value != null)
                        {
                            model.Consignor = worksheet.Cells[row, buyer].Value.ToString();//供应商
                        }
                        if (orderno > 0 && worksheet.Cells[row, orderno].Value != null)
                        {
                            model.OrderNo = worksheet.Cells[row, orderno].Value.ToString();//订单号
                        }
                        if (orderline > 0 && worksheet.Cells[row, orderline].Value != null)
                        {
                            model.OrderLine = worksheet.Cells[row, orderline].Value.ToString();//订单行号
                        }
                        if (referenceno > 0 && worksheet.Cells[row, referenceno].Value != null)
                        {
                            model.ReferenceNo = worksheet.Cells[row, referenceno].Value.ToString();//索引号
                        }
                        if (materialcode > 0 && worksheet.Cells[row, materialcode].Value != null)
                        {
                            model.MaterialCode = worksheet.Cells[row, materialcode].Value.ToString();//物料代码
                        }
                        if (description > 0 && worksheet.Cells[row, description].Value != null)
                        {
                            model.Description = worksheet.Cells[row, description].Value.ToString();//品名
                        }
                        if (type > 0 && worksheet.Cells[row, type].Value != null)
                        {
                            model.Type = worksheet.Cells[row, type].Value.ToString();//牌号
                        }
                        if (specification > 0 && worksheet.Cells[row, specification].Value != null)
                        {
                            model.Specification = worksheet.Cells[row, specification].Value.ToString();//规范
                        }
                        if (thickness > 0 && worksheet.Cells[row, thickness].Value != null)
                        {
                            model.Thickness = worksheet.Cells[row, thickness].Value.ToString();//厚度
                        }
                        if (length > 0 && worksheet.Cells[row, length].Value != null)
                        {
                            model.Length = worksheet.Cells[row, length].Value.ToString();//长
                        }
                        if (width > 0 && worksheet.Cells[row, width].Value != null)
                        {
                            model.Width = worksheet.Cells[row, width].Value.ToString();//宽
                        }
                        if (purchasequantity > 0 && worksheet.Cells[row, purchasequantity].Value != null)
                        {
                            model.PurchaseQuantity = worksheet.Cells[row, purchasequantity].Value.ToString();//采购数量
                        }
                        if (purchaseunit > 0 && worksheet.Cells[row, purchaseunit].Value != null)
                        {
                            model.PurchaseUnit = worksheet.Cells[row, purchaseunit].Value.ToString();//采购单位
                        }
                        if (unitprice > 0 && worksheet.Cells[row, unitprice].Value != null)
                        {
                            model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();//单价
                        }
                        if (totalprice > 0 && worksheet.Cells[row, totalprice].Value != null)
                        {
                            model.TotalPrice = worksheet.Cells[row, totalprice].Value.ToString();//总价
                        }
                        if (shipmentdate > 0 && worksheet.Cells[row, shipmentdate].Value != null)
                        {
                            model.ShipmentDate = Convert.ToDateTime(worksheet.Cells[row, shipmentdate].Value.ToString());//发运日期
                        }

                        if (manufacturers > 0 && worksheet.Cells[row, manufacturers].Value != null)
                        {
                            model.Manufacturers = worksheet.Cells[row, manufacturers].Value.ToString();//制造商
                        }
                        if (origincountry > 0 && worksheet.Cells[row, origincountry].Value != null)
                        {
                            model.OriginCountry = worksheet.Cells[row, origincountry].Value.ToString();//原产国
                        }
                        if (batchno > 0 && worksheet.Cells[row, batchno].Value != null)
                        {
                            model.BatchNo = worksheet.Cells[row, batchno].Value.ToString();//炉批号（质量编号）
                        }
                        if (waybill > 0 && worksheet.Cells[row, waybill].Value != null)
                        {
                            model.Waybill = worksheet.Cells[row, waybill].Value.ToString();//运单号
                        }
                        if (books > 0 && worksheet.Cells[row, books].Value != null)
                        {
                            model.Books = worksheet.Cells[row, books].Value.ToString();//账册号
                        }
                        if (booksitem > 0 && worksheet.Cells[row, booksitem].Value != null)
                        {
                            model.BooksItem = worksheet.Cells[row, booksitem].Value.ToString();//账册项号
                        }
                        if (recordunit > 0 && worksheet.Cells[row, recordunit].Value != null)
                        {
                            model.RecordUnit = worksheet.Cells[row, recordunit].Value.ToString();//申报单位
                        }
                        if (recordunitreducedprice > 0 && worksheet.Cells[row, recordunitreducedprice].Value != null)
                        {
                            model.RecordUnitReducedPrice = worksheet.Cells[row, recordunitreducedprice].Value.ToString();//按申报单位折算单价
                        }
                        if (legalunits > 0 && worksheet.Cells[row, legalunits].Value != null)
                        {
                            model.LegalUnits = worksheet.Cells[row, legalunits].Value.ToString();//法定单位
                        }
                        if (legalunitereducedprice > 0 && worksheet.Cells[row, legalunitereducedprice].Value != null)
                        {
                            model.LegalUniteReducedPrice = worksheet.Cells[row, legalunitereducedprice].Value.ToString();//按法定单位折算单价
                        }
                        if (qualification > 0 && worksheet.Cells[row, qualification].Value != null)
                        {
                            model.InvoiceNo = worksheet.Cells[row, qualification].Value.ToString();//按法定单位折算单价
                        }
                        if (planno > 0 && worksheet.Cells[row, planno].Value != null)
                        {
                            model.PlanNo = Convert.ToDouble(worksheet.Cells[row, planno].Value);//按法定单位折算单价
                        }
                        if (codeno > 0 && worksheet.Cells[row, codeno].Value != null)
                        {
                            model.CodeNo = worksheet.Cells[row, codeno].Value.ToString();//按法定单位折算单价
                        }
                        if (reduced > 0 && worksheet.Cells[row, reduced].Value != null)
                        {
                            model.Reduced = Convert.ToDouble(worksheet.Cells[row, reduced].Value);//按法定单位折算单价planunit
                        }
                        if (planunit > 0 && worksheet.Cells[row, planunit].Value != null)
                        {
                            model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();//按法定单位折算单价
                        }
                        model.Modifier = WorkContext.CurrentUser.Id;
                        model.ModifiedTime = DateTime.Now;
                        _scheduleService.updateSchedule(model);
                    }
                    catch (Exception e)
                    {
                        ViewData["IsShowAlert"] = "True";
                    }
                }
                return Redirect(ViewBag.ReturnUrl);
            }
        }
        [Route("itBuyercopy", Name = "itBuyercopy")]
        [Function("明细数据复制", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult ItBuyerCopy(int id)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("itBuyerSchedule");
            var model = _scheduleService.getById(id);
            Schedule record = new Schedule();
            record.BatchNo = model.BatchNo;
            record.Books = model.Books;
            record.BooksItem = model.BooksItem;
            record.Buyer = model.Buyer;
            record.CodeNo = model.CodeNo;
            record.Consignor = model.Consignor;
            record.CreationTime = DateTime.Now;
            record.Creator = WorkContext.CurrentUser.Id;
            record.Description = model.Description;
            record.InvoiceNo = model.InvoiceNo;
            record.IsDeleted = model.IsDeleted;
            record.LegalUniteReducedPrice = model.LegalUniteReducedPrice;
            record.LegalUnits = model.LegalUnits;
            record.Length = model.Length;
            record.MainId = model.MainId;
            record.Manufacturers = model.Manufacturers;
            record.MaterialCode = model.MaterialCode;
            record.OrderLine = model.OrderLine;
            record.OrderNo = model.OrderNo;
            record.OriginCountry = model.OriginCountry;
            record.Package = model.Package;
            record.PlanNo = model.PlanNo;
            record.PlanUnit = model.PlanUnit;
            record.PurchaseQuantity = model.PurchaseQuantity;
            record.PurchaseUnit = model.PurchaseUnit;
            record.Qualification = model.Qualification;
            record.RecordUnit = model.RecordUnit;
            record.RecordUnitReducedPrice = model.RecordUnitReducedPrice;
            record.Reduced = model.Reduced;
            record.ReducedNo = model.ReducedNo;
            record.ReferenceNo = model.ReferenceNo;
            record.ShipmentDate = model.ShipmentDate;
            record.Specification = model.Specification;
            record.Thickness = model.Thickness;
            record.TotalPrice = model.TotalPrice;
            record.Type = model.Type;
            record.UnitPrice = model.UnitPrice;
            record.Waybill = model.Waybill;
            record.Width = model.Width;
            _scheduleService.insertSchedule(record);
            return Redirect(ViewBag.ReturnUrl);

        }
        [HttpPost]
        [Route("itBuyerList", Name = "itBuyerLista")]
        [Function("要求到货时间填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult ITBuyerList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            try
            {
                foreach (Entities.ImportTrans_main_record u in jsonlist)
                {
                    var model = _importTrans_main_recordService.getById(u.Id);
                    model.Incoterms = u.Incoterms;
                    model.RequestedArrivalTime = u.RequestedArrivalTime;
                    _importTrans_main_recordService.updateImportTransmain(model);
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
        [HttpPost]
        [Route("itBuyerScheduleList", Name = "itBuyerScheduleList")]
        [Function("明细表数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult ITBuyerScheduleList(string kevin)
        {
            string test = kevin;
            List<Entities.Schedule> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Schedule>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                foreach (Entities.Schedule u in jsonlist)
                {
                    var model = _scheduleService.getById(u.Id);
                    if (u.PurchaseQuantity!="")
                    {
                        model.PurchaseQuantity = u.PurchaseQuantity;
                    }
                    if (u.UnitPrice != "")
                    {
                        model.UnitPrice = u.UnitPrice;
                    }
                    if (u.PurchaseUnit != "")
                    {
                        model.PurchaseUnit = u.PurchaseUnit;
                    }
                   
                    
                    if(u.TotalPrice== model.TotalPrice&& model.PurchaseQuantity != null&& model.PurchaseUnit != null) {
                        model.TotalPrice =Convert.ToString(Convert.ToDouble(model.PurchaseQuantity) * Convert.ToDouble(model.UnitPrice));
                    }
                    else {
                        model.TotalPrice = u.TotalPrice;
                    }
                    if (u.ShipmentDate != null)
                    {
                        model.ShipmentDate = u.ShipmentDate;
                    }
                   
                    if (u.Manufacturers != "")
                    {

                        model.Manufacturers = u.Manufacturers;
                    }
                    if (u.OriginCountry != "")
                    {
                        model.OriginCountry = u.OriginCountry;
                    }
                    if (u.Books != "")
                    {
                        model.Books = u.Books;
                    }
                    if (u.BooksItem != "")
                    {
                         model.BooksItem = u.BooksItem;
                    }
                    if (u.Waybill != "")
                    {
                        model.Waybill = u.Waybill;
                    }
                    if (u.BatchNo != "")
                    {
                        model.BatchNo = u.BatchNo;
                    }
                    if (u.RecordUnit != "")
                    {
                        model.RecordUnit = u.RecordUnit;
                    }
                    if (u.RecordUnitReducedPrice != "")
                    {
                        model.RecordUnitReducedPrice = u.RecordUnitReducedPrice;
                    }
                    if (u.LegalUnits != "")
                    {
                        model.LegalUnits = u.LegalUnits;
                    }
                    if (u.LegalUniteReducedPrice != "")
                    {
                        model.LegalUniteReducedPrice = u.LegalUniteReducedPrice;
                    }
                    if (u.Qualification != "")
                    {
                        model.Qualification = u.Qualification;
                    }
                    if (u.PlanUnit != "")
                    {
                        model.PlanUnit = u.PlanUnit;
                    }
                    if (u.Reduced != 0)
                    {
                        model.Reduced = u.Reduced;
                    }
                    

                    if (model.PlanNo > 0 && model.Reduced > 0)
                    {
                        double number = Convert.ToDouble(model.Reduced * model.PlanNo);
                        model.ReducedNo = number.ToString();
                    }

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
        [HttpPost]
        [Route("schedule", Name = "excelimportasja")]
        [Function("送检审批生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export3(List<int> checkboxId)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionbuyersch");
            string date = DateTime.Now.ToString("yyMMdd");

            var serial = _sysInspectionMainService.getByDate(date);
            int ser = serial.Count + 1;
            string inmain = "";
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                if (ser < 10) {
                    inmain = date + "0" +Convert.ToString(ser)+WorkContext.CurrentUser.Account;
                }
                else{
                    inmain = date + Convert.ToString(ser) + WorkContext.CurrentUser.Account;
                }
               
                InspecationMain modelm = new InspecationMain();
                modelm.InspecationMainId = inmain;
                modelm.Serial = ser;
                modelm.flag = 0;
                modelm.DateId = date;
                modelm.Creator = WorkContext.CurrentUser.Name;
                modelm.CreationTime = DateTime.Now;
                _sysInspectionMainService.insertInspecationMain(modelm);
                var main = _sysInspectionMainService.getByAccount(inmain);
                string yundan = "";
                string dingdan = "";
                foreach (int u in checkboxId)
                {
                    var models = _scheduleService.getById(u);
                    Inspection model = new Inspection();
                    model.ContractNo = models.OrderNo;
                    var supplier = _sysOrderService.getAccount(models.ReferenceNo);
                    model.Supplier = supplier.SupplierName;
                    model.Manufacturer = supplier.Manufacturer;
                    var serialcoc = _sysInspectionService.getByDate(date);
                    int sercoc = serialcoc.Count + 1;
                    string coc = "";
                    if (sercoc < 10)
                    {
                        coc = "COC" + date + "000" + Convert.ToString(sercoc);
                    }
                    else if(sercoc >= 10 && sercoc < 100)
                    {
                        coc = "COC" + date + "00" + Convert.ToString(sercoc);
                    }
                    else if (sercoc >= 100 && sercoc < 1000)
                    {
                        coc = "COC" + date + "0" + Convert.ToString(sercoc);
                    }
                    else
                    {
                        coc = "COC" + date  + Convert.ToString(sercoc);
                    }
                    model.Serial = sercoc;
                    model.DateId = date;
                    model.CofC = coc;
                    model.Description = models.Description;
                    model.MaterialCode = models.MaterialCode;
                    model.Type = supplier.PartNo;
                    model.Size = supplier.Size;
                    model.Item = supplier.Item;
                    model.Project = supplier.Main.Project;
                    model.Specification = models.Specification;
                    model.Batch = models.BatchNo;
                    if (models.PurchaseQuantity != "" && models.PurchaseQuantity != null) { model.Qty = Convert.ToInt32(models.PurchaseQuantity); }
                    
                    model.UnPlaceQty = model.Qty;
                    model.Creator = WorkContext.CurrentUser.Name;
                    model.CreationTime = DateTime.Now;
                    model.MainId = main.Id;
                    _sysInspectionService.insertInspection(model);
                    if (!yundan.Contains(models.Waybill))
                    {
                        yundan = yundan + models.Waybill + ";";
                    }
                    if (!dingdan.Contains(models.OrderNo))
                    {
                        dingdan = dingdan + models.OrderNo + ";";
                    }
                    _sysInspectionService.insertInspection(model);
                    //u就是jsonlist里面的一个实体类
                }
                Response.Cookies.Append("Inspection", Convert.ToString(main.Id));
                
            }
            catch
            {
                var main = _sysInspectionMainService.getByAccount(inmain);
                main.IsDeleted = true;
                _sysInspectionMainService.updateInspecationMain(main);
               
            }
            return Redirect(ViewBag.ReturnUrl);

        }
        [Route("inspectionbuyersch", Name = "inspectionbuyersch")]
        [Function("送检明细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        [HttpGet]
        public IActionResult InspectionBuyerSchIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var customizedList2 = _sysUserService.getJhy();
            ViewData["Jhy"] = new SelectList(customizedList2, "Name", "Name");
            string s;
             Request.Cookies.TryGetValue("Inspection", out s);
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionService.searchInspection(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionbuyerschupdate", Name = "inspectionbuyerschupdate")]
        [Function("送检单复制", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        [HttpGet]
        public IActionResult InspectionSchupdate(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionbuyersch");
            var model = _sysInspectionService.getById(id);
            Inspection record = new Inspection();
            record.MainId = model.MainId;
            record.Manufacturer = model.Manufacturer;
            record.MaterialCode = model.MaterialCode;
            
            record.Qty = model.Qty;
            record.ReceivedDate = model.ReceivedDate;
            record.Size = model.Size;
            record.Specification = model.Specification;
            record.Status = model.Status;
            record.Supplier = model.Supplier;
            record.Type = model.Type;
            record.UnPlaceQty = model.UnPlaceQty;
            record.AcceptQty = model.AcceptQty;
            record.Batch = model.Batch;
            string date = DateTime.Now.ToString("yyMMdd");
            var serialcoc = _sysInspectionService.getByDate(date);
            int sercoc = serialcoc.Count + 1;
            string coc = "";
            if (sercoc < 10)
            {
                coc = "COC" + date + "000" + Convert.ToString(sercoc);
            }
            else if (sercoc >= 10 && sercoc < 100)
            {
                coc = "COC" + date + "00" + Convert.ToString(sercoc);
            }
            else if (sercoc >= 100 && sercoc < 1000)
            {
                coc = "COC" + date + "0" + Convert.ToString(sercoc);
            }
            else
            {
                coc = "COC" + date + Convert.ToString(sercoc);
            }
            record.Serial = sercoc;
            record.DateId = date;
            record.CofC = coc;
            record.ContractNo = model.ContractNo;
            record.Item = model.Item;
            record.Project = model.Project;
            record.flag = model.flag;
            record.Description = model.Description;
            record.Creator = WorkContext.CurrentUser.Account;
            record.CreationTime = DateTime.Now;
            _sysInspectionService.insertInspection(record);
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpPost]
        [Route("InspectionbuyerList", Name = "InspectionbuyerList")]
        [Function("送检单修改", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult InspectionList(string kevin)
        {
            string test = kevin;
            List<Entities.Inspection> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Inspection>(test);
            try
            {
                foreach (Entities.Inspection u in jsonlist)
                {
                    var model = _sysInspectionService.getById(u.Id);
                    model.Supplier = u.Supplier;
                    model.Manufacturer = u.Manufacturer;
                    model.Description = u.Description;
                    model.MaterialCode = u.MaterialCode;
                    model.Type = u.Type;
                    model.Size = u.Size;
                    model.Batch = u.Batch;
                    model.Batch = u.Batch;
                    model.ReceivedDate = u.ReceivedDate;
                    model.Specification = u.Specification;
                   
                    model.UnPlaceQty = u.UnPlaceQty;
                    model.Qty = u.Qty;
                    model.Creator = WorkContext.CurrentUser.Account;
                    model.CreationTime = DateTime.Now;
                    //model.Status = "计划员审批";
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                    _sysInspectionService.updateInspection(model);
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
        
        [Route("InspectionbuyerspList", Name = "InspectionbuyerspList")]
        [Function("送检单提交", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult InspectionTList(string Jhy)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("itBuyerSchedule");
            string s;
            Request.Cookies.TryGetValue("Inspection", out s);
            int ida = Convert.ToInt32(s);
            try
            {
                var model = _sysInspectionMainService.getById(ida);
                model.Status = "计划员";
                model.flag =1;
                model.JhyName = Jhy;
                _sysInspectionMainService.spInspecationMain(model);
            }
            catch
            {
                return Redirect(ViewBag.ReturnUrl);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpPost]
        [Route("itBuyerScheduleDeleteList", Name = "itBuyerScheduleDeleteList")]
        [Function("明细表数据删除", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult ITBuyerScheduleDeleteList(string kevin)
        {
            string test = kevin;
            List<Entities.Schedule> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Schedule>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                foreach (Entities.Schedule u in jsonlist)
                {
                    var model = _scheduleService.getById(u.Id);
                    model.IsDeleted = true;
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
        [HttpPost]
        [Route("itOrderBuyerList", Name = "itOrderBuyerList")]
        [Function("订单数据批量导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult ITOrderBuyerList(string kevin)
        {
            string test = kevin;
            List<Entities.Order> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Order>(test);
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var main = _importTrans_main_recordService.getById(ViewBag.Import);
            
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                foreach (Entities.Order u in jsonlist)
                {
                    var item = _sysOrderService.getById(u.Id);
                    Entities.Schedule model = new Entities.Schedule();
                    model.Buyer = item.OrderSigner;
                    model.OrderNo = item.OrderNo;
                    model.OrderLine = item.Item;
                    model.ReferenceNo = item.PlanItem;
                    model.MaterialCode = item.MaterialCode;
                    model.Description = item.Name;
                    model.Type = item.Project;
                    model.Specification = item.Specification;
                    model.Thickness = item.Size;
                    model.Manufacturers = item.Manufacturer;
                    model.OriginCountry = item.Origin;
                    model.Length = item.Length;
                    model.Width = item.Width;
                    model.Package = item.Package;
                    model.PartNo = item.PartNo;
                    model.PurchaseQuantity = item.Qty;
                    model.PurchaseUnit = item.Unit;
                    model.PlanUnit = item.PlanUnit;
                    var itema = _sysOrderMainService.getById(item.MainId);
                    model.Consignor = itema.SupplierName;
                    model.CodeNo = itema.CodeNo;
                    model.Reduced = item.Reduced;
                    model.MainId = ViewBag.Import;
                    model.Waybill = main.Mbl;
                    model.UnitPrice = item.UnitPrice;
                    model.TotalPrice = item.TotalPrice;
                    model.CreationTime = DateTime.Now;
                    model.Creator = WorkContext.CurrentUser.Id;
                    model.ShipmentDate = main.Atd;
                    _scheduleService.insertSchedule(model);
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
        [HttpGet]
        [Route("edit", Name = "editBuyer")]
        [Function("编辑发运条目", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult EditBuyer(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyer");
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
        public ActionResult EditBuyer(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyer");
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
            // model.PoNo = model.PoNo.Substring(0, 2);
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
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit2", Name = "editBuyerSchedule")]
        [Function("编辑明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerScheduleIndex")]
        public IActionResult EditBuyerSchedule(int? id, string returnUrl = null)
        {//页面跳转未完成
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyerSchedule");
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
        public ActionResult EditBuyerSchedule(Entities.Schedule model, string returnUrl = null)
        {//页面跳转未完成
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyerSchedule");
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
        [Route("BuyerAttachment", Name = "BuyerAttachment")]
        [Function("运代附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        [HttpGet]
        public IActionResult ITBuyerAttachmentIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _attachmentService.searchAttachment(arg, page, size, id);
            ViewBag.Arg = arg;//传参数ITTransportAttachmentIndex
            var dataSource = pageList.toDataSourceResult<Entities.Attachment, SysCustomizedListSearchArg>("itBuyer", arg);
            return View(dataSource);//sysImport
        }
        [Route("downLoadbuyer", Name = "downLoadbuyer")]
        [Function("下载附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Download(int? id)
        {
            string load = "";

            var model = _attachmentService.getById(id.Value);
            if (model.Type == "主运单")
            {
                load = "\\Files\\mbl\\";
            }
            else if (model.Type == "箱单发票")
            {
                load = "\\Files\\fp\\";
            }
            else
            {
                load = "\\Files\\hbl\\";
            }
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
        //[Route("excelimportasja", Name = "excelimportasja")]
        //[Function("货物交接单生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        //public IActionResult Export3()
        //{
        //  //  ViewBag.Import = HttpContext.Session.GetInt32("import");
        //    //var list = _scheduleService.getAll(ViewBag.Import);
        //    string sWebRootFolder = _hostingEnvironment.WebRootPath;
        //    string sFileName = "货物交接单" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
        //    FileInfo finfo = new FileInfo(sWebRootFolder + "\\Files\\货物交接单.xlsx");
        //    finfo.CopyTo(sWebRootFolder + "\\Files\\sjdfile\\"+ sFileName,true);

        //    FileInfo file = new FileInfo(sWebRootFolder + "\\Files\\sjdfile\\"+ sFileName);
        //   // file.Delete();
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        // 添加worksheet
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
        //        ////添加头
        //        worksheet.Cells[2, 2].Value = "采购员";
        //        worksheet.Cells[2, 12].Value = "订单号";
        //        // worksheet.InsertRow(15,3);

        //        //worksheet.Cells[15].Copy(worksheet.Cells[20, 1]);
        //        worksheet.Cells[15,  2].Copy(worksheet.Cells[20,  2]);
        //        worksheet.Cells[15,  3].Copy(worksheet.Cells[20,  3]);
        //        worksheet.Cells[15,  4].Copy(worksheet.Cells[20,  4]);
        //        worksheet.Cells[15,  5].Copy(worksheet.Cells[20,  5]);
        //        worksheet.Cells[15,  6].Copy(worksheet.Cells[20,  6]);
        //        worksheet.Cells[15,  7].Copy(worksheet.Cells[20,  7]);
        //        worksheet.Cells[15,  8].Copy(worksheet.Cells[20,  8]);
        //        worksheet.Cells[15,  9].Copy(worksheet.Cells[20,  9]);
        //        worksheet.Cells[15, 10].Copy(worksheet.Cells[20, 10]);
        //        worksheet.Cells[15, 11].Copy(worksheet.Cells[20, 11]);
        //        worksheet.Cells[15, 12].Copy(worksheet.Cells[20, 12]);

        //        package.Save();
        //    }
        //    return File("\\Files\\sjdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        //}
        //[HttpPost]
        //[Route("importexcells", Name = "importexcells")]
        //public ActionResult Importa(Entities.Schedule modelschedule, IFormFile excelfile, string returnUrl = null)
        //{
        //    ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyerSchedule");
        //    string sWebRootFolder = _hostingEnvironment.WebRootPath;
        //    var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
        //    string sFileName = excelfile.FileName;
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
        //        int buyer = 0;//采购员
        //        int orderno = 0;//订单号

        //        for (int columns = 1; columns <= ColCount; columns++)
        //        {
        //            //Entities.Order model = new Entities.Order();
        //            if (worksheet.Cells[1, columns].Value.ToString() == "表1") { buyer = columns; }
        //            if (worksheet.Cells[1, columns].Value.ToString() == "项目代码表") { orderno = columns; }

        //        }
        //        for (int row = 2; row <= rowCount; row++)
        //        {
        //            try
        //            {
        //                Entities.Supplier model = new Entities.Supplier();

        //                if (worksheet.Cells[row, buyer].Value != null)
        //                {
        //                    model.SupplierCode = worksheet.Cells[row, buyer].Value.ToString();//采购员
        //                }
        //                if (worksheet.Cells[row, orderno].Value != null)
        //                {
        //                    model.Describe = worksheet.Cells[row, orderno].Value.ToString();//订单号
        //                }

        //                _sysSupplierService.insertSupplier(model);
        //            }
        //            catch (Exception e)
        //            {
        //                ViewData["IsShowAlert"] = "True";
        //            }
        //        }
        //        return Redirect(ViewBag.ReturnUrl);
        //    }
        //}
    }
}