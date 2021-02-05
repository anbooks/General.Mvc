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
using Microsoft.Win32;
using System.Diagnostics;
using Microsoft.AspNetCore.StaticFiles;
using General.Services.OrderMain;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itShipmentCreate")]
    public class ITShipmentCreateController : AdminPermissionController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private IOrderMainService _orderMainService;
        private ISysUserRoleService _sysUserRoleService;
        private ISysUserService _sysUserService;
        public ITShipmentCreateController(ISysUserService sysUserService,IOrderMainService orderMainService,ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._sysUserService = sysUserService;
            this._orderMainService = orderMainService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("itShipmentCreate", Name = "itShipmentCreate")]
        [Function("创建发运条目", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 0)]
        [HttpGet]
        public IActionResult ITShipmentCreateIndex( SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        { 
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _importTrans_main_recordService.searchList(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itShipmentCreate", arg);
            return View(dataSource);//sysImport
        }

        [Route("itShipmentCreatetran", Name = "itShipmentCreatetran")]
        [Function("创建发运条目(运代)", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 0)]
        [HttpGet]
        public IActionResult ITShipmentCreateTranIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
           string port= WorkContext.CurrentUser.Port;
            string tran = WorkContext.CurrentUser.Transport;
            var pageList = _importTrans_main_recordService.searchListYd(arg, page, size, port, tran);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itShipmentCreatetran", arg);
            return View(dataSource);//sysImport
        }
        [HttpGet]
        [Route("editTran", Name = "editITShipmentTranCreate")]
        [Function("编辑发运条目(运代)", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITShipmentCreateController.ITShipmentCreateTranIndex")]
        public IActionResult EditITShipmentTranCreate(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itShipmentCreatetran");
            var customizedList2 = _sysUserService.getTran();
            ViewData["Transportation"] = new SelectList(customizedList2, "Transport", "Transport");

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
        [Route("editTran")]
        public ActionResult EditITShipmentTranCreate(Entities.ImportTrans_main_record model, IFormFile excelfile, string returnUrl = null)
        {
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itShipmentCreatetran");
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
            if (model.Id.Equals(0))
            {
                model.CreationTime = DateTime.Now;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                model.Creator = WorkContext.CurrentUser.Id;
                var inc = _orderMainService.getByAccount(model.PoNo);
                model.Incoterms = inc.TradeTerms;
                if (model.PoNo == null || model.Shipper == null || model.Transportation == null)
                {

                    return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("editITShipmentTranCreate"));
                }
                _importTrans_main_recordService.insertImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit", Name = "editITShipmentCreate")]
        [Function("编辑发运条目", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITShipmentCreateController.ITShipmentCreateIndex")]
        public IActionResult EditITShipmentCreate(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itShipmentCreate");
            var customizedList2 = _sysUserService.getTran();
            ViewData["Transportation"] = new SelectList(customizedList2, "Transport", "Transport");
            
            if (id != null)
            {
                ViewBag.FJ = 1;
                var model = _importTrans_main_recordService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
           
            return View();
        }
       
        [HttpPost]
        [Route("edit")]
        public ActionResult EditITShipmentCreate(Entities.ImportTrans_main_record model, IFormFile excelfile, string returnUrl = null)
        {
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itShipmentCreate");
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
            if (model.Id.Equals(0))
            {
                model.CreationTime = DateTime.Now;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                model.Creator = WorkContext.CurrentUser.Id;
                var inc = _orderMainService.getByAccount(model.PoNo);
                model.Incoterms = inc.TradeTerms;
                if (model.PoNo==null||model.Shipper==null||model.Transportation==null) {
                   
                    return Redirect(Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("editITShipmentCreate"));
                }
                _importTrans_main_recordService.insertImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}