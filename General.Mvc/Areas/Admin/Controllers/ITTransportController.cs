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
        public IActionResult ITTransportIndex(List<int> sysResource,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("运输方式");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var customizedList2 = _sysCustomizedListService.getByAccount("运输状态");
            ViewData["Status"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _importTrans_main_recordService.searchList(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itTransport", arg);
            return View(dataSource);//sysImport
        }
        [Route("schedule", Name = "itTransportSchedule")]
        [Function("明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        [HttpGet]
        public IActionResult ITTransportScheduleIndex( int id ,SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.Userid = id;
            RolePermissionViewModel model = new RolePermissionViewModel();
             var pageList = _scheduleService.searchList(arg, page, size,id);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itTransportSchedule", arg);
            return View(dataSource);//sysImport
        }
        [Route("excelTransport", Name = "excelTransport")]
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
        [Route("importTransport", Name = "importTransport")]
        public ActionResult Import(IFormFile excelfile, Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itTransport");
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
        [Route("itTransportList", Name = "itTransportList")]
        public ActionResult ITTransportList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            foreach (Entities.ImportTrans_main_record u in jsonlist)
            {
                var model = _importTrans_main_recordService.getById(u.Id);
                
              
                if (u.Status != "") { model.Status = u.Status; }
                model.Ata = u.Ata;
                model.Atd = u.Atd;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            AjaxData.Status = true;
            AjaxData.Message = "OK";
            return Json(AjaxData);
        }
        [HttpGet]
        [Route("edit", Name = "editITTransport")]
        [Function("编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITTransportController.ITTransportIndex")]
        public IActionResult EditITTransport(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itTransport");
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
        public ActionResult EditITTransport(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");
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