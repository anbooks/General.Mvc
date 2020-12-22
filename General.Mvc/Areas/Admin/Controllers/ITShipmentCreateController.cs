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
    [Route("admin/itShipmentCreate")]
    public class ITShipmentCreateController : AdminPermissionController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        public ITShipmentCreateController(ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itShipmentCreate")]
        [Function("创建发运条目(新)", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]
        [HttpGet]
        public IActionResult ITShipmentCreateIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            
            
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurr"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var USER = _sysUserRoleService.getById(WorkContext.CurrentUser.Id);
            ViewBag.QX = USER.RoleName;
            var pageList = _importTrans_main_recordService.searchList(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itShipmentCreate", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("itShipmentCreateList", Name = "itShipmentCreateList")]
        [Function("发运条目编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITShipmentCreateController.ITShipmentCreateIndex")]
        public ActionResult ITShipmentCreateList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try { 
            foreach (Entities.ImportTrans_main_record u in jsonlist)
            {
                var model = _importTrans_main_recordService.getById(u.Id);
                model.Itemno = u.Itemno;
                model.Shipper = u.Shipper;
                model.PoNo = u.PoNo;
                model.Incoterms = u.Incoterms;
                model.CargoType = u.CargoType;
                model.Invamou = u.Invamou;
                if (u.Invcurr!="") { model.Invcurr = u.Invcurr; }
                model.RealReceivingDate = u.RealReceivingDate;
                model.Gw = u.Gw;
                model.Pcs = u.Pcs;
                model.Buyer = u.PoNo.Substring(1, 2);
                _importTrans_main_recordService.updateImportTransmain(model);
                //u就是jsonlist里面的一个实体类
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
        [Route("downLoadInventory", Name = "downLoadInventory")]
        [Function("下载附件", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITShipmentCreateController.ITShipmentCreateIndex")]
        public FileStreamResult DownLoadInventory(int? id)
        {
            var model = _importTrans_main_recordService.getById(id.Value);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
            string sFileName = model.InventoryAttachment;
            FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
            FileStream fs = new FileStream(file.ToString(), FileMode.Create);
            return File(fs, "application/octet-stream", sFileName);
        }
        [HttpGet]
        [Route("edit", Name = "editITShipmentCreate")]
        [Function("编辑发运条目", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITShipmentCreateController.ITShipmentCreateIndex")]
        public IActionResult EditITShipmentCreate(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itShipmentCreate");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            if (id != null)
            {
                var model = _importTrans_main_recordService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            else
            {
                ViewBag.FJ = 1;
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
                _importTrans_main_recordService.insertImportTransmain(model);
            }
            else
            {
                if (excelfile!=null) {
                    string sWebRootFolder = _hostingEnvironment.WebRootPath;
                    var fileProfile = sWebRootFolder + "\\Files\\inventoryfile\\";
                    string sFileName = model.Id + "-" + $"{DateTime.Now.ToString("yyMMdd")}" + excelfile.FileName;
                    FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
                    using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
                    {
                        excelfile.CopyTo(fs);
                        fs.Flush();
                    }
                    model.InventoryAttachment = sFileName;
                }
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}