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

        private ISysCustomizedListService _sysCustomizedListService;
        public ITProcurementPlanController(ISysCustomizedListService sysCustomizedListService, IProcurementPlanService sysProcurementPlanService, IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
        {
            this._sysCustomizedListService = sysCustomizedListService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysRoleService = sysRoleService;
            this._sysProcurementPlanService = sysProcurementPlanService;
            this._hostingEnvironment = hostingEnvironment;
        }
        [Route("", Name = "itProcurementPlanIndex")]
        [Function("采购计划", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.DataImportController", Sort = 1)]

        public IActionResult ITProcurementPlanIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysProcurementPlanService.searchProcurementPlan(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ProcurementPlan, SysCustomizedListSearchArg>("itProcurementPlanIndex", arg);
            return View(dataSource);//sysImport
        }
        [Route("excelimportPlan", Name = "excelimportPlan")]
        [Function("导出采购计划Excel", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanIndex")]

        public FileResult Excel()
        {
            //获取list数据

            var list = _sysRoleService.getAllRoles();//bll.NurseUserListExcel("", "ID asc");
            //创建Excel文件的对象
            NPOI.HSSF.UserModel.HSSFWorkbook book = new NPOI.HSSF.UserModel.HSSFWorkbook();
            //添加一个sheet
            NPOI.SS.UserModel.ISheet sheet1 = book.CreateSheet("Sheet1");
            //给sheet1添加第一行的头部标题
            NPOI.SS.UserModel.IRow row1 = sheet1.CreateRow(0);
            row1.CreateCell(0).SetCellValue("ID");
            row1.CreateCell(1).SetCellValue("用户姓名");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Id.ToString());
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            string sFileName = $"{DateTime.Now}.xlsx";
            return File(ms, "application/vnd.ms-excel", sFileName);
        }
       
        [HttpPost]
        [Route("importexcelPlan", Name = "importexcelPlan")]
        [Function("Excel导入采购计划", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanIndex")]

        public ActionResult Import(Entities.ProcurementPlan modelplan,IFormFile excelfile,string excelbh, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itProcurementPlanIndex");
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
            string sFileName = excelfile.FileName;

            //string sFileName = $"{Guid.NewGuid()}.xlsx";
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
                int accovers = 0; int purchasing = 0; int application = 0; int remark1 = 0; int remark2 = 0;
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
                    if (worksheet.Cells[1, columns].Value.ToString() == "备注1") { remark1 = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "备注2") { remark2 = columns; }

                }
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        Entities.ProcurementPlan model = new Entities.ProcurementPlan();
                        model.PlanItem = modelplan.PlanItem;
                        model.Prepare = modelplan.Prepare;
                        model.Project = modelplan.Project;
                        model.Item = worksheet.Cells[row, item].Value.ToString();
                        model.Materialno = worksheet.Cells[row, materialno].Value.ToString();
                        model.Name = worksheet.Cells[row, name].Value.ToString();
                        model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                        model.Technical = worksheet.Cells[row, technical].Value.ToString();
                        model.Width = worksheet.Cells[row, width].Value.ToString();
                        model.Size = worksheet.Cells[row, size].Value.ToString();
                        model.Length = worksheet.Cells[row, length].Value.ToString();
                        model.PlanNo = worksheet.Cells[row, planno].Value.ToString();
                        model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();
                        model.PlanOrderNo = worksheet.Cells[row, planorderno].Value.ToString();
                        model.PlanOrderUnit = worksheet.Cells[row, planorderunit].Value.ToString();
                        model.RequiredDockDate =Convert.ToDateTime(worksheet.Cells[row,requireddockdate].Value.ToString()) ;
                        model.ACCovers = worksheet.Cells[row, accovers].Value.ToString();
                        model.Purchasing = worksheet.Cells[row, purchasing].Value.ToString();
                        model.Application = worksheet.Cells[row, application].Value.ToString();
                        if (worksheet.Cells[row, remark1].Value!=null)
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
        [Route("edit", Name = "editITProcurementPlan")]
        [Function("编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITProcurementPlanController.ITProcurementPlanIndex")]
        public IActionResult EditITProcurementPlan(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            if (id != null)
            {
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
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itPorkCustoms");
            if (!ModelState.IsValid)
                return View(model);
            //if (!String.IsNullOrEmpty(model.Invcurr))
            //    model.Invcurr = model.Invcurr.Trim();
            //if (!String.IsNullOrEmpty(model.Shipper))
            //    model.Shipper = model.Shipper.Trim();
            //if (!String.IsNullOrEmpty(model.Itemno))
            //    model.Itemno = model.Itemno.Trim();
            //if (!String.IsNullOrEmpty(model.PoNo))
            //    model.PoNo = model.PoNo.Trim();
            //if (!String.IsNullOrEmpty(model.PoNo))
            //    model.Buyer = model.PoNo.Substring(1, 2);
            if (model.Id.Equals(0))
            {
                //model.CreationTime = DateTime.Now;
                //model.IsDeleted = false;
                //model.Modifier = null;
                //model.ModifiedTime = null;
                //model.Creator = WorkContext.CurrentUser.Id;
                _sysProcurementPlanService.insertProcurementPlan(model);
            }
            else
            {
                //model.Modifier = WorkContext.CurrentUser.Id;
                //model.ModifiedTime = DateTime.Now;
                //_importTrans_main_recordService.updateImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
    }