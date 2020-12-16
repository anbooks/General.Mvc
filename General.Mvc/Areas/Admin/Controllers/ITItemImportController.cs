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
    [Route("admin/itItemImport")]
    public class ITItemImportController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private ISysRoleService _sysRoleService;
        private IOrderService _sysOrderService;
       
        private ISysCustomizedListService _sysCustomizedListService;
        public ITItemImportController(ISysCustomizedListService sysCustomizedListService, IOrderService sysOrderService,IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
        {
            this._sysCustomizedListService = sysCustomizedListService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysRoleService = sysRoleService;
            this._sysOrderService = sysOrderService;
            this._hostingEnvironment = hostingEnvironment;
        }
        [Route("", Name = "itItemImportIndex")]
        [Function("条目数据导入", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.DataImportController", Sort = 1)]

        public IActionResult ITItemImportIndex( SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysOrderService.searchOrder(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Order, SysCustomizedListSearchArg>("itItemImportIndex", arg);
            return View(dataSource);//sysImport
        }
        [Route("Itemexcelimport", Name = "Itemexcelimport")]
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
            //row1.CreateCell(2).SetCellValue("电话");
            //row1.CreateCell(3).SetCellValue("注册时间");
            //row1.CreateCell(4).SetCellValue("邀请人ID");
            //row1.CreateCell(5).SetCellValue("邀请人名称");
            //row1.CreateCell(6).SetCellValue("邀请人电话");
            //row1.CreateCell(7).SetCellValue("总积分");
            //row1.CreateCell(8).SetCellValue("已使用积分");
            //row1.CreateCell(9).SetCellValue("可用积分");
            //将数据逐步写入sheet1各个行
            for (int i = 0; i < list.Count; i++)
            {
                NPOI.SS.UserModel.IRow rowtemp = sheet1.CreateRow(i + 1);
                rowtemp.CreateCell(0).SetCellValue(list[i].Id.ToString());
                rowtemp.CreateCell(1).SetCellValue(list[i].Name);
                //rowtemp.CreateCell(2).SetCellValue(list[i].Phone);
                //rowtemp.CreateCell(3).SetCellValue(list[i].CreateTime.Value.ToString());
                //rowtemp.CreateCell(4).SetCellValue(list[i].InviterID.Value);
                //rowtemp.CreateCell(5).SetCellValue(list[i].iName);
                //rowtemp.CreateCell(6).SetCellValue(list[i].iPhone);
                //rowtemp.CreateCell(7).SetCellValue(list[i].IntegralSum);
                //rowtemp.CreateCell(8).SetCellValue(list[i].IntegralSy);
                //rowtemp.CreateCell(9).SetCellValue(list[i].IntegralKy);
            }
            // 写入到客户端 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            book.Write(ms);
            ms.Seek(0, SeekOrigin.Begin);
            string sFileName = $"{DateTime.Now}.xlsx";
            return File(ms, "application/vnd.ms-excel", sFileName);
        }
        [HttpPost]
        [Route("Itemimportexcel", Name = "Itemimportexcel")]
        public ActionResult Import(IFormFile excelfile, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itItemImportIndex");
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
            string sFileName = $"{Guid.NewGuid()}.xlsx";
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
                int orderno=0; int suppliername = 0; int suppliercode = 0; int item = 0; int materialcode = 0; int name = 0; int size = 0;
                for (int columns = 1;columns <= ColCount; columns++)
                {
                    //Entities.Order model = new Entities.Order();
                    if (worksheet.Cells[1, columns].Value.ToString() == "订单号") {  orderno = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "供应商名称") { suppliername = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "供应商代码") { suppliercode = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "行号") { item = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "物料代码") {  materialcode = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "品名") {  name = columns; }
                    if (worksheet.Cells[1, columns].Value.ToString() == "规格") {  size = columns; }
                }
                for (int row = 2; row <= rowCount; row++)
                {
                   
                    try
                    {
                        Entities.Order model = new Entities.Order();
                        model.OrderNo = worksheet.Cells[row, orderno].Value.ToString();
                    model.SupplierName = worksheet.Cells[row, suppliername].Value.ToString();
                    model.SupplierCode = worksheet.Cells[row, suppliercode].Value.ToString();
                    model.Item = worksheet.Cells[row, item].Value.ToString();
                    model.MaterialCode = worksheet.Cells[row, materialcode].Value.ToString();
                    model.Name = worksheet.Cells[row, name].Value.ToString();
                    model.Size = worksheet.Cells[row, size].Value.ToString();
                    model.CreationTime = DateTime.Now;
                    model.Creator = WorkContext.CurrentUser.Id;
                    _sysOrderService.insertOrder(model);
                    }
                    catch (Exception e)
                    {
                        ViewData["IsShowAlert"] = "True";
                        // return Content("<script>alert('请先登录');</script>");
                    }

                   
                   
                }
                return Redirect(ViewBag.ReturnUrl);
            }
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