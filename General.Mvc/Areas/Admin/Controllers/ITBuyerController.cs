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
        public IActionResult ITBuyerIndex(List<int> sysResource,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
        public IActionResult ITBuyerScheduleIndex( int id ,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.QX = WorkContext.CurrentUser.Co;
            int? ida = id;
            if (ida == 0) {
                ViewBag.Import = HttpContext.Session.GetInt32("import");
               
            } else {
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
        [Route("excelimporta2", Name = "excelimporta2")]
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
                model.InvoiceNo= u.InvoiceNo;
                model.MaterielNo = u.MaterielNo;
                model.NetOrder = u.NetOrder;
                model.NetPrice = u.NetPrice;
                model.OrderUnit = u.OrderUnit;
                model.PurchaseOrderQuantity = u.PurchaseOrderQuantity;
                model.PurchasingDocuments = u.PurchasingDocuments;
                model.ShortTxt = u.ShortTxt;
                model.Waybill = u.Waybill;
                model.BatchNo = u.BatchNo;
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
                model.MaterielNo = item.OrderNo;
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
                model.Buyer= model.PoNo.Substring(1, 2);
            // model.PoNo = model.PoNo.Substring(0, 2);
            if (model.Id.Equals(0)) {
               
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

            if (!String.IsNullOrEmpty(model.InvoiceNo))
                model.InvoiceNo = model.InvoiceNo.Trim();
            if (!String.IsNullOrEmpty(model.MaterielNo))
                model.MaterielNo = model.MaterielNo.Trim();
            if (!String.IsNullOrEmpty(model.PurchasingDocuments))
                model.PurchasingDocuments = model.PurchasingDocuments.Trim();
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