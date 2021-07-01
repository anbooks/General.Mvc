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
using Microsoft.AspNetCore.Hosting;
using General.Services.Inspection;
using General.Services.InspectionRecord;
using General.Services.InspecationMain;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/ccPaymentInvoice")]
    public class CCPaymentInvoiceController : AdminPermissionController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        private ISysUserService _sysUserService;
        private IInspectionService _sysInspectionService;
        private IInspectionRecordService _sysInspectionRecordService;
        private IInspecationMainService _sysInspectionMainService;
        public CCPaymentInvoiceController(ISysUserService sysUserService, IInspecationMainService sysInspectionMainService, IInspectionRecordService sysInspectionRecordService, IInspectionService sysInspectionService, ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
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

        [Route("", Name = "ccPaymentInvoice")]
        [Function("生成发票清单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.CashingController", Sort = 1)]
        [HttpGet]
        public IActionResult CCPaymentInvoiceIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _sysInspectionMainService.searchInspecationMain(arg, page, size, WorkContext.CurrentUser.Name);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspecationMain, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport  InvoiceSch
        }
        [Route("InvoiceSch", Name = "InvoiceSch")]
        [Function("查看送检单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.CCPaymentInvoiceController.CCPaymentInvoiceIndex")]
        [HttpGet]
        public IActionResult InvoiceSchIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
        [Route("Invoiceschedule", Name = "Invoiceimportasja")]
        [Function("送检审批生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITBuyerController.ITBuyerIndex")]
        public IActionResult Export3(List<int> checkboxId)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionbuyersch");
           
            try
            {
               

            }
            catch
            {
               

            }
            return Redirect(ViewBag.ReturnUrl);

        }
    }
}