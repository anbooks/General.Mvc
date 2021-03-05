using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using General.Services.SysUser;
using General.Services.SysRole;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using OfficeOpenXml;
using System.Text;
using General.Services.ImportTrans_main_recordService;
using General.Services.ProcurementPlan;
using General.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using General.Framework.Datatable;
using General.Services.SysCustomizedList;
using Microsoft.AspNetCore.StaticFiles;
using General.Services.ProcurementPlanMain;

namespace General.Mvc.Areas.Admin.Controllers
{
    //[Area("Admin")]
    [Route("admin/itProcurementPlan")]
    public class ITProcurementPlanController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ISysRoleService _sysRoleService;
        private IProcurementPlanService _sysProcurementPlanService;
        private IProcurementPlanMainService _sysProcurementPlanMainService;
        private ISysCustomizedListService _sysCustomizedListService;
        private ISysUserService _sysUserService;
        public ITProcurementPlanController(ISysUserService sysUserService, ISysCustomizedListService sysCustomizedListService, IProcurementPlanMainService sysProcurementPlanMainService, IProcurementPlanService sysProcurementPlanService, IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
        {
            this._sysCustomizedListService = sysCustomizedListService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysRoleService = sysRoleService;
            this._sysUserService = sysUserService;
            this._sysProcurementPlanService = sysProcurementPlanService;
            this._sysProcurementPlanMainService = sysProcurementPlanMainService;
            this._hostingEnvironment = hostingEnvironment;
        }
        [Route("itProcurementPlanIndex", Name = "itProcurementPlanIndex")]
        [Function("采购计划详细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanMainIndex")]

        public IActionResult ITProcurementPlanIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string s;
            if (id != 0)
            {
                Response.Cookies.Append("Plan", Convert.ToString(id));
                s = Convert.ToString(id);
            }
            else
            {
                Request.Cookies.TryGetValue("Plan", out s);
            }
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var pageList = _sysProcurementPlanService.searchProcurementPlan(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ProcurementPlan, SysCustomizedListSearchArg>("itProcurementPlanIndex", arg);
            return View(dataSource);//sysImport
        }
        [Route("ITProcurementPlanMainDelete", Name = "ITProcurementPlanMainDelete")]
        [Function("采购计划删除", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanMainIndex")]
        public IActionResult ITProcurementPlanMainDelete(int id)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("itProcurementPlanMainIndex");
            var model = _sysProcurementPlanMainService.getById(id);
            model.IsDeleted = true;
            _sysProcurementPlanMainService.updateProcurementPlanMain(model);
            return Redirect(ViewBag.ReturnUrl);

        }
        [Route("", Name = "itProcurementPlanMainIndex")]
        [Function("采购计划", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.DataImportController", Sort = 1)]

        public IActionResult ITProcurementPlanMainIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysProcurementPlanMainService.searchProcurementPlanMain(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ProcurementPlanMain, SysCustomizedListSearchArg>("itProcurementPlanMainIndex", arg);
            return View(dataSource);//sysImport
        }
        //[Route("excelimportPlan", Name = "excelimportPlan")]
        //[Function("导出采购计划Excel", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanMainIndex")]
        //public FileResult Excel()
        //{
        //    //获取list数据

        //    var list = _sysRoleService.getAllRoles();//bll.NurseUserListExcel("", "ID asc");
        //    //创建Excel文件的对象
        //    NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
        //    //添加一个sheet
        //    NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
        //    //给sheet1添加第一行的头部标题
        //    NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
        //    row1.CreateCell(0).SetCellValue("ID");
        //    row1.CreateCell(1).SetCellValue("用户姓名");
        //    //将数据逐步写入sheet1各个行
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
        //        rowtemp.CreateCell(0).SetCellValue(list[i].Id.ToString());
        //        rowtemp.CreateCell(1).SetCellValue(list[i].Name);
        //    }
        //    // 写入到客户端 
        //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
        //    book.Write(ms);
        //    ms.Seek(0, SeekOrigin.Begin);
        //    string sFileName = $"{DateTime.Now}.xlsx";
        //    return File(ms, "application/vnd.ms-excel", sFileName);
        //}
        [HttpPost]
        [Route("importexcelPlan", Name = "importexcelPlan")]
        [Function("Excel导入采购计划", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanMainIndex")]
        public ActionResult Import(Entities.ProcurementPlanMain modelplan, IFormFile excelfile, string excelbh, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itProcurementPlanMainIndex");
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
                int materialno = 0; int partno = 0; int technical = 0; int item = 0; int width = 0; int name = 0; int size = 0;
                int length = 0; int planno = 0; int planunit = 0; int planorderno = 0; int planorderunit = 0; int requireddockdate = 0;
                int accovers = 0; int purchasing = 0; int application = 0; int remark1 = 0; int remark2 = 0; int packagea = 0; int single = 0;
                for (int columns = 1; columns <= ColCount; columns++)
                {
                    //Entities.Order model = new Entities.Order();
                    if (worksheet.Cells[1, columns].Value.ToString() == "序号") { item = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "物料编码") { materialno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "名称") { name = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "牌号") { partno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "技术规范") { technical = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格") { size = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "宽") { width = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "长") { length = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "计划数量") { planno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "计划单位") { planunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购订单数量") { planorderno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购订单单位") { planorderunit = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "要求到货时间") { requireddockdate = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购起止架份") { accovers = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "采购依据及批准人") { purchasing = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "申请号") { application = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "包装规格") { packagea = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "单机定额") { single = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "备注1") { remark1 = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "备注2") { remark2 = columns; }
                }
                Entities.ProcurementPlanMain modelmain = new Entities.ProcurementPlanMain();
                var listmain = _sysProcurementPlanMainService.existAccount(modelplan.PlanItem);
                if (listmain == true)
                {

                    return Redirect(ViewBag.ReturnUrl);
                }

                modelmain.PlanItem = modelplan.PlanItem;
                modelmain.Prepare = modelplan.Prepare;
                modelmain.Project = modelplan.Project;
                _sysProcurementPlanMainService.insertProcurementPlanMain(modelmain);
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        Entities.ProcurementPlan model = new Entities.ProcurementPlan();
                        model.PlanItem = modelplan.PlanItem;
                        model.Prepare = modelplan.Prepare;
                        model.Project = modelplan.Project;
                        var main = _sysProcurementPlanMainService.getByAccount(model.PlanItem);
                        model.MainId = main.Id;
                        if (worksheet.Cells[row, item].Value != null)
                        {
                            model.Item = worksheet.Cells[row, item].Value.ToString();
                        }
                        if (worksheet.Cells[row, materialno].Value != null)
                        {
                            model.Materialno = worksheet.Cells[row, materialno].Value.ToString();
                        }
                        if (worksheet.Cells[row, name].Value != null)
                        {
                            model.Name = worksheet.Cells[row, name].Value.ToString();
                        }
                        if (worksheet.Cells[row, partno].Value != null)
                        {
                            model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                        }
                        if (worksheet.Cells[row, technical].Value != null)
                        {
                            model.Technical = worksheet.Cells[row, technical].Value.ToString();
                        }
                        if (worksheet.Cells[row, width].Value != null)
                        {
                            model.Width = worksheet.Cells[row, width].Value.ToString();
                        }
                        if (worksheet.Cells[row, size].Value != null)
                        {
                            model.Size = worksheet.Cells[row, size].Value.ToString();
                        }
                        if (worksheet.Cells[row, length].Value != null)
                        {
                            model.Length = worksheet.Cells[row, length].Value.ToString();
                        }
                        if (worksheet.Cells[row, planno].Value != null)
                        {
                            model.PlanNo = worksheet.Cells[row, planno].Value.ToString();
                        }
                        if (worksheet.Cells[row, planunit].Value != null)
                        {
                            model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();
                        }
                        if (worksheet.Cells[row, planorderno].Value != null)
                        {
                            model.PlanOrderNo = worksheet.Cells[row, planorderno].Value.ToString();
                        }
                        if (worksheet.Cells[row, planorderunit].Value != null)
                        {
                            model.PlanOrderUnit = worksheet.Cells[row, planorderunit].Value.ToString();
                        }
                        if (worksheet.Cells[row, requireddockdate].Value != null)
                        {
                            model.RequiredDockDate = Convert.ToDateTime(worksheet.Cells[row, requireddockdate].Value.ToString());
                        }
                        if (worksheet.Cells[row, accovers].Value != null)
                        {
                            model.ACCovers = worksheet.Cells[row, accovers].Value.ToString();
                        }
                        if (worksheet.Cells[row, purchasing].Value != null)
                        {
                            model.Purchasing = worksheet.Cells[row, purchasing].Value.ToString();
                        }
                        if (worksheet.Cells[row, application].Value != null)
                        {
                            model.Application = worksheet.Cells[row, application].Value.ToString();
                        }
                        if (worksheet.Cells[row, single].Value != null)
                        {
                            model.SingleQuota = worksheet.Cells[row, single].Value.ToString();
                        }
                        if (worksheet.Cells[row, packagea].Value != null)
                        {
                            model.Package = worksheet.Cells[row, packagea].Value.ToString();
                        }
                        if (worksheet.Cells[row, remark1].Value != null)
                        {
                            model.Remark1 = worksheet.Cells[row, remark1].Value.ToString();
                        }
                        if (worksheet.Cells[row, remark2].Value != null)
                        {
                            model.Remark2 = worksheet.Cells[row, remark2].Value.ToString();
                        }
                        //  model.CreationTime = DateTime.Now;
                        //  model.Creator = WorkContext.CurrentUser.Id;
                        _sysProcurementPlanService.insertProcurementPlan(model);
                    }
                    catch (Exception e)
                    {
                        ViewData["IsShowAlert"] = "True";
                    }
                }
                return Redirect(ViewBag.ReturnUrl);
            }
        }
        [HttpGet]
        [Route("editMain", Name = "editITProcurementPlanMain")]
        [Function("编辑采购计划主表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanMainIndex")]
        public IActionResult EditITProcurementPlanMain(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itProcurementPlanMainIndex");
            var customizedList = _sysUserService.getBuyer();
            ViewData["Creator"] = new SelectList(customizedList, "Name", "Name");
            if (id != null)
            {
                ViewBag.fw = 1;
                var model = _sysProcurementPlanMainService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("editMain")]
        public ActionResult EditITProcurementPlanMain(Entities.ProcurementPlanMain modelplan, IFormFile excelfile, string excelbh, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itProcurementPlanMainIndex");
            if (!ModelState.IsValid)
                return View(modelplan);
            if (modelplan.Id.Equals(0))
            {
                if (excelfile == null)
                {

                    Response.WriteAsync("<script>alert('未添加导入模板!');window.location.href ='editMain'</script>", Encoding.GetEncoding("GB2312"));
                }
                try
                {
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
                        int materialno = 0; int partno = 0; int technical = 0; int item = 0; int width = 0; int name = 0; int size = 0;
                        int length = 0; int planno = 0; int planunit = 0; int planorderno = 0; int planorderunit = 0; int requireddockdate = 0;
                        int accovers = 0; int purchasing = 0; int application = 0; int remark1 = 0; int remark2 = 0; int packagea = 0; int single = 0;
                        for (int columns = 1; columns <= ColCount; columns++)
                        {
                            if (worksheet.Cells[1, columns].Value.ToString() == "序号") { item = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "物料编码") { materialno = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "名称") { name = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "牌号") { partno = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "技术规范") { technical = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "规格") { size = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "宽") { width = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "长") { length = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "计划数量") { planno = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "计划单位") { planunit = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "采购订单数量") { planorderno = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "采购订单单位") { planorderunit = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "要求到货时间") { requireddockdate = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "采购起止架份") { accovers = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "采购依据及批准人") { purchasing = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "申请号") { application = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "包装规格") { packagea = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "单机定额") { single = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "备注1") { remark1 = columns; }
                            if (worksheet.Cells[1, columns].Value.ToString() == "备注2") { remark2 = columns; }
                        }
                        Entities.ProcurementPlanMain modelmain = new Entities.ProcurementPlanMain();
                        var listmain = _sysProcurementPlanMainService.existAccount(modelplan.PlanItem);
                        if (listmain == true)
                        {
                            Response.WriteAsync("<script>alert('采购计划编号重复!');window.location.href ='editMain'</script>", Encoding.GetEncoding("GB2312"));
                            return Redirect(ViewBag.ReturnUrl);
                        }
                        modelmain.PlanItem = modelplan.PlanItem;
                        modelmain.Prepare = modelplan.Prepare;
                        modelmain.Project = modelplan.Project;
                        modelmain.CreationTime = modelplan.CreationTime;
                        modelmain.Creator = modelplan.Creator;
                      
                        var pre = _sysUserService.existName(modelplan.Prepare);
                        var cre = _sysUserService.existName(modelplan.Creator);
                        if (pre == false && modelplan.Prepare!=null)
                        {
                            Response.WriteAsync("<script>alert('采购计划编制人不存在!');window.location.href ='editMain'</script>", Encoding.GetEncoding("GB2312"));
                            return Redirect(ViewBag.ReturnUrl);
                        }
                        if (cre == false)
                        {
                            Response.WriteAsync("<script>alert('采购计划接收人不存在!');window.location.href ='editMain'</script>", Encoding.GetEncoding("GB2312"));
                            return Redirect(ViewBag.ReturnUrl);
                        }
                        _sysProcurementPlanMainService.insertProcurementPlanMain(modelmain);
                        var main = _sysProcurementPlanMainService.getByAccount(modelplan.PlanItem);
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                Entities.ProcurementPlan model = new Entities.ProcurementPlan();
                                model.PlanItem = modelplan.PlanItem;
                                model.Prepare = modelplan.Prepare;
                                model.Project = modelplan.Project;

                                model.MainId = main.Id;
                                if (item > 0 && worksheet.Cells[row, item].Value != null)
                                {
                                    model.Item = worksheet.Cells[row, item].Value.ToString();
                                }
                                if (materialno > 0 && worksheet.Cells[row, materialno].Value != null)
                                {
                                    model.Materialno = worksheet.Cells[row, materialno].Value.ToString();
                                }
                                if (name > 0 && worksheet.Cells[row, name].Value != null)
                                {
                                    model.Name = worksheet.Cells[row, name].Value.ToString();
                                }
                                if (partno > 0 && worksheet.Cells[row, partno].Value != null)
                                {
                                    model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                                }
                                if (technical > 0 && worksheet.Cells[row, technical].Value != null)
                                {
                                    model.Technical = worksheet.Cells[row, technical].Value.ToString();
                                }
                                if (width > 0 && worksheet.Cells[row, width].Value != null)
                                {
                                    model.Width = worksheet.Cells[row, width].Value.ToString();
                                }
                                if (size > 0 && worksheet.Cells[row, size].Value != null)
                                {
                                    model.Size = worksheet.Cells[row, size].Value.ToString();
                                }
                                if (length > 0 && worksheet.Cells[row, length].Value != null)
                                {
                                    model.Length = worksheet.Cells[row, length].Value.ToString();
                                }
                                if (planno > 0 && worksheet.Cells[row, planno].Value != null)
                                {
                                    model.PlanNo = worksheet.Cells[row, planno].Value.ToString();
                                }
                                if (planunit > 0 && worksheet.Cells[row, planunit].Value != null)
                                {
                                    model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();
                                }
                                if (planorderno > 0 && worksheet.Cells[row, planorderno].Value != null)
                                {
                                    model.PlanOrderNo = worksheet.Cells[row, planorderno].Value.ToString();
                                }
                                if (planorderunit > 0 && worksheet.Cells[row, planorderunit].Value != null)
                                {
                                    model.PlanOrderUnit = worksheet.Cells[row, planorderunit].Value.ToString();
                                }
                                if (requireddockdate > 0 && worksheet.Cells[row, requireddockdate].Value != null)
                                {
                                    model.RequiredDockDate = Convert.ToDateTime(worksheet.Cells[row, requireddockdate].Value.ToString());

                                }
                                if (accovers > 0 && worksheet.Cells[row, accovers].Value != null)
                                {
                                    model.ACCovers = worksheet.Cells[row, accovers].Value.ToString();
                                }
                                if (purchasing > 0 && worksheet.Cells[row, purchasing].Value != null)
                                {
                                    model.Purchasing = worksheet.Cells[row, purchasing].Value.ToString();
                                }
                                if (application > 0 && worksheet.Cells[row, application].Value != null)
                                {
                                    model.Application = worksheet.Cells[row, application].Value.ToString();
                                }
                                if (single > 0 && worksheet.Cells[row, single].Value != null)
                                {
                                    model.SingleQuota = worksheet.Cells[row, single].Value.ToString();
                                }
                                if (packagea > 0 && worksheet.Cells[row, packagea].Value != null)
                                {
                                    model.Package = worksheet.Cells[row, packagea].Value.ToString();
                                }
                                if (remark1 > 0 && worksheet.Cells[row, remark1].Value != null)
                                {
                                    model.Remark1 = worksheet.Cells[row, remark1].Value.ToString();
                                }
                                if (remark2 > 0 && worksheet.Cells[row, remark2].Value != null)
                                {
                                    model.Remark2 = worksheet.Cells[row, remark2].Value.ToString();
                                }
                                model.CreationTime = DateTime.Now;
                                model.Creator = WorkContext.CurrentUser.Id;
                                _sysProcurementPlanService.insertProcurementPlan(model);
                            }
                            catch (Exception e)
                            {
                                main.IsDeleted = true;
                                _sysProcurementPlanMainService.updateProcurementPlanMain(main);
                                Response.WriteAsync("<script>alert('要求到货时间格式有误!');window.location.href ='editMain'</script>", Encoding.GetEncoding("GB2312"));
                            }
                        }
                    }
                }
                catch
                {
                    Response.WriteAsync("<script>alert('请将文件名中的空格或特殊字符去掉!');window.location.href ='editMain'</script>", Encoding.GetEncoding("GB2312"));
                }
                }
            else
            {
                var modelmain = _sysProcurementPlanMainService.getById(modelplan.Id);
                modelmain.PlanItem = modelplan.PlanItem;
                modelmain.Prepare = modelplan.Prepare;
                modelmain.Project = modelplan.Project;
                modelmain.Modifier = WorkContext.CurrentUser.Id;
                modelmain.ModifiedTime = DateTime.Now;
                _sysProcurementPlanMainService.updateProcurementPlanMain(modelplan);
            }


            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit", Name = "editITProcurementPlan")]
        [Function("编辑采购计划详细数据", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanMainIndex")]
        public IActionResult EditITProcurementPlan(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itProcurementPlanIndex");
            if (id != null)
            {
                ViewBag.fw = 1;
                var model = _sysProcurementPlanService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditITProcurementPlan(Entities.ProcurementPlan model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itProcurementPlanIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (model.Id.Equals(0))
            {
                //model.CreationTime = DateTime.Now;
                //model.IsDeleted = false;
                //model.Modifier = null;
                //model.ModifiedTime = null;
                //model.Creator = WorkContext.CurrentUser.Id;
                //_sysProcurementPlanService.insertProcurementPlan(model);
            }
            else
            {
                var modela = _sysProcurementPlanService.getById(model.Id);
                modela.Item = model.Item;
                modela.Materialno = model.Materialno;
                modela.Name = model.Name;
                modela.PartNo = model.PartNo;
                modela.Technical = model.Technical;
                modela.Size = model.Size;
                modela.Width = model.Width;
                modela.Length = model.Length;
                modela.SingleQuota = model.SingleQuota;
                modela.Package = model.Package;
                modela.PlanNo = model.PlanNo;
                modela.PlanUnit = model.PlanUnit;
                modela.PlanOrderNo = model.PlanOrderNo;
                modela.PlanOrderUnit = model.PlanOrderUnit;
                modela.RequiredDockDate = model.RequiredDockDate;
                modela.ACCovers = model.ACCovers;
                modela.Purchasing = model.Purchasing;
                modela.Application = model.Application;
                modela.Remark1 = model.Remark1;
                modela.Remark2 = model.Remark2;
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _sysProcurementPlanService.updateProcurementPlan(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}