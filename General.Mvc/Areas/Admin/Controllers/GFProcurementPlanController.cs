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
using General.Services.ScheduleService;
using General.Services.Order;
using General.Services.OrderMain;
using General.Core.Librs;

namespace General.Mvc.Areas.Admin.Controllers
{
    //[Area("Admin")]
    [Route("admin/gfProcurementPlan")]
    public class GFProcurementPlanController : AdminPermissionController
    {
        private IOrderService _sysOrderService;
        private IOrderMainService _sysOrderMainService;
        private IScheduleService _scheduleService;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ISysRoleService _sysRoleService;
        private IProcurementPlanService _sysProcurementPlanService;
        private IProcurementPlanMainService _sysProcurementPlanMainService;
        private ISysCustomizedListService _sysCustomizedListService;
        private ISysUserService _sysUserService;
        public GFProcurementPlanController(IOrderMainService sysOrderMainService,IOrderService sysOrderService,IScheduleService scheduleService, ISysUserService sysUserService, ISysCustomizedListService sysCustomizedListService, IProcurementPlanMainService sysProcurementPlanMainService, IProcurementPlanService sysProcurementPlanService, IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
        {
            this._sysCustomizedListService = sysCustomizedListService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._scheduleService = scheduleService;
            this._sysOrderMainService = sysOrderMainService;
            this._sysOrderService = sysOrderService;
            this._sysRoleService = sysRoleService;
            this._sysUserService = sysUserService;
            this._sysProcurementPlanService = sysProcurementPlanService;
            this._sysProcurementPlanMainService = sysProcurementPlanMainService;
            this._hostingEnvironment = hostingEnvironment;
        }
        [Route("gfProcurementPlanIndex", Name = "gfProcurementPlanIndex")]
        [Function("采购计划详细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.GFProcurementPlanController.GFProcurementPlanMainIndex")]

        public IActionResult GFProcurementPlanIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            var dataSource = pageList.toDataSourceResult<Entities.ProcurementPlan, SysCustomizedListSearchArg>("gfProcurementPlanIndex", arg);
            return View(dataSource);//sysImport
        }

