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
using General.Services.Order;
using General.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using General.Framework.Datatable;
using General.Services.SysCustomizedList;

namespace General.Mvc.Areas.Admin.Controllers
{
    //[Area("Admin")]
    [Route("admin/itOrderImport")]
    public class ITOrderImportController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ISysRoleService _sysRoleService;
        private IOrderService _sysOrderService;
       
        private ISysCustomizedListService _sysCustomizedListService;
        public ITOrderImportController(ISysCustomizedListService sysCustomizedListService, IOrderService sysOrderService,IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
        {
            this._sysCustomizedListService = sysCustomizedListService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysRoleService = sysRoleService;
            this._sysOrderService = sysOrderService;
            this._hostingEnvironment = hostingEnvironment;
        }
        [Route("", Name = "itOrderImportIndex")]
        [Function("订单数据导入", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.DataImportController", Sort = 1)]

        public IActionResult ITOrderImportIndex( SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysOrderService.searchOrder(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Order, SysCustomizedListSearchArg>("itOrderImportIndex", arg);
            return View(dataSource);//sysImport
        }
        [Route("excelimport", Name = "excelimport")]
        public FileStreamResult Excel(int?id)
        {
            var model = _sysOrderService.getById(id.Value);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\profile\\";
            string sFileName = model.Attachment;
            FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
            FileStream fs = new FileStream(file.ToString(), FileMode.Create);
            
            return File(fs,"application/octet-stream", sFileName);

        }
        [HttpPost]
        [Route("importexcel", Name = "importexcel")]
        public ActionResult Import(IFormFile excelfile, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itOrderImportIndex");
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\profile\\";
            string sFileName = excelfile.FileName;
            FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
            using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
            {
                excelfile.CopyTo(fs);
                fs.Flush();
            }
           return Redirect(ViewBag.ReturnUrl);
            
        }
        //[HttpPost]
        //public JsonResult ExcelToUpload(HttpPostedFileBase file)
        //{
        //    DataTable excelTable = new DataTable();
        //    string msg = string.Empty;
        //    if (Request.Files.Count > 0)
        //    {
        //        try
        //        {
        //            HttpPostedFileBase mypost = Request.Files[0];
        //            string fileName = Request.Files[0].FileName;
        //            string serverpath = Server.MapPath(string.Format("~/{0}", "ExcelFiles"));
        //            string path = Path.Combine(serverpath, fileName);
        //            mypost.SaveAs(path);
        //            excelTable = ImportExcel.GetExcelDataTable(path);

        //            //注意Excel表内容格式，第一行必须为列名与数据库列名匹配
        //            //接下来为各列名对应下来的内容  

        //            msg = SaveExcelToDB.InsertDataToDB(excelTable, "Table");// 写入基础数据
        //            //msg = SaveExcelToDB.InsAndDelDataToDB(excelTable, "Key", 1, ”Table“);// 写入基础数据，并删除其中的重复的项目                 
        //            //msg = SaveExcelToDB.UpdateDataToDB(excelTable, "[GamesList]");// 修改对应列
        //        }
        //        catch (Exception ex)
        //        {
        //            msg = ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        msg = "请选择文件";
        //    }
        //    return Json(msg);
        //}
    }
}