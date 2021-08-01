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
using General.Services.InspecationMain;
using General.Services.Order;
using General.Services.OrderMain;
using Microsoft.AspNetCore.StaticFiles;
using General.Services.InspectionAttachment;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/inspection")]
    public class InspectionController : AdminPermissionController
    {
        private IOrderService _sysOrderService;
        private IOrderMainService _sysOrderMainService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        private IInspectionAttachmentService _InspectionAttachmentService;
        private ISysUserService _sysUserService;
        private IInspectionService _sysInspectionService;
        private IInspectionRecordService _sysInspectionRecordService;
        private IInspecationMainService _sysInspectionMainService;
        public InspectionController(IInspectionAttachmentService InspectionAttachmentService, ISysUserService sysUserService, IOrderService sysOrderService, IOrderMainService sysOrderMainService, IInspecationMainService sysInspectionMainService, IInspectionRecordService sysInspectionRecordService, IInspectionService sysInspectionService, ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._sysOrderMainService = sysOrderMainService;
            this._InspectionAttachmentService = InspectionAttachmentService;
            this._sysOrderService = sysOrderService;
            this._sysInspectionMainService = sysInspectionMainService;
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._sysUserService = sysUserService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
            this._sysInspectionService = sysInspectionService;
            this._sysInspectionRecordService = sysInspectionRecordService;
        }
        [Route("inspection", Name = "inspection")]
        [Function("采购员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _sysInspectionMainService.searchInspecationMain(arg, page, size, WorkContext.CurrentUser.Name);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspecationMain, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionBuyer", Name = "inspectionBuyer")]
        [Function("创建送检单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 0)]
        [HttpGet]
        public IActionResult InspectionBuyerIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            // var USER = _sysUserRoleService.getById(WorkContext.CurrentUser.Id);
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _importTrans_main_recordService.searchListBuyerSj(arg, page, size, WorkContext.CurrentUser.Name);
 
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("inspectionBuyer", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("excelinspection", Name = "excelinspection")]
        [Function("送检单导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionBuyerIndex")]
        public ActionResult Import(IFormFile excelfile, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("inspectionBuyer");
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
                string date = DateTime.Now.ToString("yyMMdd");

                var serial = _sysInspectionMainService.getByDate(date);
                int ser = serial.Count + 1;
                string inmain = "";
                if (ser < 10)
                {
                    inmain = WorkContext.CurrentUser.Account + date + "0" + Convert.ToString(ser);
                }
                else
                {
                    inmain = WorkContext.CurrentUser.Account + date + Convert.ToString(ser);
                }

                InspecationMain modelm = new InspecationMain();
                modelm.InspecationMainId = inmain;
                modelm.Serial = ser;
                modelm.flag = 0;
                modelm.DateId = date;
                modelm.IsDeleted = false;
                modelm.Creator = WorkContext.CurrentUser.Name;
                ViewBag.Import = HttpContext.Session.GetInt32("import");
                int importid = ViewBag.Import;
                modelm.FytmId = importid;
                _sysInspectionMainService.insertInspecationMain(modelm);
                var main = _sysInspectionMainService.getByAccount(inmain);
                string yundan = "";
                string dingdan = "";
                StringBuilder sb = new StringBuilder();
                ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                int rowCount = worksheet.Dimension.Rows;
                int ColCount = worksheet.Dimension.Columns;
                int order = 0; int zzs = 0; int gys = 0; int clmc = 0; int wldm = 0; int ph = 0; int gg = 0; int zlbh = 0; int rcrq = 0; int clgf = 0; int cgsl = 0;
                int xdsl = 0; int hh = 0; int xm = 0;
                for (int columns = 1; columns <= ColCount; columns++)
                {
                    //Entities.Order model = new Entities.Order();
                    if (worksheet.Cells[1, columns].Value != null)
                    {
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("订单号")) { order = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("供应商")) { gys = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("制造商")) { zzs = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("材料名称")) { clmc = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("物料代码")) { wldm = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("牌号/图号")) { ph = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("规格")) { gg = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("质量编号")) { zlbh = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("入厂日期")) { rcrq = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("材料规范")) { clgf = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("采购数量")) { cgsl = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("行号")) { hh = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("项目")) { xm = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString().Contains("运单号")) { xdsl = columns; }
                    }
                }
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        Entities.Inspection model = new Entities.Inspection();

                        if (order > 0 && worksheet.Cells[row, order].Value != null)
                        {
                            model.ContractNo = worksheet.Cells[row, order].Value.ToString();//供应商
                        }
                        if (gys > 0 && worksheet.Cells[row, gys].Value != null)
                        {
                            model.Supplier = worksheet.Cells[row, gys].Value.ToString();//供应商
                        }
                        if (zzs > 0 && worksheet.Cells[row, zzs].Value != null)
                        {
                            model.Manufacturer = worksheet.Cells[row, zzs].Value.ToString();//供应商
                        }
                        if (clmc > 0 && worksheet.Cells[row, clmc].Value != null)
                        {
                            model.Description = worksheet.Cells[row, clmc].Value.ToString();//供应商
                        }
                        if (wldm > 0 && worksheet.Cells[row, wldm].Value != null)
                        {
                            model.MaterialCode = worksheet.Cells[row, wldm].Value.ToString();//供应商
                        }
                        if (ph > 0 && worksheet.Cells[row, ph].Value != null)
                        {
                            model.Type = worksheet.Cells[row, ph].Value.ToString();//供应商
                        }
                        if (gg > 0 && worksheet.Cells[row, gg].Value != null)
                        {
                            model.Size = worksheet.Cells[row, gg].Value.ToString();//供应商
                        }
                        if (zlbh > 0 && worksheet.Cells[row, zlbh].Value != null)
                        {
                            model.Batch = worksheet.Cells[row, zlbh].Value.ToString();//供应商
                        }
                        if (rcrq > 0 && worksheet.Cells[row, rcrq].Value != null)
                        {
                            model.ReceivedDate = Convert.ToDateTime(worksheet.Cells[row, rcrq].Value.ToString());//供应商
                        }
                        if (clgf > 0 && worksheet.Cells[row, clgf].Value != null)
                        {
                            model.Specification = worksheet.Cells[row, clgf].Value.ToString();//供应商
                        }
                        if (cgsl > 0 && worksheet.Cells[row, cgsl].Value != null)
                        {
                            model.Qty = Convert.ToInt32(worksheet.Cells[row, cgsl].Value.ToString());//供应商
                        }
                        if (hh > 0 && worksheet.Cells[row, hh].Value != null)
                        {
                            model.Item = worksheet.Cells[row, hh].Value.ToString();//供应商
                        }
                        if (xm > 0 && worksheet.Cells[row, xm].Value != null)
                        {
                            model.Project = worksheet.Cells[row, xm].Value.ToString();//供应商
                        }
                        //var supplier = _sysOrderService.getAccount(models.ReferenceNo);
                        ////   model.Supplier = supplier.SupplierName;
                        // model.Manufacturer = supplier.Manufacturer;
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
                        model.Serial = sercoc;
                        model.DateId = date;
                        model.CofC = coc;
                        model.IsDeleted = false;
                        model.Creator = WorkContext.CurrentUser.Name;
                        // model.CreationTime = DateTime.Now;
                        model.MainId = main.Id;
                        
                        if (!yundan.Contains(worksheet.Cells[row, xdsl].Value.ToString()))
                        {
                            yundan = yundan + worksheet.Cells[row, xdsl].Value.ToString() + ";";
                        }
                        if (!dingdan.Contains(model.ContractNo))
                        {
                            dingdan = dingdan + model.ContractNo + ";";
                        }
                        _sysInspectionService.insertInspection(model);
                        //u就是jsonlist里面的一个实体类

                        var mainyd = _sysInspectionMainService.getByAccount(inmain);
                        // mainyd.Waybill = yundan;
                        mainyd.OrderNo = dingdan;
                        mainyd.Waybill = yundan;
                        _sysInspectionMainService.updateInspecationMain(mainyd);
                    }
                    catch (Exception e)
                    {
                        var maind = _sysInspectionMainService.getByAccount(inmain);
                        maind.IsDeleted = true;
                        _sysInspectionMainService.updateInspecationMain(maind);
                    }
                }
                return Redirect(ViewBag.ReturnUrl);
            }
        }
        [HttpPost]
        [Route("inspectionBuyerschedule", Name = "Inspectionspsc")]
        [Function("送检审批生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionBuyerIndex")]
        public IActionResult InspectionSC(List<int> checkboxId)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionsch");
            string date = DateTime.Now.ToString("yyMMdd");

            var serial = _sysInspectionMainService.getByDate(date);
            int ser = serial.Count + 1;
            string inmain = "";
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                if (ser < 10)
                {
                    inmain = WorkContext.CurrentUser.Account + date + "0" + Convert.ToString(ser);
                }
                else
                {
                    inmain = WorkContext.CurrentUser.Account + date + Convert.ToString(ser);
                }

                InspecationMain modelm = new InspecationMain();
                modelm.InspecationMainId = inmain;
                modelm.Serial = ser;
                modelm.flag = 0;
                modelm.DateId = date;
                modelm.Creator = WorkContext.CurrentUser.Name;
                ViewBag.Import = HttpContext.Session.GetInt32("import");
                modelm.IsDeleted = false;
                int importid = ViewBag.Import;
             modelm.FytmId = importid;
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
                    if (models.Consignor==null|| models.Consignor =="")
                    {
                        var item = _importTrans_main_recordService.getById(models.MainId);
                        model.Supplier = item.Shipper;
                    }
                    else
                    {
                        model.Supplier = models.Consignor;
                    }
                    
                    model.Manufacturer = models.Manufacturers;
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
                    model.Serial = sercoc;
                    model.DateId = date;
                    model.CofC = coc;
                    model.Description = models.Description;
                    model.MaterialCode = models.MaterialCode;
                    model.Type = models.PartNo;
                    model.Size = supplier.Size;
                    model.Item = models.OrderLine;
                    model.Project = supplier.Main.Project;
                    model.Specification = models.Specification;
                    model.Batch = models.BatchNo;
                    var imp = _importTrans_main_recordService.getById(models.MainId);
                    if (imp != null && imp.ActualDeliveryDate != null)
                    {
                        model.ReceivedDate = imp.ActualDeliveryDate;
                    }

                    model.IsDeleted = false;
                    if (models.PurchaseQuantity != "" && models.PurchaseQuantity != null) { model.Qty = Convert.ToDouble(models.PurchaseQuantity); }

                    model.Creator = WorkContext.CurrentUser.Name;
                    //  model.CreationTime = DateTime.Now;
                    model.MainId = main.Id;
                    if (models.Waybill!=null)
                    {
                        if (!yundan.Contains(models.Waybill))
                        {
                            yundan = yundan + models.Waybill + ";";
                        }

                    }
                    if (models.OrderNo != null)
                    {
                        if (!dingdan.Contains(models.OrderNo))
                        {
                            dingdan = dingdan + models.OrderNo + ";";
                        }
                    }
                    _sysInspectionService.insertInspection(model);
                    models.Sjflag =true;
                    _scheduleService.updateSchedule(models);
                    //u就是jsonlist里面的一个实体类
                }
                var mainyd = _sysInspectionMainService.getByAccount(inmain);
                mainyd.Waybill = yundan;
                mainyd.OrderNo = dingdan;
                _sysInspectionMainService.updateInspecationMain(mainyd);
                

                Response.Cookies.Append("Inspection", Convert.ToString(main.Id));

            }
            catch
            {
                var main = _sysInspectionMainService.getByAccount(inmain);
                main.IsDeleted = true;
                _sysInspectionMainService.updateInspecationMain(main);
                Response.WriteAsync("<script>alert('送检单生成失败!');window.location.href ='inspectionBuyerschedule'</script>", Encoding.GetEncoding("GB2312"));
            }
            return Redirect(ViewBag.ReturnUrl);

        }
        [Route("inspectionBuyerschedule", Name = "inspectionBuyerschedule")]
        [Function("明细表查看", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionBuyerIndex")]
        [HttpGet]
        public IActionResult InspectionBuyerScheduleIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("inspectionBuyerschedule", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionsch", Name = "inspectionsch")]
        [Function("采购员审批明细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        [HttpGet]
        public IActionResult InspectionSchIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var customizedList2 = _sysUserService.getJhy();
            ViewData["Jhy"] = new SelectList(customizedList2, "Name", "Name");
            string s;
            if (id != 0)
            {
                Response.Cookies.Append("Inspection", Convert.ToString(id));
                s = Convert.ToString(id);
            }
            else
            {
                Request.Cookies.TryGetValue("Inspection", out s);
            }
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = ida;
            var pageList = _sysInspectionService.searchInspection(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionscha", Name = "inspectionscha")]
        [Function("查看送检单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        [HttpGet]
        public IActionResult InspectionSchAIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string s;
            if (id != 0)
            {
                Response.Cookies.Append("Inspection", Convert.ToString(id));
                s = Convert.ToString(id);
            }
            else
            {
                Request.Cookies.TryGetValue("Inspection", out s);
            }
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionService.searchInspection(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionschb", Name = "inspectionschb")]
        [Function("查看送检单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        [HttpGet]
        public IActionResult InspectionSchBIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string s;
            if (id != 0)
            {
                Response.Cookies.Append("Inspection", Convert.ToString(id));
                s = Convert.ToString(id);
            }
            else
            {
                Request.Cookies.TryGetValue("Inspection", out s);
            }
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionService.searchInspection(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionxgList", Name = "InspectionxgList")]
        [Function("送检单修改", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
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
                    model.ReceivedDate = u.ReceivedDate;
                    model.Specification = u.Specification;
                    model.Remark = u.Remark;
                    model.UnPlaceQty = u.UnPlaceQty;
                    model.Qty = u.Qty;
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
        [HttpPost]
        [Route("InspectionjhList", Name = "InspectionjhList")]
        [Function("计划员送检单修改", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        public ActionResult InspectionJhList(string kevin)
        {
            string test = kevin;
            List<Entities.Inspection> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Inspection>(test);
            try
            {
                foreach (Entities.Inspection u in jsonlist)
                {
                    var model = _sysInspectionService.getById(u.Id);

                    model.Manufacturer = u.Manufacturer;

                    model.Batch = u.Batch;

                    model.Remark = u.Remark;
                    model.Qty = u.Qty;

                    //model.Status = "计划员审批";

                    _sysInspectionService.updateInspection(model);
                }
                AjaxData.Status = true;
                AjaxData.Message = "OK";
            }
            catch
            {
                AjaxData.Status = true;
                AjaxData.Message = "OK";
            }
            return Json(AjaxData);
        }
        [Route("inspectionschupdate", Name = "inspectionschupdate")]
        [Function("采购员审批复制", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        [HttpGet]
        public IActionResult InspectionSchupdate(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionsch");
            var model = _sysInspectionService.getById(id);
            Inspection record = new Inspection();
            record.MainId = model.MainId;
            record.Manufacturer = model.Manufacturer;
            record.MaterialCode = model.MaterialCode;
            record.Remark = model.Remark;

            record.Qty = model.Qty;
            record.ReceivedDate = model.ReceivedDate;
            record.Size = model.Size;
            record.Specification = model.Specification;
            record.Status = model.Status;
            record.Supplier = model.Supplier;
            record.Type = model.Type;
            record.UnPlaceQty = model.UnPlaceQty;
            record.AcceptQty = model.AcceptQty;
            record.Batch = model.Batch; string date = DateTime.Now.ToString("yyMMdd");
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
            record.IsDeleted = false;
            record.ContractNo = model.ContractNo;
            record.Item = model.Item;
            record.Project = model.Project;
            record.Description = model.Description;
            record.Creator = WorkContext.CurrentUser.Name;
            //record.CreationTime = DateTime.Now;
            _sysInspectionService.insertInspection(record);
            return Redirect(ViewBag.ReturnUrl);
        }
        [Route("inspectionjhyschupdate", Name = "inspectionjhyschupdate")]
        [Function("计划员审批复制", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        [HttpGet]
        public IActionResult InspectionJhySchupdate(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionjhysch");
            var model = _sysInspectionService.getById(id);
            Inspection record = new Inspection();
            record.MainId = model.MainId;
            record.Manufacturer = model.Manufacturer;
            record.MaterialCode = model.MaterialCode;
            record.Remark = model.Remark;
            record.Qty = model.Qty;
            record.ReceivedDate = model.ReceivedDate;
            record.Size = model.Size;
            record.Specification = model.Specification;
            record.Status = model.Status;
            record.Supplier = model.Supplier;
            record.Type = model.Type;
            record.UnPlaceQty = model.UnPlaceQty;
            record.AcceptQty = model.AcceptQty;
            record.Batch = model.Batch; string date = DateTime.Now.ToString("yyMMdd");
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
            record.IsDeleted = false;
            record.Project = model.Project;
            record.Description = model.Description;
            record.Creator = WorkContext.CurrentUser.Name;
            //  record.CreationTime = DateTime.Now;
            _sysInspectionService.insertInspection(record);
            return Redirect(ViewBag.ReturnUrl);
        }
        [Route("InspectionspList", Name = "InspectionspList")]
        [Function("送检单提交", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        public IActionResult InspectionTList(string Jhy, string kevin)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspection");
            InspectionList(kevin);
            string s;
            Request.Cookies.TryGetValue("Inspection", out s);
            int ida = Convert.ToInt32(s);

            if (Jhy == null)
            {
                Response.WriteAsync("<script>alert('计划员不能为空!');window.location.href ='inspectionsch'</script>", Encoding.GetEncoding("GB2312"));
            }
            try
            {
                var model = _sysInspectionMainService.getById(ida);
                model.Status = "计划员";
                model.flag = 2;
                model.CreationTime = DateTime.Now;
                model.JhyName = Jhy;
                _sysInspectionMainService.updateInspecationMain(model);
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
        [Route("inspectiondelet", Name = "inspectiondelet")]
        [Function("送检单删除", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        [HttpGet]
        public IActionResult inspectiondelet(int id)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspection");

            try
            {
                var model = _sysInspectionMainService.getById(id);
                model.IsDeleted = true;
                //model.JhyName = Jhy;
                _sysInspectionMainService.deleteInspecationMain(model);
                //var models = _sysInspectionService.getByMain(id);
                //if (model.FytmId != 0)
                //{

                //}
                //var modela = _scheduleService.getAll(model.FytmId);
                //foreach (var u in models)
                //{
                //    u.IsDeleted = true;
                //}
            }
            catch
            {
                return Redirect(ViewBag.ReturnUrl);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [Route("inspectionjhy", Name = "inspectionjhy")]
        [Function("计划员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionjhyIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _sysInspectionMainService.searchInspecationMainjh(arg, page, size, WorkContext.CurrentUser.Name);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspecationMain, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionjhysch", Name = "inspectionjhysch")]
        [Function("计划员审批明细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        [HttpGet]
        public IActionResult InspectionjhySchIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string s;
            if (id != 0)
            {
                Response.Cookies.Append("Inspection", Convert.ToString(id));
                s = Convert.ToString(id);
            }
            else
            {
                Request.Cookies.TryGetValue("Inspection", out s);
            }
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _sysInspectionService.searchInspection(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [HttpGet]
        [Route("InspectionjhyList", Name = "InspectionjhyList")]
        [Function("接收审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        public IActionResult InspectionjhyList(string kevin)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionjhy");
            string s;
            int ida;
            InspectionJhList(kevin);
            Request.Cookies.TryGetValue("Inspection", out s);
            ida = Convert.ToInt32(s);

            try
            {
                var record = _sysInspectionMainService.getById(ida);
                record.flag = 4;
                record.JhTime = DateTime.Now;
                _sysInspectionMainService.updateInspecationMain(record);

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
        [Function("退回审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionjhyIndex")]
        public IActionResult InspectionjhythList(string remark)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionjhy");
            string s;

            Request.Cookies.TryGetValue("Inspection", out s);

            int ida = Convert.ToInt32(s);

            try
            {
                var record = _sysInspectionMainService.getById(ida);
                record.flag = 1;
                record.Remark = remark;
                _sysInspectionMainService.updateInspecationMain(record);
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
        [Route("InspectionSjdList", Name = "InspectionSjdList")]
        [Function("送检单导出", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        public IActionResult InspectionSjdList(int id)
        {

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "送检单" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\sjdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                //添加头
                worksheet.Cells[1, 1].Value = "送检单号";
                worksheet.Cells[1, 2].Value = "订单号";
                worksheet.Cells[1, 3].Value = "订单行号";
                worksheet.Cells[1, 4].Value = "供应商";
                worksheet.Cells[1, 5].Value = "制造商";
                worksheet.Cells[1, 6].Value = "合格证号";
                worksheet.Cells[1, 7].Value = "材料名称";
                worksheet.Cells[1, 8].Value = "物料代码";
                worksheet.Cells[1, 9].Value = "牌号图号";
                worksheet.Cells[1, 10].Value = "规格";
                worksheet.Cells[1, 11].Value = "质量编号";
                worksheet.Cells[1, 12].Value = "入厂日期";
                worksheet.Cells[1, 13].Value = "材料规范";
                worksheet.Cells[1, 14].Value = "采购数量";
                worksheet.Cells[1, 15].Value = "实收数量";
                worksheet.Cells[1, 16].Value = "实收时间";
                worksheet.Cells[1, 17].Value = "保管员";
                int a = 0;
                var list = _sysInspectionService.getByMain(id);
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    if (list[i].Main.InspecationMainId != null)
                    {
                        worksheet.Cells[a + 2, 1].Value = list[i].Main.InspecationMainId.ToString();
                    }
                    if (list[i].ContractNo != null)
                    {
                        worksheet.Cells[a + 2, 2].Value = list[i].ContractNo.ToString();
                    }
                    if (list[i].Item != null)
                    {
                        worksheet.Cells[a + 2, 3].Value = list[i].Item.ToString();
                    }
                    if (list[i].Supplier != null)
                    {
                        worksheet.Cells[a + 2, 4].Value = list[i].Supplier.ToString();
                    }
                    if (list[i].Manufacturer != null)
                    {
                        worksheet.Cells[a + 2, 5].Value = list[i].Manufacturer.ToString();
                    }
                    if (list[i].CofC != null)
                    {
                        worksheet.Cells[a + 2, 6].Value = list[i].CofC.ToString();
                    }
                    if (list[i].Description != null)
                    {
                        worksheet.Cells[a + 2, 7].Value = list[i].Description.ToString();
                    }
                    if (list[i].MaterialCode != null)
                    {
                        worksheet.Cells[a + 2, 8].Value = list[i].MaterialCode.ToString();
                    }
                    if (list[i].Type != null)
                    {
                        worksheet.Cells[a + 2, 9].Value = list[i].Type.ToString();
                    }
                    if (list[i].Size != null)
                    {
                        worksheet.Cells[a + 2, 10].Value = list[i].Size.ToString();
                    }
                    if (list[i].Batch != null)
                    {
                        worksheet.Cells[a + 2, 11].Value = list[i].Batch.ToString();
                    }
                    if (list[i].ReceivedDate != null)
                    {
                        worksheet.Cells[a + 2, 12].Value = list[i].ReceivedDate.Value.ToString("yyyy-MM-dd");
                    }
                    if (list[i].Specification != null)
                    {
                        worksheet.Cells[a + 2, 13].Value = list[i].Specification.ToString();
                    }
                    if (list[i].Qty != null)
                    {
                        worksheet.Cells[a + 2, 14].Value = list[i].Qty;
                    }
                    if (list[i].AcceptQty != null)
                    {
                        worksheet.Cells[a + 2, 14].Value = list[i].AcceptQty;
                    }
                    if (list[i].AcceptTime != null)
                    {
                        worksheet.Cells[a + 2, 14].Value = list[i].AcceptTime.Value.ToString("yyyy-MM-dd");
                    }
                    if (list[i].Keeper != null)
                    {
                        worksheet.Cells[a + 2, 14].Value = list[i].Keeper;
                    }
                    a++;
                }
                package.Save();
            }
            return File("\\Files\\sjdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [Route("ZbdAttachment", Name = "ZbdAttachment")]
        [Function("质保单查看", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        [HttpGet]
        public IActionResult InspectionAttachmentIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string s = "";
            int ida = 0;
            if (id != 0)
            {
                ida = id;
            }
            else
            {
                Request.Cookies.TryGetValue("Inspection", out s);
                ida = Convert.ToInt32(s);
            }
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _InspectionAttachmentService.searchInspectionAttachment(arg, page, size, id);
            ViewBag.Arg = arg;//传参数ITTransportAttachmentIndex
            var dataSource = pageList.toDataSourceResult<Entities.InspectionAttachment, SysCustomizedListSearchArg>("ZbdAttachment", arg);
            return View(dataSource);//sysImport
        }
        [Route("downLoadsjdfile", Name = "downLoadsjdfile")]
        [Function("下载附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        public IActionResult Download(int? id)
        {
            string load = "";

            var model = _InspectionAttachmentService.getById(id.Value);

            load = "\\Files\\zbd\\";

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
        [HttpGet]
        [Route("edit", Name = "editInspectionAttachment")]
        [Function("质保单编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionController.InspectionIndex")]
        public IActionResult EditInspection(string kevin)
        {

           // InspectionList(kevin);
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditInspection(List<IFormFile> files, string returnUrl = null)
        {

            string s = "";
            int ida = 0;
            Request.Cookies.TryGetValue("Inspection", out s);
            ida = Convert.ToInt32(s);

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("inspectionsch");

            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            foreach (var fpfile in files)
            {
                if (fpfile != null)
                {
                    var fileProfilef = sWebRootFolder + "\\Files\\zbd\\";
                    string f = Guid.NewGuid().ToString("N");
                    string sFileNamef = f + fpfile.FileName;
                    FileInfo filef = new FileInfo(Path.Combine(fileProfilef, sFileNamef));
                    using (FileStream fsf = new FileStream(filef.ToString(), FileMode.Create))
                    {
                        fpfile.CopyTo(fsf);
                        fsf.Flush();
                    }
                    InspectionAttachment attachmentf = new InspectionAttachment();
                    attachmentf.AttachmentLoad = sFileNamef;
                    attachmentf.Name = fpfile.FileName;
                    attachmentf.Type = "质保单";
                    attachmentf.ImportId = ida;
                    attachmentf.Creator = WorkContext.CurrentUser.Account;
                    attachmentf.CreationTime = DateTime.Now;

                    _InspectionAttachmentService.insertInspectionAttachment(attachmentf);
                }
            }
            return Redirect(ViewBag.ReturnUrl);
        }

    }
}