        [Route("gfProcurementPlanMainIndex", Name = "gfProcurementPlanMainIndex")]
        [Function("采购计划", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.GeneratedFormController", Sort = 1)]

        public IActionResult GFProcurementPlanMainIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysProcurementPlanMainService.searchProcurementPlanMain(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ProcurementPlanMain, SysCustomizedListSearchArg>("gfProcurementPlanMainIndex", arg);
            return View(dataSource);//sysImport
        }

        [Route("excelimportgf", Name = "excelimportgf")]
        [Function("采购计划执行状态跟踪表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.GFProcurementPlanController.GFProcurementPlanMainIndex")]
        public IActionResult Export2(int id)
        {
            var list = _sysProcurementPlanService.getAll(id);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "采购计划执行状态跟踪表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("采购计划执行状态跟踪表");
                //添加头
                worksheet.Cells[1, 1].Value = "计划检索号";
                worksheet.Cells[1, 2].Value = "序号";
                worksheet.Cells[1, 3].Value = "物料编码";
                worksheet.Cells[1, 4].Value = "材料名称";
                worksheet.Cells[1, 5].Value = "材料牌号/件号";
                worksheet.Cells[1, 6].Value = "规范";
                worksheet.Cells[1, 7].Value = "规格1";
                worksheet.Cells[1, 8].Value = "规格2";
                worksheet.Cells[1, 9].Value = "规格3";
                worksheet.Cells[1, 10].Value = "单机定额";
                worksheet.Cells[1, 11].Value = "要求采购数量";
                worksheet.Cells[1, 12].Value = "采购单位";
                worksheet.Cells[1, 13].Value = "采购起止架份";
                worksheet.Cells[1, 14].Value = "要求到货时间";
                worksheet.Cells[1, 15].Value = "采购依据";
                worksheet.Cells[1, 16].Value = "采购状态描述";
                worksheet.Cells[1, 17].Value = "数值";

                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 27; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }
                //添加值
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    for (int a = i*5; a <= i*5 + 4; a=a+5)
                    {
                        if (list[i].PlanMain.PlanItem != null)
                        {
                            worksheet.Cells[a + 2, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 3, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 4, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 5, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 6, 1].Value = list[i].PlanMain.PlanItem.ToString();
                        }
                        if (list[i].Item != null)
                        {
                            worksheet.Cells[a + 2, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 3, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 4, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 5, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 6, 2].Value = list[i].Item.ToString();
                        }
                        if (list[i].Materialno != null)
                        {
                            worksheet.Cells[a + 2, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 3, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 4, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 5, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 6, 3].Value = list[i].Materialno.ToString();
                        }
                        if (list[i].Name != null)
                        {
                            worksheet.Cells[a + 2, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 3, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 4, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 5, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 6, 4].Value = list[i].Name.ToString();
                        }
                        if (list[i].PartNo != null)
                        {
                            worksheet.Cells[a + 2, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 3, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 4, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 5, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 6, 5].Value = list[i].PartNo.ToString();
                        }
                        if (list[i].Technical != null)
                        {
                            worksheet.Cells[a + 2, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 3, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 4, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 5, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 6, 6].Value = list[i].Technical.ToString();
                        }
                        if (list[i].Size != null)
                        {
                            worksheet.Cells[a + 2, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 3, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 4, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 5, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 6, 7].Value = list[i].Size.ToString();
                        }
                        if (list[i].Width != null)
                        {
                            worksheet.Cells[a + 2, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 3, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 4, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 5, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 6, 8].Value = list[i].Width.ToString();
                        }
                        if (list[i].Length != null)
                        {
                            worksheet.Cells[a + 2, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 3, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 4, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 5, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 6, 9].Value = list[i].Length.ToString();
                        }
                        if (list[i].SingleQuota != null)
                        {
                            worksheet.Cells[a + 2, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 3, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 4, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 5, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 6, 10].Value = list[i].SingleQuota.ToString();
                        }
                        if (list[i].PlanOrderNo != null)
                        {
                            worksheet.Cells[a + 2, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 3, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 4, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 5, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 6, 11].Value = list[i].PlanOrderNo.ToString();
                        }
                        if (list[i].PlanOrderUnit != null)
                        {
                            worksheet.Cells[a + 2, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 3, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 4, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 5, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 6, 12].Value = list[i].PlanOrderUnit.ToString();
                        }
                        if (list[i].ACCovers != null)
                        {
                            worksheet.Cells[a + 2, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 3, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 4, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 5, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 6, 13].Value = list[i].ACCovers.ToString();
                        }
                        if (list[i].RequiredDockDate != null)
                        {
                            worksheet.Cells[a + 2, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 3, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 4, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 5, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 6, 14].Value = list[i].RequiredDockDate.ToString();
                        }
                        if (list[i].Purchasing != null)
                        {
                            worksheet.Cells[a + 2, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 3, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 4, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 5, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 6, 15].Value = list[i].Purchasing.ToString();
                        }
                        var num = _scheduleService.getItem(list[i].Item);
                        var numorder = _sysOrderService.getPlan(list[i].Item);
                        double countx = 0;
                        double countb = 0;
                        double countf = 0;
                        double countz = 0;
                        double countd = 0;
                        //CheckAndPass
                        for (int no = 0; no <= numorder.Count - 1; no++)
                        {
                            if (numorder[no].Qty != null && numorder[no].Qty != "")
                            {
                                if (numorder[no].Reduced > 0 && numorder[no].Reduced != null)
                                {
                                    countx = countx + Convert.ToDouble(numorder[no].Reduced) * Convert.ToDouble(numorder[no].Qty.ToString());
                                }
                                else
                                {
                                    countx = countx + Convert.ToDouble(numorder[no].Qty.ToString());
                                }

                            }
                        }
                        for (int n = 0; n <= num.Count - 1; n++)
                        {
                            if (num[n].ReducedNo != null && num[n].ReducedNo != "")
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass!="是"&&main.Mbl==""||main.Mbl==null)
                                {
                                    countf = countf + Convert.ToDouble(num[n].ReducedNo.ToString());
                                }
                            }
                            if (num[n].ReducedNo != null && num[n].ReducedNo != "")
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass != "是" && main.Mbl != "" && main.Mbl != null)
                                {
                                    countz = countz + Convert.ToDouble(num[n].ReducedNo.ToString());
                                }
                                
                            }
                            if (num[n].ReducedNo != null && num[n].ReducedNo != "")
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass == "是" )
                                {
                                    countd = countd + Convert.ToDouble(num[n].ReducedNo.ToString());
                                }
                              
                            }

                        }
                        worksheet.Cells[a + 2, 16].Value = "询价中";
                        worksheet.Cells[a + 3, 16].Value = "供应商备货";
                        worksheet.Cells[a + 4, 16].Value = "已发货待运输";
                        worksheet.Cells[a + 5, 16].Value = "在途运输";
                        worksheet.Cells[a + 6, 16].Value = "已到货";

                        worksheet.Cells[a + 2, 17].Value = Convert.ToDouble(list[i].PlanNo) - countx;
                        worksheet.Cells[a + 3, 17].Value = countx - countf - countz - countd;
                        worksheet.Cells[a + 4, 17].Value = countf;
                        worksheet.Cells[a + 5, 17].Value = countz;
                        worksheet.Cells[a + 6, 17].Value = countd;
                    }
                }
                package.Save();
            }
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [Route("GfPlanText", Name = "GfPlanText")]
        [Function("采购计划执行状态跟踪表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.GFProcurementPlanController.GFProcurementPlanMainIndex")]
        public IActionResult GfPlan(string item)
        {
            
            var list = _sysProcurementPlanService.getItem(item);
            if (list.Count == 0)
            {
                Response.WriteAsync("<script>alert('当前索引号未录入采购计划!');window.location.href ='gfProcurementPlanMainIndex'</script>", Encoding.GetEncoding("GB2312"));
            }
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "采购计划执行状态跟踪表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("采购计划执行状态跟踪表");
                //添加头
                worksheet.Cells[1, 1].Value = "计划检索号";
                worksheet.Cells[1, 2].Value = "序号";
                worksheet.Cells[1, 3].Value = "物料编码";
                worksheet.Cells[1, 4].Value = "材料名称";
                worksheet.Cells[1, 5].Value = "材料牌号/件号";
                worksheet.Cells[1, 6].Value = "规范";
                worksheet.Cells[1, 7].Value = "规格1";
                worksheet.Cells[1, 8].Value = "规格2";
                worksheet.Cells[1, 9].Value = "规格3";
                worksheet.Cells[1, 10].Value = "单机定额";
                worksheet.Cells[1, 11].Value = "要求采购数量";
                worksheet.Cells[1, 12].Value = "采购单位";
                worksheet.Cells[1, 13].Value = "采购起止架份";
                worksheet.Cells[1, 14].Value = "要求到货时间";
                worksheet.Cells[1, 15].Value = "采购依据";
                worksheet.Cells[1, 16].Value = "采购状态描述";
                worksheet.Cells[1, 17].Value = "数值";

                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 27; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }
                //添加值
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    for (int a = i * 5; a <= i * 5 + 4; a = a + 5)
                    {
                        if (list[i].PlanMain.PlanItem != null)
                        {
                            worksheet.Cells[a + 2, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 3, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 4, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 5, 1].Value = list[i].PlanMain.PlanItem.ToString();
                            worksheet.Cells[a + 6, 1].Value = list[i].PlanMain.PlanItem.ToString();
                        }
                        if (list[i].Item != null)
                        {
                            worksheet.Cells[a + 2, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 3, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 4, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 5, 2].Value = list[i].Item.ToString();
                            worksheet.Cells[a + 6, 2].Value = list[i].Item.ToString();
                        }
                        if (list[i].Materialno != null)
                        {
                            worksheet.Cells[a + 2, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 3, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 4, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 5, 3].Value = list[i].Materialno.ToString();
                            worksheet.Cells[a + 6, 3].Value = list[i].Materialno.ToString();
                        }
                        if (list[i].Name != null)
                        {
                            worksheet.Cells[a + 2, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 3, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 4, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 5, 4].Value = list[i].Name.ToString();
                            worksheet.Cells[a + 6, 4].Value = list[i].Name.ToString();
                        }
                        if (list[i].PartNo != null)
                        {
                            worksheet.Cells[a + 2, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 3, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 4, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 5, 5].Value = list[i].PartNo.ToString();
                            worksheet.Cells[a + 6, 5].Value = list[i].PartNo.ToString();
                        }
                        if (list[i].Technical != null)
                        {
                            worksheet.Cells[a + 2, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 3, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 4, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 5, 6].Value = list[i].Technical.ToString();
                            worksheet.Cells[a + 6, 6].Value = list[i].Technical.ToString();
                        }
                        if (list[i].Size != null)
                        {
                            worksheet.Cells[a + 2, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 3, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 4, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 5, 7].Value = list[i].Size.ToString();
                            worksheet.Cells[a + 6, 7].Value = list[i].Size.ToString();
                        }
                        if (list[i].Width != null)
                        {
                            worksheet.Cells[a + 2, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 3, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 4, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 5, 8].Value = list[i].Width.ToString();
                            worksheet.Cells[a + 6, 8].Value = list[i].Width.ToString();
                        }
                        if (list[i].Length != null)
                        {
                            worksheet.Cells[a + 2, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 3, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 4, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 5, 9].Value = list[i].Length.ToString();
                            worksheet.Cells[a + 6, 9].Value = list[i].Length.ToString();
                        }
                        if (list[i].SingleQuota != null)
                        {
                            worksheet.Cells[a + 2, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 3, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 4, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 5, 10].Value = list[i].SingleQuota.ToString();
                            worksheet.Cells[a + 6, 10].Value = list[i].SingleQuota.ToString();
                        }
                        if (list[i].PlanOrderNo != null)
                        {
                            worksheet.Cells[a + 2, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 3, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 4, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 5, 11].Value = list[i].PlanOrderNo.ToString();
                            worksheet.Cells[a + 6, 11].Value = list[i].PlanOrderNo.ToString();
                        }
                        if (list[i].PlanOrderUnit != null)
                        {
                            worksheet.Cells[a + 2, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 3, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 4, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 5, 12].Value = list[i].PlanOrderUnit.ToString();
                            worksheet.Cells[a + 6, 12].Value = list[i].PlanOrderUnit.ToString();
                        }
                        if (list[i].ACCovers != null)
                        {
                            worksheet.Cells[a + 2, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 3, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 4, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 5, 13].Value = list[i].ACCovers.ToString();
                            worksheet.Cells[a + 6, 13].Value = list[i].ACCovers.ToString();
                        }
                        if (list[i].RequiredDockDate != null)
                        {
                            worksheet.Cells[a + 2, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 3, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 4, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 5, 14].Value = list[i].RequiredDockDate.ToString();
                            worksheet.Cells[a + 6, 14].Value = list[i].RequiredDockDate.ToString();
                        }
                        if (list[i].Purchasing != null)
                        {
                            worksheet.Cells[a + 2, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 3, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 4, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 5, 15].Value = list[i].Purchasing.ToString();
                            worksheet.Cells[a + 6, 15].Value = list[i].Purchasing.ToString();
                        }
                        var num = _scheduleService.getItem(list[i].Item);
                        var numorder = _sysOrderService.getPlan(list[i].Item);
                        double countx = 0;
                        double countb = 0;
                        double countf = 0;
                        double countz = 0;
                        double countd = 0;
                        //CheckAndPass
                        for (int no = 0; no <= numorder.Count - 1; no++)
                        {
                            if (numorder[no].Qty != null && numorder[no].Qty != "")
                            {
                                if (numorder[no].Reduced>0&& numorder[no].Reduced!=null) {
                                    countx = countx + Convert.ToDouble(numorder[no].Reduced) * Convert.ToDouble(numorder[no].Qty.ToString());
                                }
                                else
                                {
                                    countx = countx + Convert.ToDouble(numorder[no].Qty.ToString());
                                }
                               
                            }
                        }
                        for (int n = 0; n <= num.Count - 1; n++)
                        {
                            if (num[n].Reduced != null && num[n].Reduced > 0)
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass != "是" &&( main.Mbl == "" || main.Mbl == null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                {
                                    countf = countf + Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                }
                            }
                            else
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass != "是" &&  (main.Mbl == "" || main.Mbl == null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                {
                                    countf = countf + Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                }
                            }
                            if (num[n].Reduced != null && num[n].Reduced > 0)
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass != "是" &&( main.Mbl != "" && main.Mbl != null )&& (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                {
                                    countz = countz + Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                }

                            }
                            else
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl != "" && main.Mbl != null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                {
                                    countz = countz + Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                }
                            }
                            if (num[n].Reduced != null && num[n].Reduced > 0)
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass == "是" && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                {
                                    countd = countd + Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                }

                            }
                            else
                            {
                                var main = _importTrans_main_recordService.getById(num[n].MainId);
                                if (main.IsDeleted != true && main.CheckAndPass == "是" && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                {
                                    countd = countd + Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                }
                            }

                        }
                        worksheet.Cells[a + 2, 16].Value = "询价中";
                        worksheet.Cells[a + 3, 16].Value = "供应商备货";
                        worksheet.Cells[a + 4, 16].Value = "已发货待运输";
                        worksheet.Cells[a + 5, 16].Value = "在途运输";
                        worksheet.Cells[a + 6, 16].Value = "已到货";

                        worksheet.Cells[a + 2, 17].Value = Convert.ToDouble(list[i].PlanNo) - countx;
                        worksheet.Cells[a + 3, 17].Value = countx - countf - countz - countd;
                        worksheet.Cells[a + 4, 17].Value = countf;
                        worksheet.Cells[a + 5, 17].Value = countz;
                        worksheet.Cells[a + 6, 17].Value = countd;
                    }
                }
                package.Save();
            }
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        [Route("gfProcurementPlanMainIndex", Name = "GfPlanList")]
        [Function("采购计划执行状态跟踪表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.GFProcurementPlanController.GFProcurementPlanMainIndex")]
        public IActionResult GfPlanList(List<int> checkboxId)
        {
                int asd = 0;
               
                  
                    string sWebRootFolder = _hostingEnvironment.WebRootPath;
                    string sFileName = "采购计划执行状态跟踪表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
                    FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
                    file.Delete();
                    using (ExcelPackage package = new ExcelPackage(file))
                    {
                        // 添加worksheet
                        ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("采购计划执行状态跟踪表");
                        //添加头
                        worksheet.Cells[1, 1].Value = "计划检索号";
                        worksheet.Cells[1, 2].Value = "序号";
                        worksheet.Cells[1, 3].Value = "物料编码";
                        worksheet.Cells[1, 4].Value = "材料名称";
                        worksheet.Cells[1, 5].Value = "材料牌号/件号";
                        worksheet.Cells[1, 6].Value = "规范";
                        worksheet.Cells[1, 7].Value = "规格1";
                        worksheet.Cells[1, 8].Value = "规格2";
                        worksheet.Cells[1, 9].Value = "规格3";
                        worksheet.Cells[1, 10].Value = "单机定额";
                        worksheet.Cells[1, 11].Value = "要求采购数量";
                        worksheet.Cells[1, 12].Value = "采购单位";
                        worksheet.Cells[1, 13].Value = "采购起止架份";
                        worksheet.Cells[1, 14].Value = "要求到货时间";
                        worksheet.Cells[1, 15].Value = "采购依据";
                        worksheet.Cells[1, 16].Value = "采购状态描述";
                        worksheet.Cells[1, 17].Value = "数值";
                    foreach (int u in checkboxId)
                    {
                        var list = _sysProcurementPlanService.getAll(u);
                        // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                        for (int a = 1; a <= 27; a++)
                        {
                            worksheet.Cells[1, a].Style.Font.Bold = true;
                        }
                        //添加值
                        for (int i = 0; i <= list.Count - 1; i++)
                        {
                            for (int a = (asd+ i) * 5; a <= (asd + i) * 5 +4; a = a + 5)
                            {
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a + 2, 1].Value = list[i].PlanMain.PlanItem.ToString();
                                    worksheet.Cells[a + 3, 1].Value = list[i].PlanMain.PlanItem.ToString();
                                    worksheet.Cells[a + 4, 1].Value = list[i].PlanMain.PlanItem.ToString();
                                    worksheet.Cells[a + 5, 1].Value = list[i].PlanMain.PlanItem.ToString();
                                    worksheet.Cells[a + 6, 1].Value = list[i].PlanMain.PlanItem.ToString();
                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a + 2, 2].Value = list[i].Item.ToString();
                                    worksheet.Cells[a + 3, 2].Value = list[i].Item.ToString();
                                    worksheet.Cells[a + 4, 2].Value = list[i].Item.ToString();
                                    worksheet.Cells[a + 5, 2].Value = list[i].Item.ToString();
                                    worksheet.Cells[a + 6, 2].Value = list[i].Item.ToString();
                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a + 2, 3].Value = list[i].Materialno.ToString();
                                    worksheet.Cells[a + 3, 3].Value = list[i].Materialno.ToString();
                                    worksheet.Cells[a + 4, 3].Value = list[i].Materialno.ToString();
                                    worksheet.Cells[a + 5, 3].Value = list[i].Materialno.ToString();
                                    worksheet.Cells[a + 6, 3].Value = list[i].Materialno.ToString();
                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a + 2, 4].Value = list[i].Name.ToString();
                                    worksheet.Cells[a + 3, 4].Value = list[i].Name.ToString();
                                    worksheet.Cells[a + 4, 4].Value = list[i].Name.ToString();
                                    worksheet.Cells[a + 5, 4].Value = list[i].Name.ToString();
                                    worksheet.Cells[a + 6, 4].Value = list[i].Name.ToString();
                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a + 2, 5].Value = list[i].PartNo.ToString();
                                    worksheet.Cells[a + 3, 5].Value = list[i].PartNo.ToString();
                                    worksheet.Cells[a + 4, 5].Value = list[i].PartNo.ToString();
                                    worksheet.Cells[a + 5, 5].Value = list[i].PartNo.ToString();
                                    worksheet.Cells[a + 6, 5].Value = list[i].PartNo.ToString();
                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a + 2, 6].Value = list[i].Technical.ToString();
                                    worksheet.Cells[a + 3, 6].Value = list[i].Technical.ToString();
                                    worksheet.Cells[a + 4, 6].Value = list[i].Technical.ToString();
                                    worksheet.Cells[a + 5, 6].Value = list[i].Technical.ToString();
                                    worksheet.Cells[a + 6, 6].Value = list[i].Technical.ToString();
                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a + 2, 7].Value = list[i].Size.ToString();
                                    worksheet.Cells[a + 3, 7].Value = list[i].Size.ToString();
                                    worksheet.Cells[a + 4, 7].Value = list[i].Size.ToString();
                                    worksheet.Cells[a + 5, 7].Value = list[i].Size.ToString();
                                    worksheet.Cells[a + 6, 7].Value = list[i].Size.ToString();
                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a + 2, 8].Value = list[i].Width.ToString();
                                    worksheet.Cells[a + 3, 8].Value = list[i].Width.ToString();
                                    worksheet.Cells[a + 4, 8].Value = list[i].Width.ToString();
                                    worksheet.Cells[a + 5, 8].Value = list[i].Width.ToString();
                                    worksheet.Cells[a + 6, 8].Value = list[i].Width.ToString();
                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a + 2, 9].Value = list[i].Length.ToString();
                                    worksheet.Cells[a + 3, 9].Value = list[i].Length.ToString();
                                    worksheet.Cells[a + 4, 9].Value = list[i].Length.ToString();
                                    worksheet.Cells[a + 5, 9].Value = list[i].Length.ToString();
                                    worksheet.Cells[a + 6, 9].Value = list[i].Length.ToString();
                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a + 2, 10].Value = list[i].SingleQuota.ToString();
                                    worksheet.Cells[a + 3, 10].Value = list[i].SingleQuota.ToString();
                                    worksheet.Cells[a + 4, 10].Value = list[i].SingleQuota.ToString();
                                    worksheet.Cells[a + 5, 10].Value = list[i].SingleQuota.ToString();
                                    worksheet.Cells[a + 6, 10].Value = list[i].SingleQuota.ToString();
                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a + 2, 11].Value = list[i].PlanOrderNo.ToString();
                                    worksheet.Cells[a + 3, 11].Value = list[i].PlanOrderNo.ToString();
                                    worksheet.Cells[a + 4, 11].Value = list[i].PlanOrderNo.ToString();
                                    worksheet.Cells[a + 5, 11].Value = list[i].PlanOrderNo.ToString();
                                    worksheet.Cells[a + 6, 11].Value = list[i].PlanOrderNo.ToString();
                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a + 2, 12].Value = list[i].PlanOrderUnit.ToString();
                                    worksheet.Cells[a + 3, 12].Value = list[i].PlanOrderUnit.ToString();
                                    worksheet.Cells[a + 4, 12].Value = list[i].PlanOrderUnit.ToString();
                                    worksheet.Cells[a + 5, 12].Value = list[i].PlanOrderUnit.ToString();
                                    worksheet.Cells[a + 6, 12].Value = list[i].PlanOrderUnit.ToString();
                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a + 2, 13].Value = list[i].ACCovers.ToString();
                                    worksheet.Cells[a + 3, 13].Value = list[i].ACCovers.ToString();
                                    worksheet.Cells[a + 4, 13].Value = list[i].ACCovers.ToString();
                                    worksheet.Cells[a + 5, 13].Value = list[i].ACCovers.ToString();
                                    worksheet.Cells[a + 6, 13].Value = list[i].ACCovers.ToString();
                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a + 2, 14].Value = list[i].RequiredDockDate.ToString();
                                    worksheet.Cells[a + 3, 14].Value = list[i].RequiredDockDate.ToString();
                                    worksheet.Cells[a + 4, 14].Value = list[i].RequiredDockDate.ToString();
                                    worksheet.Cells[a + 5, 14].Value = list[i].RequiredDockDate.ToString();
                                    worksheet.Cells[a + 6, 14].Value = list[i].RequiredDockDate.ToString();
                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a + 2, 15].Value = list[i].Purchasing.ToString();
                                    worksheet.Cells[a + 3, 15].Value = list[i].Purchasing.ToString();
                                    worksheet.Cells[a + 4, 15].Value = list[i].Purchasing.ToString();
                                    worksheet.Cells[a + 5, 15].Value = list[i].Purchasing.ToString();
                                    worksheet.Cells[a + 6, 15].Value = list[i].Purchasing.ToString();
                                }
                                var num = _scheduleService.getItem(list[i].Item);
                                var numorder = _sysOrderService.getPlan(list[i].Item);
                                double countx = 0;
                                double countb = 0;
                                double countf = 0;
                                double countz = 0;
                                double countd = 0;
                            //CheckAndPass
                            for (int no = 0; no <= numorder.Count - 1; no++)
                            {
                                if (numorder[no].Qty != null && numorder[no].Qty != "")
                                {
                                    if (numorder[no].Reduced > 0 && numorder[no].Reduced != null)
                                    {
                                        countx = countx + Convert.ToDouble(numorder[no].Reduced) * Convert.ToDouble(numorder[no].Qty.ToString());
                                    }
                                    else
                                    {
                                        countx = countx + Convert.ToDouble(numorder[no].Qty.ToString());
                                    }

                                }
                            }
                            for (int n = 0; n <= num.Count - 1; n++)
                            {
                                if (num[n].Reduced != null && num[n].Reduced > 0)
                                {
                                    var main = _importTrans_main_recordService.getById(num[n].MainId);
                                    if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl == "" || main.Mbl == null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                    {
                                        countf = countf + Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                    }
                                }
                                else
                                {
                                    var main = _importTrans_main_recordService.getById(num[n].MainId);
                                    if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl == "" || main.Mbl == null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                    {
                                        countf = countf + Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                    }
                                }
                                if (num[n].Reduced != null && num[n].Reduced > 0)
                                {
                                    var main = _importTrans_main_recordService.getById(num[n].MainId);
                                    if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl != "" && main.Mbl != null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                    {
                                        countz = countz + Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                    }

                                }
                                else
                                {
                                    var main = _importTrans_main_recordService.getById(num[n].MainId);
                                    if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl != "" && main.Mbl != null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                    {
                                        countz = countz + Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                    }
                                }
                                if (num[n].Reduced != null && num[n].Reduced > 0)
                                {
                                    var main = _importTrans_main_recordService.getById(num[n].MainId);
                                    if (main.IsDeleted != true && main.CheckAndPass == "是" && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                    {
                                        countd = countd + Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                    }
                                }
                                else
                                {
                                    var main = _importTrans_main_recordService.getById(num[n].MainId);
                                    if (main.IsDeleted != true && main.CheckAndPass == "是" && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                                    {
                                        countd = countd + Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                    }
                                }
                            }
                                worksheet.Cells[a + 2, 16].Value = "询价中";
                                worksheet.Cells[a + 3, 16].Value = "供应商备货";
                                worksheet.Cells[a + 4, 16].Value = "已发货待运输";
                                worksheet.Cells[a + 5, 16].Value = "在途运输";
                                worksheet.Cells[a + 6, 16].Value = "已到货";
                                worksheet.Cells[a + 2, 17].Value = Convert.ToDouble(list[i].PlanNo) - countx;
                                worksheet.Cells[a + 3, 17].Value = countx - countf - countz - countd;
                                worksheet.Cells[a + 4, 17].Value = countf;
                                worksheet.Cells[a + 5, 17].Value = countz;
                                worksheet.Cells[a + 6, 17].Value = countd;
                            }
                        }
                        asd=asd + list.Count;
                    }
                        package.Save();
                    }
                    return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
             
         
           
        }
        [Route("GfOrderText", Name = "GfOrderText")]
        [Function("采购订单执行状态跟踪表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.GFProcurementPlanController.GFProcurementPlanMainIndex")]
        public IActionResult GfOrder(int id)
        {

            var list = _sysProcurementPlanService.getAll(id);
            if (list.Count == 0)
            {
                Response.WriteAsync("<script>alert('当前索引号未录入采购计划!');window.location.href ='gfProcurementPlanMainIndex'</script>", Encoding.GetEncoding("GB2312"));
            }
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "采购订单执行状态跟踪表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("采购订单执行状态跟踪表");
                //添加头
                worksheet.Cells[1, 1].Value = "计划检索号";
                worksheet.Cells[1, 2].Value = "序号";
                worksheet.Cells[1, 3].Value = "物料编码";
                worksheet.Cells[1, 4].Value = "材料名称";
                worksheet.Cells[1, 5].Value = "材料牌号/件号";
                worksheet.Cells[1, 6].Value = "规范";
                worksheet.Cells[1, 7].Value = "规格1";
                worksheet.Cells[1, 8].Value = "规格2";
                worksheet.Cells[1, 9].Value = "规格3";
                worksheet.Cells[1, 10].Value = "单机定额";
                worksheet.Cells[1, 11].Value = "包装规格";
                worksheet.Cells[1, 12].Value = "计划数量";
                worksheet.Cells[1, 13].Value = "计划单位";
                worksheet.Cells[1, 14].Value = "要求采购数量";
                worksheet.Cells[1, 15].Value = "采购单位";
                worksheet.Cells[1, 16].Value = "采购起止架份";
                worksheet.Cells[1, 17].Value = "要求到货时间";
                worksheet.Cells[1, 18].Value = "采购依据";
                worksheet.Cells[1, 19].Value = "申请号";
                worksheet.Cells[1, 20].Value = "备注1";
                worksheet.Cells[1, 21].Value = "备注2";
                worksheet.Cells[1, 22].Value = "采购状态描述";
                worksheet.Cells[1, 23].Value = "数值";
                worksheet.Cells[1, 24].Value = "订单约定发货日期";
                worksheet.Cells[1, 25].Value = "供应商发货日期";
                worksheet.Cells[1, 26].Value = "预计入厂日期";
                worksheet.Cells[1, 27].Value = "实际到货日期";
                worksheet.Cells[1, 28].Value = "与计划符合性";
                worksheet.Cells[1, 29].Value = "运单号";
                worksheet.Cells[1, 30].Value = "订单号";
                int a = 2;
                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int b = 1; b <= 30; b++)
                {
                    worksheet.Cells[1, b].Style.Font.Bold = true;
                }
                //添加值
                for (int i = 0; i <= list.Count - 1; i++)
                {


                    var num = _scheduleService.getItem(list[i].Item);
                    var numorder = _sysOrderService.getPlan(list[i].Item);
                   
                    double countx = 0;
                    double countb = 0;
                    double countf = 0;
                    double countz = 0;
                    double countd = 0;
                    //CheckAndPass
                    for (int no = 0; no <= numorder.Count - 1; no++)
                    {
                        var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                        
                        if (numorder[no].Qty != null && numorder[no].Qty != "")
                        {
                           
                            if (numorder[no].Reduced > 0 && numorder[no].Reduced != null)
                            {
                                countb =  Convert.ToDouble(numorder[no].Reduced) * Convert.ToDouble(numorder[no].Qty.ToString());
                            }
                            else
                            {
                                countb = Convert.ToDouble(numorder[no].Qty.ToString());
                            }
                            var sud = _scheduleService.getOrder(numorder[no].OrderNo, numorder[no].Item);
                            double s = 0;
                            for (int nos = 0; nos <= sud.Count - 1; nos++)
                            {
                                if (sud[nos].PurchaseQuantity != "" && sud[nos].PurchaseQuantity != null) {
                                if (sud[nos].Reduced > 0 && sud[nos].Reduced != null )
                                {
                                    s =s+ Convert.ToDouble(sud[nos].Reduced) * Convert.ToDouble(sud[nos].PurchaseQuantity.ToString());
                                }
                                else
                                {
                                    s = s + Convert.ToDouble(sud[nos].PurchaseQuantity.ToString());
                                    }
                                }
                            }
                            if (order != null && (countb - s)>0)
                            {

                           
                            if (list[i].PlanMain.PlanItem != null)
                            {
                                worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                            }
                            if (list[i].Item != null)
                            {
                                worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                            }
                            if (list[i].Materialno != null)
                            {
                                worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                            }
                            if (list[i].Name != null)
                            {
                                worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                            }
                            if (list[i].PartNo != null)
                            {
                                worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                            }
                            if (list[i].Technical != null)
                            {
                                worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                            }
                            if (list[i].Size != null)
                            {
                                worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                            }
                            if (list[i].Width != null)
                            {
                                worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                            }
                            if (list[i].Length != null)
                            {
                                worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                            }
                            if (list[i].SingleQuota != null)
                            {
                                worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                            }

                            if (list[i].Package != null)
                            {
                                worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                            }
                            if (list[i].PlanNo != null)
                            {
                                worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                            }
                            if (list[i].PlanUnit != null)
                            {
                                worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                            }

                            if (list[i].PlanOrderNo != null)
                            {
                                worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                            }
                            if (list[i].PlanOrderUnit != null)
                            {
                                worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                            }
                            if (list[i].ACCovers != null)
                            {
                                worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                            }
                            if (list[i].RequiredDockDate != null)
                            {
                                worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                            }
                            if (list[i].Purchasing != null)
                            {
                                worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                            }
                            if (list[i].Application != null)
                            {
                                worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                            }
                            if (list[i].Remark1 != null)
                            {
                                worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                            }
                            if (list[i].Remark2 != null)
                            {
                                worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                            }
                            worksheet.Cells[a, 22].Value = "供应商备货";
                            worksheet.Cells[a, 23].Value = countb - s;
                         
                            if (order.LeadTime != null)
                            {
                                worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                            }
                           
                            if (order.Main.Transportion == "AIR")
                            {
                                worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(30).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(30)).ToString();
                            }
                            else if (order.Main.Transportion == "SEA")
                            {
                                worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(90).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(90)).ToString();
                            }
                            else if (order.Main.Transportion == "Railway")
                            {
                                worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(80).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(80)).ToString();
                            }
                            else if (order.Main.Transportion == "Truck")
                            {
                                worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(10).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(10)).ToString();
                            }
                            worksheet.Cells[a, 30].Value = order.OrderNo;
                            a++;
                            }
                        }
                    }
                    for (int no = 0; no <= numorder.Count - 1; no++)
                    {
                        if (numorder[no].Qty != null && numorder[no].Qty != "")
                        {
                            if (numorder[no].Reduced > 0 && numorder[no].Reduced != null)
                            {
                                countx = countx + Convert.ToDouble(numorder[no].Reduced) * Convert.ToDouble(numorder[no].Qty.ToString());
                            }
                            else
                            {
                                countx = countx + Convert.ToDouble(numorder[no].Qty.ToString());
                            }

                        }
                    }
                    if (list[i].PlanMain.PlanItem != null)
                    {
                        worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                    }
                    if (list[i].Item != null)
                    {
                        worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                    }
                    if (list[i].Materialno != null)
                    {
                        worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                    }
                    if (list[i].Name != null)
                    {
                        worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                    }
                    if (list[i].PartNo != null)
                    {
                        worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                    }
                    if (list[i].Technical != null)
                    {
                        worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                    }
                    if (list[i].Size != null)
                    {
                        worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                    }
                    if (list[i].Width != null)
                    {
                        worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                    }
                    if (list[i].Length != null)
                    {
                        worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                    }
                    if (list[i].SingleQuota != null)
                    {
                        worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                    }

                    if (list[i].Package != null)
                    {
                        worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                    }
                    if (list[i].PlanNo != null)
                    {
                        worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                    }
                    if (list[i].PlanUnit != null)
                    {
                        worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                    }

                    if (list[i].PlanOrderNo != null)
                    {
                        worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                    }
                    if (list[i].PlanOrderUnit != null)
                    {
                        worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                    }
                    if (list[i].ACCovers != null)
                    {
                        worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                    }
                    if (list[i].RequiredDockDate != null)
                    {
                        worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                    }
                    if (list[i].Purchasing != null)
                    {
                        worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                    }
                    if (list[i].Application != null)
                    {
                        worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                    }
                    if (list[i].Remark1 != null)
                    {
                        worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                    }
                    if (list[i].Remark2 != null)
                    {
                        worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                    }
                    worksheet.Cells[a, 22].Value = "询价中";
                    worksheet.Cells[a, 23].Value = Convert.ToDouble(list[i].PlanNo) - countx;
                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - DateTime.Today).Days.ToString();
                    a++;
                    for (int n = 0; n <= num.Count - 1; n++)
                    {
                        if (num[n].Reduced != null && num[n].Reduced > 0)
                        {
                            var main = _importTrans_main_recordService.getById(num[n].MainId);
                            if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl == "" || main.Mbl == null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                            {
                                countf = Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                if (list[i].Package != null)
                                {
                                    worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                                }
                                if (list[i].PlanNo != null)
                                {
                                    worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                                }
                                if (list[i].PlanUnit != null)
                                {
                                    worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                                }
                                if (list[i].Application != null)
                                {
                                    worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                                }
                                if (list[i].Remark1 != null)
                                {
                                    worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                                }
                                if (list[i].Remark2 != null)
                                {
                                    worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                                }
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                                }
                                worksheet.Cells[a, 22].Value = "已发货待运输";
                                worksheet.Cells[a , 23].Value = countf;
                                var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                                if (order.LeadTime!=null)
                                {
                                    worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                                }
                                if (main.RealReceivingDate != null)
                                {
                                    worksheet.Cells[a, 25].Value = main.RealReceivingDate.Value.ToString("yyyy-MM-dd");
                                }
                             
                                if (order.Main.Transportion== "AIR")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(30).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(30)).ToString();
                                }
                                else if(order.Main.Transportion == "SEA")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(90).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(90)).ToString();
                                }
                                else if (order.Main.Transportion == "Railway")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(80).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(80)).ToString();
                                }
                                else if (order.Main.Transportion == "Truck")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(10).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(10)).ToString();
                                }
                                worksheet.Cells[a, 30].Value = order.OrderNo;
                                a = a + 1;
                            }
                        }
                        else
                        {
                            var main = _importTrans_main_recordService.getById(num[n].MainId);
                            if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl == "" || main.Mbl == null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                            {
                                countf = Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                if (list[i].Package != null)
                                {
                                    worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                                }
                                if (list[i].PlanNo != null)
                                {
                                    worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                                }
                                if (list[i].PlanUnit != null)
                                {
                                    worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                                }
                                if (list[i].Application != null)
                                {
                                    worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                                }
                                if (list[i].Remark1 != null)
                                {
                                    worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                                }
                                if (list[i].Remark2 != null)
                                {
                                    worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                                }
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                                }
                                worksheet.Cells[a, 22].Value = "已发货待运输";
                                worksheet.Cells[a, 23].Value = countf;
                                var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                                if (order.LeadTime != null)
                                {
                                    worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                                }
                                if (main.RealReceivingDate != null)
                                {
                                    worksheet.Cells[a, 25].Value = main.RealReceivingDate.Value.ToString("yyyy-MM-dd");
                                }

                                if (order.Main.Transportion == "AIR")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(30).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(30)).ToString();
                                }
                                else if (order.Main.Transportion == "SEA")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(90).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(90)).ToString();
                                }
                                else if (order.Main.Transportion == "Railway")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(80).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(80)).ToString();
                                }
                                else if (order.Main.Transportion == "Truck")
                                {
                                    worksheet.Cells[a, 26].Value = order.LeadTime.Value.AddDays(10).ToString("yyyy-MM-dd");
                                    worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - order.LeadTime.Value.AddDays(10)).ToString();
                                }
                                worksheet.Cells[a, 30].Value = order.OrderNo;
                                a = a + 1;
                            }
                        }
                        if (num[n].Reduced != null && num[n].Reduced > 0)
                        {
                            var main = _importTrans_main_recordService.getById(num[n].MainId);
                            if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl != "" && main.Mbl != null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                            {
                                countz = Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                if (list[i].Package != null)
                                {
                                    worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                                }
                                if (list[i].PlanNo != null)
                                {
                                    worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                                }
                                if (list[i].PlanUnit != null)
                                {
                                    worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                                }
                                if (list[i].Application != null)
                                {
                                    worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                                }
                                if (list[i].Remark1 != null)
                                {
                                    worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                                }
                                if (list[i].Remark2 != null)
                                {
                                    worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                                }
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                                }
                                worksheet.Cells[a, 22].Value = "在途运输";
                                worksheet.Cells[a, 23].Value = countz;
                                var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                                if (order.LeadTime != null)
                                {
                                    worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                                }
                                if (main.RealReceivingDate != null)
                                {
                                    worksheet.Cells[a, 25].Value = main.RealReceivingDate.Value.ToString("yyyy-MM-dd");
                                }
                                worksheet.Cells[a, 26].Value =  main.Ata.Value.AddDays(5).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - main.Ata.Value.AddDays(5)).ToString();
                                worksheet.Cells[a, 29].Value = main.Mbl;
                                worksheet.Cells[a, 30].Value = order.OrderNo;

                                a = a + 1;
                            }

                        }
                        else
                        {
                            var main = _importTrans_main_recordService.getById(num[n].MainId);
                            if (main.IsDeleted != true && main.CheckAndPass != "是" && (main.Mbl != "" && main.Mbl != null) && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                            {
                                countz = Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                if (list[i].Package != null)
                                {
                                    worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                                }
                                if (list[i].PlanNo != null)
                                {
                                    worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                                }
                                if (list[i].PlanUnit != null)
                                {
                                    worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                                }
                                if (list[i].Application != null)
                                {
                                    worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                                }
                                if (list[i].Remark1 != null)
                                {
                                    worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                                }
                                if (list[i].Remark2 != null)
                                {
                                    worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                                }
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                                }
                                worksheet.Cells[a, 22].Value = "在途运输";
                                worksheet.Cells[a, 23].Value = countz;
                                var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                                if (order.LeadTime != null)
                                {
                                    worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                                }
                                if (main.RealReceivingDate != null)
                                {
                                    worksheet.Cells[a, 25].Value = main.RealReceivingDate.Value.ToString("yyyy-MM-dd");
                                }
                                worksheet.Cells[a, 26].Value = main.Ata.Value.AddDays(5).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - main.Ata.Value.AddDays(5)).ToString();
                                worksheet.Cells[a, 29].Value = main.Mbl;
                                worksheet.Cells[a, 30].Value = order.OrderNo;
                                a = a + 1;
                            }
                        }
                        if (num[n].Reduced != null && num[n].Reduced > 0)
                        {
                            var main = _importTrans_main_recordService.getById(num[n].MainId);
                            if (main.IsDeleted != true && main.CheckAndPass == "是" && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                            {
                                countd = Convert.ToDouble(num[n].Reduced) * Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                if (list[i].Package != null)
                                {
                                    worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                                }
                                if (list[i].PlanNo != null)
                                {
                                    worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                                }
                                if (list[i].PlanUnit != null)
                                {
                                    worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                                }
                                if (list[i].Application != null)
                                {
                                    worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                                }
                                if (list[i].Remark1 != null)
                                {
                                    worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                                }
                                if (list[i].Remark2 != null)
                                {
                                    worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                                }
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                                }
                                worksheet.Cells[a, 22].Value = "已到货";
                                worksheet.Cells[a, 23].Value = countd;
                                var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                                if (order.LeadTime != null)
                                {
                                    worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                                }
                                if (main.RealReceivingDate != null)
                                {
                                    worksheet.Cells[a, 25].Value = main.RealReceivingDate.Value.ToString("yyyy-MM-dd");
                                }
                                worksheet.Cells[a, 27].Value = main.ActualDeliveryDate.Value.AddDays(5).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - main.ActualDeliveryDate.Value.AddDays(5)).ToString();
                                worksheet.Cells[a, 29].Value = main.Mbl;
                                worksheet.Cells[a, 30].Value = order.OrderNo;
                                a = a + 1;
                            }

                        }
                        else
                        {
                            var main = _importTrans_main_recordService.getById(num[n].MainId);
                            if (main.IsDeleted != true && main.CheckAndPass == "是" && (num[n].PurchaseQuantity != null && num[n].PurchaseQuantity != ""))
                            {
                                countd = Convert.ToDouble(num[n].PurchaseQuantity.ToString());
                                if (list[i].Package != null)
                                {
                                    worksheet.Cells[a, 11].Value = list[i].Package.ToString();

                                }
                                if (list[i].PlanNo != null)
                                {
                                    worksheet.Cells[a, 12].Value = list[i].PlanNo.ToString();

                                }
                                if (list[i].PlanUnit != null)
                                {
                                    worksheet.Cells[a, 13].Value = list[i].PlanUnit.ToString();

                                }
                                if (list[i].Application != null)
                                {
                                    worksheet.Cells[a, 19].Value = list[i].Application.ToString();

                                }
                                if (list[i].Remark1 != null)
                                {
                                    worksheet.Cells[a, 20].Value = list[i].Remark1.ToString();

                                }
                                if (list[i].Remark2 != null)
                                {
                                    worksheet.Cells[a, 21].Value = list[i].Remark2.ToString();

                                }
                                if (list[i].PlanMain.PlanItem != null)
                                {
                                    worksheet.Cells[a, 1].Value = list[i].PlanMain.PlanItem.ToString();

                                }
                                if (list[i].Item != null)
                                {
                                    worksheet.Cells[a, 2].Value = list[i].Item.ToString();

                                }
                                if (list[i].Materialno != null)
                                {
                                    worksheet.Cells[a, 3].Value = list[i].Materialno.ToString();

                                }
                                if (list[i].Name != null)
                                {
                                    worksheet.Cells[a, 4].Value = list[i].Name.ToString();

                                }
                                if (list[i].PartNo != null)
                                {
                                    worksheet.Cells[a, 5].Value = list[i].PartNo.ToString();

                                }
                                if (list[i].Technical != null)
                                {
                                    worksheet.Cells[a, 6].Value = list[i].Technical.ToString();

                                }
                                if (list[i].Size != null)
                                {
                                    worksheet.Cells[a, 7].Value = list[i].Size.ToString();

                                }
                                if (list[i].Width != null)
                                {
                                    worksheet.Cells[a, 8].Value = list[i].Width.ToString();

                                }
                                if (list[i].Length != null)
                                {
                                    worksheet.Cells[a, 9].Value = list[i].Length.ToString();

                                }
                                if (list[i].SingleQuota != null)
                                {
                                    worksheet.Cells[a, 10].Value = list[i].SingleQuota.ToString();

                                }
                                if (list[i].PlanOrderNo != null)
                                {
                                    worksheet.Cells[a, 14].Value = list[i].PlanOrderNo.ToString();

                                }
                                if (list[i].PlanOrderUnit != null)
                                {
                                    worksheet.Cells[a, 15].Value = list[i].PlanOrderUnit.ToString();

                                }
                                if (list[i].ACCovers != null)
                                {
                                    worksheet.Cells[a, 16].Value = list[i].ACCovers.ToString();

                                }
                                if (list[i].RequiredDockDate != null)
                                {
                                    worksheet.Cells[a, 17].Value = list[i].RequiredDockDate.Value.ToString("yyyy-MM-dd");

                                }
                                if (list[i].Purchasing != null)
                                {
                                    worksheet.Cells[a, 18].Value = list[i].Purchasing.ToString();

                                }
                                worksheet.Cells[a, 22].Value = "已到货";
                                worksheet.Cells[a, 23].Value = countd;
                                var order = _sysOrderService.getOrder(num[i].OrderNo, num[i].OrderLine);
                                if (order.LeadTime != null)
                                {
                                    worksheet.Cells[a, 24].Value = order.LeadTime.Value.ToString("yyyy-MM-dd");
                                }
                                if (main.RealReceivingDate != null)
                                {
                                    worksheet.Cells[a, 25].Value = main.RealReceivingDate.Value.ToString("yyyy-MM-dd");
                                }
                                worksheet.Cells[a, 27].Value = main.ActualDeliveryDate.Value.AddDays(5).ToString("yyyy-MM-dd");
                                worksheet.Cells[a, 28].Value = (list[i].RequiredDockDate.Value - main.ActualDeliveryDate.Value.AddDays(5)).ToString();
                                worksheet.Cells[a, 29].Value = main.Mbl;
                                worksheet.Cells[a, 30].Value = order.OrderNo;
                                a = a + 1;
                            }
                        }

                    }
                    //

                    //worksheet.Cells[a + 3, 16].Value = "供应商备货";
                   

                   // worksheet.Cells[a + 2, 17].Value = Convert.ToDouble(list[i].PlanNo) - countx;
                   // worksheet.Cells[a + 3, 17].Value = countx - countf - countz - countd;
                   // worksheet.Cells[a + 4, 17].Value = countf;
                   // worksheet.Cells[a + 5, 17].Value = countz;
                   // worksheet.Cells[a + 6, 17].Value = countd;

                }
                package.Save();
            }
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
    }
}