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
using General.Services.Inspection;
using General.Services.InspectionRecord;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/inspection")]
    public class InspectionController : AdminPermissionController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        private IInspectionService _sysInspectionService;
        private IInspectionRecordService _sysInspectionRecordService;
        public InspectionController(IInspectionRecordService sysInspectionRecordService, IInspectionService sysInspectionService, ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
            this._sysInspectionService = sysInspectionService;
            this._sysInspectionRecordService = sysInspectionRecordService;
        }
        [Route("inspection", Name = "inspection")]
        [Function("送检审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionService.searchInspection(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionList", Name = "InspectionList")]
        [Function("批量送检审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        public ActionResult InspectionList(string kevin)
        {
            string test = kevin;
            List<Entities.Inspection> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Inspection>(test);
            try
            {
                foreach (Entities.Inspection u in jsonlist)
                {
                    var model = _sysInspectionService.getById(u.Id);
                    model.PlaceQty = u.PlaceQty+model.PlaceQty;
                    model.UnPlaceQty = model.Qty - model.PlaceQty;
                    if (model.UnPlaceQty<0) {
                        AjaxData.Status = false;
                        AjaxData.Message = "OK";
                        return Json(AjaxData);
                    }
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                    InspectionRecord record = new InspectionRecord();
                    record.ContractNo = model.ContractNo;
                    record.Supplier = model.Supplier;
                    record.Manufacturer = model.Manufacturer;
                    record.CofC = model.CofC;
                    record.Description = model.Description;
                    record.MaterialCode = model.MaterialCode;
                    record.Type = model.Type;
                    record.Size = model.Size;
                    record.Batch = model.Batch;
                    record.Batch = model.Batch;
                    record.ReceivedDate = model.ReceivedDate;
                    record.Specification = model.Specification;
                    record.PlaceQty = u.PlaceQty;
                    record.UnPlaceQty = model.UnPlaceQty;
                    record.Qty = model.Qty;
                    record.Creator = WorkContext.CurrentUser.Account;
                    record.CreationTime = DateTime.Now;
                    record.InspectionId = model.Id;
                    record.Status = "计划员审批";
                    _sysInspectionService.updateInspection(model);
                    _sysInspectionRecordService.insertInspectionRecord(record);
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
        [Route("inspectionjhy", Name = "inspectionjhy")]
        [Function("计划员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionjhyIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionRecordService.searchInspectionRecord(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspectionRecord, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionjhyList", Name = "InspectionjhyList")]
        [Function("批量接收审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        public ActionResult InspectionjhyList(string kevin)
        {
            string test = kevin;
            
            List<Entities.InspectionRecord> jsonlist = JsonHelper.DeserializeJsonToList<Entities.InspectionRecord>(test);
            try
            {
                foreach (Entities.InspectionRecord u in jsonlist)
                {
                    var model = _sysInspectionRecordService.getById(u.Id);
                    model.Status = "检验员审批中";
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                    var record = _sysInspectionService.getById(model.InspectionId.Value);
                    record.AcceptQty = model.PlaceQty+ record.AcceptQty;
                    _sysInspectionRecordService.updateInspectionRecord(model);
                    _sysInspectionService.updateInspection(record);
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
        [Route("inspectionjyy", Name = "inspectionjyy")]
        [Function("检验员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionjyyIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionRecordService.searchInspectionjy(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspectionRecord, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionjyyList", Name = "InspectionjyyList")]
        [Function("检验员批量审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjyyIndex")]
        public ActionResult InspectionjyyList(string kevin)
        {
            string test = kevin;

            List<Entities.InspectionRecord> jsonlist = JsonHelper.DeserializeJsonToList<Entities.InspectionRecord>(test);
            try
            {
                foreach (Entities.InspectionRecord u in jsonlist)
                {
                    var model = _sysInspectionRecordService.getById(u.Id);
                    model.Status = "保管员审批中";
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                    _sysInspectionRecordService.updateInspectionRecord(model);
                    
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
        [Route("inspectionbg", Name = "inspectionbg")]
        [Function("保管员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionbgyIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionRecordService.searchInspectionbg(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspectionRecord, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionbgList", Name = "InspectionbgList")]
        [Function("保管员批量审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionbgIndex")]
        public ActionResult InspectionbgList(string kevin)
        {
            string test = kevin;

            List<Entities.InspectionRecord> jsonlist = JsonHelper.DeserializeJsonToList<Entities.InspectionRecord>(test);
            try
            {
                foreach (Entities.InspectionRecord u in jsonlist)
                {
                    var model = _sysInspectionRecordService.getById(u.Id);
                    model.Status = "库房主管审批中";
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                    _sysInspectionRecordService.updateInspectionRecord(model);
                  
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
        [Route("inspectionzg", Name = "inspectionzg")]
        [Function("库房主管审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionzgIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionRecordService.searchInspectionzg(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspectionRecord, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionzgList", Name = "InspectionzgList")]
        [Function("库房主管批量审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjyyIndex")]
        public ActionResult InspectionzgList(string kevin)
        {
            string test = kevin;

            List<Entities.InspectionRecord> jsonlist = JsonHelper.DeserializeJsonToList<Entities.InspectionRecord>(test);
            try
            {
                foreach (Entities.InspectionRecord u in jsonlist)
                {
                    var model = _sysInspectionRecordService.getById(u.Id);
                    model.Status = "审批完成";
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                  
                    _sysInspectionRecordService.updateInspectionRecord(model);
                   
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
        [Route("InspectionjhythList", Name = "InspectionjhythList")]
        [Function("批量退回审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        public ActionResult InspectionjhythList(string kevin)
        {
            string test = kevin;

            List<Entities.InspectionRecord> jsonlist = JsonHelper.DeserializeJsonToList<Entities.InspectionRecord>(test);
            try
            {
                foreach (Entities.InspectionRecord u in jsonlist)
                {
                    var model = _sysInspectionRecordService.getById(u.Id);
                    model.Status = "退回";
                   
                    model.Modifier = WorkContext.CurrentUser.Account;
                    model.ModifiedTime = DateTime.Now;
                    var record = _sysInspectionService.getById(Convert.ToInt32(model.InspectionId));
                    record.PlaceQty = record.PlaceQty - model.PlaceQty;
                    record.UnPlaceQty= record.UnPlaceQty + model.PlaceQty;
                    record.Status = "退回";
                    _sysInspectionRecordService.updateInspectionRecord(model);
                    _sysInspectionService.updateInspection(record);
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
        [Route("inspectionRecord", Name = "inspectionRecord")]
        [Function("器材保管卡片", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionRecordIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionRecordService.searchInspectionEnd(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspectionRecord, SysCustomizedListSearchArg>("inspectionRecord", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionRecordList", Name = "InspectionRecordList")]
        [Function("器材验收卡片导出", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionRecordIndex")]
        public IActionResult InspectionRecordList(List<int> checkboxId)
        {

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "器材验收卡片" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\sjdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                //添加头
                worksheet.Cells[1, 1].Value = "合同号";
                worksheet.Cells[1, 2].Value = "供应商";
                worksheet.Cells[1, 3].Value = "制造商";
                worksheet.Cells[1, 4].Value = "合格证号";
                worksheet.Cells[1, 5].Value = "材料名称";
                worksheet.Cells[1, 6].Value = "物料代码";
                worksheet.Cells[1, 7].Value = "牌号图号";
                worksheet.Cells[1, 8].Value = "规格";
                worksheet.Cells[1, 9].Value = "质量编号";
                worksheet.Cells[1, 10].Value = "入厂日期";
                worksheet.Cells[1, 11].Value = "材料规范";
                worksheet.Cells[1, 12].Value = "接收数量";
                int a = 0;
                foreach (int u in checkboxId)
                {
                    var model = _sysInspectionRecordService.getById(u);

                    if (model.ContractNo != null)
                    {
                        worksheet.Cells[a + 2, 1].Value = model.ContractNo.ToString();
                    }
                    if (model.Supplier != null)
                    {
                        worksheet.Cells[a + 2, 2].Value = model.Supplier.ToString();
                    }
                    if (model.Manufacturer != null)
                    {
                        worksheet.Cells[a + 2, 3].Value = model.Manufacturer.ToString();
                    }
                    if (model.CofC != null)
                    {
                        worksheet.Cells[a + 2, 4].Value = model.CofC.ToString();
                    }
                    if (model.Description != null)
                    {
                        worksheet.Cells[a + 2, 5].Value = model.Description.ToString();
                    }
                    if (model.MaterialCode != null)
                    {
                        worksheet.Cells[a + 2, 6].Value = model.MaterialCode.ToString();
                    }
                    if (model.Type != null)
                    {
                        worksheet.Cells[a + 2, 7].Value = model.Type.ToString();
                    }
                    if (model.Size != null)
                    {
                        worksheet.Cells[a + 2, 8].Value = model.Size.ToString();
                    }
                    if (model.Batch != null)
                    {
                        worksheet.Cells[a + 2, 9].Value = model.Batch.ToString();
                    }
                    if (model.ReceivedDate != null)
                    {
                        worksheet.Cells[a + 2, 10].Value = model.ReceivedDate.ToString();
                    }
                    if (model.Specification != null)
                    {
                        worksheet.Cells[a + 2, 11].Value = model.Specification.ToString();
                    }
                    if (model.AcceptQty != null)
                    {
                        worksheet.Cells[a + 2, 12].Value = model.AcceptQty;
                    }
                    a++;
                }
                package.Save();
            }
            return File("\\Files\\sjdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
    }
}