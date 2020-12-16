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
        public ITBuyerController(ISysUserRoleService sysUserRoleService, IOrderService sysOrderService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
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
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itBuyerSchedule", arg);
            return View(dataSource);//sysImport
        }
        [Route("Order", Name = "itOrderBuyerIndex")]
        [Function("订单导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]

        public IActionResult ITOrderBuyerIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysOrderService.searchOrder(arg, page, size);
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

            string sFileName = "送检单"+ $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));

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
                for (int i = 0; i <= list.Count-1;i++)
                {
                    if (list[i].Buyer.ToString() !=null)
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
                    { worksheet.Cells[i + 2, 9].Value = list[i].Length.ToString(); }
                    if (list[i].Width.ToString() != null)
                    { worksheet.Cells[i + 2, 10].Value = list[i].Width.ToString(); }
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
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [Route("excelimportaej", Name = "excelimportaej")]
        [Function("二检单生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export2()
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var list = _scheduleService.getAll(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "二检单" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder, sFileName));
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
                    { worksheet.Cells[i + 2, 9].Value = list[i].Length.ToString(); }
                    if (list[i].Width.ToString() != null)
                    { worksheet.Cells[i + 2, 10].Value = list[i].Width.ToString(); }
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
            return File(sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        [Route("importexcela2", Name = "importexcela2")]
        public ActionResult Import(IFormFile excelfile, Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("Buyer");
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
                    model.Itemno = worksheet.Cells[row, 1].Value.ToString();
                    model.Shipper = worksheet.Cells[row, 2].Value.ToString();
                    model.PoNo = worksheet.Cells[row, 3].Value.ToString();
                    model.Incoterms = worksheet.Cells[row, 4].Value.ToString();
                    model.CargoType = worksheet.Cells[row, 5].Value.ToString();
                    model.Invamou = worksheet.Cells[row, 6].Value.ToString();
                    model.Invcurr = worksheet.Cells[row, 7].Value.ToString();
                    model.CreationTime = DateTime.Now;
                    model.Creator = WorkContext.CurrentUser.Id;
                    _importTrans_main_recordService.insertImportTransmain(model);
                }
                return Redirect(ViewBag.ReturnUrl);
            }
        }
        [HttpPost]
        [Route("itBuyerList", Name = "itBuyerLista")]
        public ActionResult ITBuyerList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            foreach (Entities.ImportTrans_main_record u in jsonlist)
            {
                var model = _importTrans_main_recordService.getById(u.Id);
                model.RequestedArrivalTime = u.RequestedArrivalTime;
                _importTrans_main_recordService.updateImportTransmain(model);
                //u就是jsonlist里面的一个实体类
            }
            AjaxData.Status = true;
            AjaxData.Message = "OK";
            return Json(AjaxData);
        }
        [HttpPost]
        [Route("itBuyerScheduleList", Name = "itBuyerScheduleList")]
        public ActionResult ITBuyerScheduleList(string kevin)
        {
            string test = kevin;
            List<Entities.Schedule> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Schedule>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            foreach (Entities.Schedule u in jsonlist)
            {
                var model = _scheduleService.getById(u.Id);
                model.Buyer = u.Buyer;
                model.OrderNo = u.OrderNo;
                model.OrderLine = u.OrderLine;
                model.ReferenceNo = u.ReferenceNo;
                model.MaterialCode = u.MaterialCode;
                model.Description = u.Description;
                model.Type = u.Type;
                model.Specification = u.Specification;
                model.Thickness = u.Thickness;
                model.Length = u.Length;
                model.Width = u.Width;
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
            return Json(AjaxData);
        }
        [HttpPost]
        [Route("itOrderBuyerList", Name = "itOrderBuyerList")]
        public ActionResult ITOrderBuyerList(string kevin)
        {
            string test = kevin;
            List<Entities.Order> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Order>(test);
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            foreach (Entities.Order u in jsonlist)
            {
                var item = _sysOrderService.getById(u.Id);
                Entities.Schedule model = new Entities.Schedule();
                model.OrderNo = item.OrderNo;
                model.MainId = ViewBag.Import;
                model.CreationTime = DateTime.Now;
                model.Creator = WorkContext.CurrentUser.Id;
                _scheduleService.insertSchedule(model);
                //u就是jsonlist里面的一个实体类
            }
            AjaxData.Status = true;
            AjaxData.Message = "OK";
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