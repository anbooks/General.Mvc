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
    [Route("admin/inspectionjyy")]
    public class InspectionJyyController : AdminPermissionController
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
        public InspectionJyyController(IInspectionAttachmentService InspectionAttachmentService, ISysUserService sysUserService, IOrderService sysOrderService, IOrderMainService sysOrderMainService, IInspecationMainService sysInspectionMainService, IInspectionRecordService sysInspectionRecordService, IInspectionService sysInspectionService, ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
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
        [Route("InspectionjyyspList", Name = "InspectionjyyspList")]
        [Function("检验员", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        public IActionResult InspectionYList(string Jyy)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("inspectionjyy");
            string s;
            Request.Cookies.TryGetValue("Inspection", out s);
            int ida = Convert.ToInt32(s);
            try
            {
                var model = _sysInspectionMainService.getById(ida);
                model.Status = "检验员";
                model.flag = 5;
                model.CreationTime = DateTime.Now;
                model.JhyName = Jyy;
                _sysInspectionMainService.updateInspecationMain(model);
            }
            catch
            {
                return Redirect(ViewBag.ReturnUrl);
            }
            return Redirect(ViewBag.ReturnUrl);
        }

        [Route("inspectionjyy", Name = "inspectionjyy")]
        [Function("检验员审批", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionProcessController", Sort = 1)]
        [HttpGet]
        public IActionResult InspectionjyyIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            ViewBag.QX = WorkContext.CurrentUser.Name;
            var pageList = _sysInspectionMainService.searchInspecationMainjy(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.InspecationMain, SysCustomizedListSearchArg>("inspection", arg);
            return View(dataSource);//sysImport
        }
        [Route("inspectionjyysch", Name = "inspectionjyysch")]
        [Function("检验员审批明细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        [HttpGet]
        public IActionResult InspectionjyySchIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
            var dataSource = pageList.toDataSourceResult<Entities.Inspection, SysCustomizedListSearchArg>("inspectionjyy", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("InspectionjyyList", Name = "InspectionjyyList")]
        [Function("接收审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        public ActionResult InspectionjhyList(string  remark)
        {
            string s;
                Request.Cookies.TryGetValue("Inspection", out s);
            int ida = Convert.ToInt32(s);
            try
            {
                var record = _sysInspectionMainService.getById(ida);
                record.flag = 5;
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
        [Route("InspectionjyythList", Name = "InspectionjyythList")]
        [Function("退回审批", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        public ActionResult InspectionjyythList(string remark)
        {
            string s;

            Request.Cookies.TryGetValue("Inspection", out s);

            int ida = Convert.ToInt32(s);

            try
            {
                var record = _sysInspectionMainService.getById(ida);
                record.flag = 3;
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
        [HttpPost]
        [Route("InspectionjyythQx", Name = "InspectionjyythQx")]
        [Function("退回取消", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        public ActionResult InspectionjyythQx(string remark)
        {
                AjaxData.Status = true;
                AjaxData.Message = "OK";
            return Json(AjaxData);
        }
        [Route("JyyZbdAttachment", Name = "JyyZbdAttachment")]
        [Function("质保单查看", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        [HttpGet]
        public IActionResult InspectionjyyAttachmentIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
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
        [Route("JyydownLoadsjdfile", Name = "JyydownLoadsjdfile")]
        [Function("下载附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
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
        [Route("edit", Name = "editInspectionJyyAttachment")]
        [Function("质保单编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.InspectionJyyController.InspectionjyyIndex")]
        public IActionResult EditInspectionjyy(int id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("inspectionjyy");
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

            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditInspectionjyy(List<IFormFile> files, string returnUrl = null)
        {

            string s = "";
            int ida = 0;
            Request.Cookies.TryGetValue("Inspection", out s);
            ida = Convert.ToInt32(s);

            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("inspectionjyysch");

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