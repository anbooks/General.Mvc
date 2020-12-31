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
        private IOrderService _sysOrderService;
        private IMaterialService _sysMaterialService;
        private IProjectService _sysProjectService;

        public ITBuyerController(IProjectService sysProjectService, IMaterialService sysMaterialService, ISysUserRoleService sysUserRoleService, IOrderService sysOrderService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._sysProjectService = sysProjectService;
            this._sysMaterialService = sysMaterialService;
            this._sysOrderService = sysOrderService;
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itBuyer")]
        [Function("采购员页面(新)", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]
        [HttpGet]
        public IActionResult ITBuyerIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            // var USER = _sysUserRoleService.getById(WorkContext.CurrentUser.Id);
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _importTrans_main_recordService.searchListBuyer(arg, page, size, WorkContext.CurrentUser.Co);

            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itBuyer", arg);
            return View(dataSource);//sysImport
        }
        [Route("schedule", Name = "itBuyerSchedule")]
        [Function("明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
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
            if (pageList.Count > 0)
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
        [Function("订单数据导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult ITOrderBuyerIndex(string orderno, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var pageList = _sysOrderService.searchOrderD(arg, page, size, orderno);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Order, SysCustomizedListSearchArg>("itOrderImportIndex", arg);
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
                worksheet.Cells[1, 6].Value = "型号";
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
        [Route("excelimportasja", Name = "excelimportasja")]
        [Function("货物交接单生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export3()
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var list = _scheduleService.getAll(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "货物交接单" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\sjdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("货物交接单");
                //添加头
                worksheet.Cells[1, 1].Value = "采购员";
                worksheet.Cells[1, 2].Value = "订单号";
                worksheet.Cells[1, 3].Value = "索引号";
                worksheet.Cells[1, 4].Value = "物料代码";
                worksheet.Cells[1, 5].Value = "品名";
                worksheet.Cells[1, 6].Value = "型号";
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
                int i = 0;
                for (int a = 0; a <= list.Count - 1; a++)
                {
                    if (list[a].BatchNo != null)
                    {
                        string s = list[a].BatchNo.ToString();
                        string[] sArray = s.Split('@');
                        foreach (string b in sArray)
                        {
                            if (list[a].Buyer.ToString() != null)
                            {
                                worksheet.Cells[i + 2, 1].Value = list[a].Buyer.ToString();
                            }
                            if (list[a].OrderNo.ToString() != null)
                            {
                                worksheet.Cells[i + 2, 2].Value = list[a].OrderNo.ToString();
                            }
                            if (list[a].ReferenceNo.ToString() != null)
                            { worksheet.Cells[i + 2, 3].Value = list[a].ReferenceNo.ToString(); }
                            if (list[a].MaterialCode.ToString() != null)
                            { worksheet.Cells[i + 2, 4].Value = list[a].MaterialCode.ToString(); }
                            if (list[a].Description.ToString() != null)
                            { worksheet.Cells[i + 2, 5].Value = list[a].Description.ToString(); }
                            if (list[a].Type.ToString() != null)
                            { worksheet.Cells[i + 2, 6].Value = list[a].Type.ToString(); }
                            if (list[a].Specification.ToString() != null)
                            { worksheet.Cells[i + 2, 7].Value = list[a].Specification.ToString(); }
                            if (list[a].Thickness.ToString() != null)
                            { worksheet.Cells[i + 2, 8].Value = list[a].Thickness; }
                            if (list[a].Length.ToString() != null)
                            { worksheet.Cells[i + 2, 9].Value = Convert.ToInt32(list[i].Length.ToString()); }
                            if (list[a].Width.ToString() != null)
                            { worksheet.Cells[i + 2, 10].Value = Convert.ToInt32(list[i].Width.ToString()); }
                            if (list[a].PurchaseQuantity.ToString() != null)
                            { worksheet.Cells[i + 2, 11].Value = list[a].PurchaseQuantity.ToString(); }
                            if (list[a].PurchaseUnit.ToString() != null)
                            { worksheet.Cells[i + 2, 12].Value = list[a].PurchaseUnit.ToString(); }
                            if (list[a].Manufacturers.ToString() != null)
                            { worksheet.Cells[i + 2, 13].Value = list[a].Manufacturers.ToString(); }
                            if (list[a].BatchNo.ToString() != null)
                            { worksheet.Cells[i + 2, 14].Value = b; }
                            if (list[a].Waybill.ToString() != null)
                            { worksheet.Cells[i + 2, 15].Value = list[a].Waybill.ToString(); }
                            i++;
                        }
                    }
                    else
                    {
                        ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("itBuyerSchedule");
                        return Redirect(ViewBag.ReturnUrl);
                    }
                }
                package.Save();
            }
            return File("\\Files\\sjdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [Route("excelimportaej", Name = "excelimportaej")]
        [Function("二检单生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export2()
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var list = _scheduleService.getAll(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "二检单" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("二检单");
                //添加头
                worksheet.Cells[1, 1].Value = "采购员";
                worksheet.Cells[1, 2].Value = "订单号";
                worksheet.Cells[1, 3].Value = "索引号";
                worksheet.Cells[1, 4].Value = "物料代码";
                worksheet.Cells[1, 5].Value = "品名";
                worksheet.Cells[1, 6].Value = "型号";
                worksheet.Cells[1, 7].Value = "规范";
                worksheet.Cells[1, 8].Value = "厚度";
                worksheet.Cells[1, 9].Value = "长";
                worksheet.Cells[1, 10].Value = "宽";
                worksheet.Cells[1, 11].Value = "采购数量";
                worksheet.Cells[1, 12].Value = "采购单位";
                worksheet.Cells[1, 13].Value = "单价";
                worksheet.Cells[1, 14].Value = "总价";
                worksheet.Cells[1, 15].Value = "发运日期";
                worksheet.Cells[1, 16].Value = "发货人";
                worksheet.Cells[1, 17].Value = "制造商";
                worksheet.Cells[1, 18].Value = "原产国";
                worksheet.Cells[1, 19].Value = "炉批号";
                worksheet.Cells[1, 20].Value = "运单号";
                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 20; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }
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
                    {
                        worksheet.Cells[i + 2, 9].Value = Convert.ToInt32(list[i].Length.ToString());
                    }
                    if (list[i].Width.ToString() != null)
                    { worksheet.Cells[i + 2, 10].Value = Convert.ToInt32(list[i].Width.ToString()); }
                    if (list[i].PurchaseQuantity.ToString() != null)
                    { worksheet.Cells[i + 2, 11].Value = list[i].PurchaseQuantity.ToString(); }
                    if (list[i].PurchaseUnit.ToString() != null)
                    { worksheet.Cells[i + 2, 12].Value = list[i].PurchaseUnit.ToString(); }
                    if (list[i].UnitPrice.ToString() != null)
                    { worksheet.Cells[i + 2, 13].Value = list[i].UnitPrice.ToString(); }
                    if (list[i].TotalPrice.ToString() != null)
                    { worksheet.Cells[i + 2, 14].Value = list[i].TotalPrice.ToString(); }
                    if (list[i].ShipmentDate != null)
                    { worksheet.Cells[i + 2, 15].Value = list[i].ShipmentDate.ToString(); }
                    if (list[i].Consignor.ToString() != null)
                    { worksheet.Cells[i + 2, 16].Value = list[i].Consignor.ToString(); }
                    if (list[i].Manufacturers.ToString() != null)
                    { worksheet.Cells[i + 2, 17].Value = list[i].Manufacturers.ToString(); }
                    if (list[i].OriginCountry.ToString() != null)
                    { worksheet.Cells[i + 2, 18].Value = list[i].OriginCountry.ToString(); }
                    if (list[i].BatchNo.ToString() != null)
                    { worksheet.Cells[i + 2, 19].Value = list[i].BatchNo.ToString(); }
                    if (list[i].Waybill.ToString() != null)
                    { worksheet.Cells[i + 2, 20].Value = list[i].Waybill.ToString(); }
                }
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
                int type = 0;//型号
                int specification = 0;//规范
                int thickness = 0;//厚度
                int length = 0;//长
                int width = 0;//宽
                int purchasequantity = 0;//采购数量
                int purchaseunit = 0;//采购单位
                int unitprice = 0;//单价
                int totalprice = 0;//总价
                int shipmentdate = 0;//发运日期
                int consignor = 0;//发货人
                int manufacturers = 0;//制造商
                int origincountry = 0;//原产国
                int batchno = 0;//炉批号（质量编号）
                int waybill = 0;//运单号
                int books = 0;//账册号
                int booksitem = 0;//账册项号
                int recordunit = 0;//备案单位
                int recordunitreducedprice = 0;//按备案单位折算单价
                int legalunits = 0;//法定单位
                int legalunitereducedprice = 0;//按法定单位折算单价
                int qualification = 0;//合格证号
                for (int columns = 1; columns <= ColCount; columns++)
                {
                    //Entities.Order model = new Entities.Order();
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购员") { buyer = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单号") { orderno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单行号") { orderline = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "索引号") { referenceno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "物料代码") { materialcode = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "品名") { description = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "型号") { type = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规范") { specification = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "厚度") { thickness = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "长") { length = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "宽") { width = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购数量") { purchasequantity = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购单位") { purchaseunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "单价") { unitprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "总价") { totalprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "发运日期") { shipmentdate = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "发货人") { consignor = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "制造商") { manufacturers = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "原产国") { origincountry = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "炉批号") { batchno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "运单号") { waybill = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "账册号") { books = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "账册项号") { booksitem = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "备案单位") { recordunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "按备案单位折算单价") { recordunitreducedprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "法定单位") { legalunits = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "按法定单位折算单价") { legalunitereducedprice = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "合格证号") { qualification = columns; }
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        Entities.Schedule model = new Entities.Schedule();
                        ViewBag.mainid = HttpContext.Session.GetInt32("import");
                        model.MainId = ViewBag.mainid;
                        if (worksheet.Cells[row, buyer].Value != null)
                        {
                            model.Buyer = worksheet.Cells[row, buyer].Value.ToString();//采购员
                        }
                        if (worksheet.Cells[row, orderno].Value != null)
                        {
                            model.OrderNo = worksheet.Cells[row, orderno].Value.ToString();//订单号
                        }
                        if (worksheet.Cells[row, orderline].Value != null)
                        {
                            model.OrderLine = worksheet.Cells[row, orderline].Value.ToString();//订单行号
                        }
                        if (worksheet.Cells[row, referenceno].Value != null)
                        {
                            model.ReferenceNo = worksheet.Cells[row, referenceno].Value.ToString();//索引号
                        }
                        if (worksheet.Cells[row, materialcode].Value != null)
                        {
                            model.MaterialCode = worksheet.Cells[row, materialcode].Value.ToString();//物料代码
                        }
                        if (worksheet.Cells[row, description].Value != null)
                        {
                            model.Description = worksheet.Cells[row, description].Value.ToString();//品名
                        }
                        if (worksheet.Cells[row, type].Value != null)
                        {
                            model.Type = worksheet.Cells[row, type].Value.ToString();//型号
                        }
                        if (worksheet.Cells[row, specification].Value != null)
                        {
                            model.Specification = worksheet.Cells[row, specification].Value.ToString();//规范
                        }
                        if (worksheet.Cells[row, thickness].Value != null)
                        {
                            model.Thickness = worksheet.Cells[row, thickness].Value.ToString();//厚度
                        }
                        if (worksheet.Cells[row, length].Value != null)
                        {
                            model.Length = worksheet.Cells[row, length].Value.ToString();//长
                        }
                        if (worksheet.Cells[row, width].Value != null)
                        {
                            model.Width = worksheet.Cells[row, width].Value.ToString();//宽
                        }
                        if (worksheet.Cells[row, purchasequantity].Value != null)
                        {
                            model.PurchaseQuantity = worksheet.Cells[row, purchasequantity].Value.ToString();//采购数量
                        }
                        if (worksheet.Cells[row, purchaseunit].Value != null)
                        {
                            model.PurchaseUnit = worksheet.Cells[row, purchaseunit].Value.ToString();//采购单位
                        }
                        if (worksheet.Cells[row, unitprice].Value != null)
                        {
                            model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();//单价
                        }
                        if (worksheet.Cells[row, totalprice].Value != null)
                        {
                            model.TotalPrice = worksheet.Cells[row, totalprice].Value.ToString();//总价
                        }
                        if (worksheet.Cells[row, shipmentdate].Value != null)
                        {
                            model.ShipmentDate = Convert.ToDateTime(worksheet.Cells[row, shipmentdate].Value.ToString());//发运日期
                        }
                        if (worksheet.Cells[row, consignor].Value != null)
                        {
                            model.Consignor = worksheet.Cells[row, consignor].Value.ToString();//发货人
                        }
                        if (worksheet.Cells[row, manufacturers].Value != null)
                        {
                            model.Manufacturers = worksheet.Cells[row, manufacturers].Value.ToString();//制造商
                        }
                        if (worksheet.Cells[row, origincountry].Value != null)
                        {
                            model.OriginCountry = worksheet.Cells[row, origincountry].Value.ToString();//原产国
                        }
                        if (worksheet.Cells[row, batchno].Value != null)
                        {
                            model.BatchNo = worksheet.Cells[row, batchno].Value.ToString();//炉批号（质量编号）
                        }
                        if (worksheet.Cells[row, waybill].Value != null)
                        {
                            model.Waybill = worksheet.Cells[row, waybill].Value.ToString();//运单号
                        }
                        if (worksheet.Cells[row, books].Value != null)
                        {
                            model.Books = worksheet.Cells[row, books].Value.ToString();//账册号
                        }
                        if (worksheet.Cells[row, booksitem].Value != null)
                        {
                            model.BooksItem = worksheet.Cells[row, booksitem].Value.ToString();//账册项号
                        }
                        if (worksheet.Cells[row, recordunit].Value != null)
                        {
                            model.RecordUnit = worksheet.Cells[row, recordunit].Value.ToString();//备案单位
                        }
                        if (worksheet.Cells[row, recordunitreducedprice].Value != null)
                        {
                            model.RecordUnitReducedPrice = worksheet.Cells[row, recordunitreducedprice].Value.ToString();//按备案单位折算单价
                        }
                        if (worksheet.Cells[row, legalunits].Value != null)
                        {
                            model.LegalUnits = worksheet.Cells[row, legalunits].Value.ToString();//法定单位
                        }
                        if (worksheet.Cells[row, legalunitereducedprice].Value != null)
                        {
                            model.LegalUniteReducedPrice = worksheet.Cells[row, legalunitereducedprice].Value.ToString();//按法定单位折算单价
                        }
                        if (worksheet.Cells[row, qualification].Value != null)
                        {
                            model.Qualification = worksheet.Cells[row, qualification].Value.ToString();//合格证号
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
        //                Entities.Project model = new Entities.Project();

        //                if (worksheet.Cells[row, buyer].Value != null)
        //                {
        //                    model.ProjectCode = worksheet.Cells[row, buyer].Value.ToString();//采购员
        //                }
        //                if (worksheet.Cells[row, orderno].Value != null)
        //                {
        //                    model.Describe = worksheet.Cells[row, orderno].Value.ToString();//订单号
        //                }

        //                _sysProjectService.insertProject(model);
        //            }
        //            catch (Exception e)
        //            {
        //                ViewData["IsShowAlert"] = "True";
        //            }
        //        }
        //        return Redirect(ViewBag.ReturnUrl);
        //    }
        //}
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
        [HttpPost]
        [Route("itOrderBuyerList", Name = "itOrderBuyerList")]
        [Function("订单数据批量导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public ActionResult ITOrderBuyerList(string kevin)
        {
            string test = kevin;
            List<Entities.Order> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Order>(test);
            ViewBag.Import = HttpContext.Session.GetInt32("import");
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
                    model.Length = item.Length;
                    model.Width = item.Width;
                    model.MainId = ViewBag.Import;
                    model.CreationTime = DateTime.Now;
                    model.Creator = WorkContext.CurrentUser.Id;
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
        [HttpGet]
        [Route("editce", Name = "editBuyerce")]
        [Function("编辑发运条目", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult EditBuyerce(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itBuyer");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            ViewBag.Itemo = "1234";
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

                //model.Invcurr = model.Invcurr.Trim();
                model.CreationTime = DateTime.Now;
                // model.Shipper = model.Shipper.Trim();
                //  model.Itemno = model.Itemno.Trim();
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

            // if (!String.IsNullOrEmpty(model.InvoiceNo))
            //     model.InvoiceNo = model.InvoiceNo.Trim();
            // if (!String.IsNullOrEmpty(model.MaterielNo))
            //     model.MaterielNo = model.MaterielNo.Trim();
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