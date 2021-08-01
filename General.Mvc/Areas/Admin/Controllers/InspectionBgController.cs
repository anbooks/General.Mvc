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
    [Route("admin/inspectionbg")]
    public class InspectionBgController : AdminPermissionController
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
        public InspectionBgController(IInspectionAttachmentService InspectionAttachmentService, ISysUserService sysUserService, IOrderService sysOrderService, IOrderMainService sysOrderMainService, IInspecationMainService sysInspectionMainService, IInspectionRecordService sysInspectionRecordService, IInspectionService sysInspectionService, ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
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
        [HttpPost]
        [Route("InspectionBgspList", Name = "InspectionBgspList")]
        [Function("保管员填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionBgController.InspectionBgIndex")]
        public ActionResult InspectionBgList(string kevin)
        {
            string test = kevin;
            List<Entities.Inspection> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Inspection>(test);
            try
            {
                foreach (Entities.Inspection u in jsonlist)
                {
                    var model = _sysInspectionService.getById(u.Id);

                    model.AcceptQty = u.AcceptQty;
                    model.AcceptTime = u.AcceptTime;
                    model.Keeper = u.Keeper;

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

        [Route("inspectionbg", Name = "inspectionbg")]
        [Function("保管员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 6)]
        [HttpGet]
        public IActionResult InspectionBgIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _sysInspectionMainService.searchInspecationMainjy(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspecationMain, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionbgsch", Name = "inspectionbgsch")]
        [Function("保管员审批明细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionBgController.InspectionBgIndex")]
        [HttpGet]
        public IActionResult InspectionbgSchIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            ViewBag.QX = ida;
            var pageList = _sysInspectionService.searchInspection(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspectionbg", arg);
            return View(dataSource);//sysImport
        }
        
        [Route("bgZbdAttachment", Name = "bgZbdAttachment")]
        [Function("质保单查看", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionBgController.InspectionBgIndex")]
        [HttpGet]
        public IActionResult InspectionbgAttachmentIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            var dataSource = pageList.toDataSourceResult<Entities.InspectionAttachment, SysCustomizedListSearchArg>("bgZbdAttachment", arg);
            return View(dataSource);//sysImport
        }
        [Route("bgdownLoadsjdfile", Name = "bgdownLoadsjdfile")]
        [Function("下载附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionBgController.InspectionBgIndex")]
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
       

    }
}