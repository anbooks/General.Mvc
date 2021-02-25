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
    [Route("admin/itConfirmedCustoms")]
    public class ITConfirmedCustomsController : AdminPermissionController
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        private IScheduleService _scheduleService;
        private ISysUserRoleService _sysUserRoleService;
        public ITConfirmedCustomsController(ISysUserRoleService sysUserRoleService, IHostingEnvironment hostingEnvironment, IScheduleService scheduleService, IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {
            this._hostingEnvironment = hostingEnvironment;
            this._sysUserRoleService = sysUserRoleService;
            this._scheduleService = scheduleService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itConfirmedCustoms")]
        [Function("综保报关行（新）", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 5)]
        [HttpGet]
        public IActionResult ITConfirmedCustomsIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("是否");
            ViewData["CheckAndPass"] = new SelectList(customizedList, "CustomizedValue", "Description");
            //var customizedList2 = _sysCustomizedListService.getByAccount("运输状态");
            //ViewData["Status"] = new SelectList(customizedList2, "CustomizedValue", "CustomizedValue");
            ViewBag.QX = WorkContext.CurrentUser.Co;
            var pageList = _importTrans_main_recordService.searchListLogistics(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itConfirmedCustoms", arg);
            return View(dataSource);//sysImport
        }

        [Route("schedule", Name = "itConfirmedCustomsSchedule")]
        [Function("综保报关行明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsIndex")]
        [HttpGet]
        public IActionResult ITConfirmedCustomsScheduleIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            ViewBag.QX = WorkContext.CurrentUser.Co;
            int? ida = id;
            if (ida == 0)
            {
                ViewBag.Import = HttpContext.Session.GetInt32("import");
            }
            else
            {
                HttpContext.Session.SetInt32("import", id);
                ViewBag.Import = HttpContext.Session.GetInt32("import");
            }
            //Session["username"] = id.ToString(); 
            int importid = ViewBag.Import;
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _scheduleService.searchList(arg, page, size, importid);
            var item = _importTrans_main_recordService.getById(importid);
            if (pageList.Count > 0)
            {
                item.F_ShippingModeGiven = true;

                _importTrans_main_recordService.updateImportTransmain(item);
            }
            ViewBag.orderno = item.PoNo;
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Schedule, SysCustomizedListSearchArg>("itConfirmedCustomsSchedule", arg);
            return View(dataSource);//sysImport
        }
        [HttpPost]
        [Route("itConfirmedCustomsScheduleList", Name = "itConfirmedCustomsScheduleList")]
        [Function("综保报关行明细表数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsIndex")]
        public ActionResult ITConfirmedCustomsScheduleList(string kevin)
        {
            string test = kevin;
            List<Entities.Schedule> jsonlist = JsonHelper.DeserializeJsonToList<Entities.Schedule>(test);
            //  Entities.ImportTrans_main_record model = new Entities.ImportTrans_main_record();
            try
            {
                foreach (Entities.Schedule u in jsonlist)
                {
                    var model = _scheduleService.getById(u.Id);
                    model.PurchaseQuantity = u.PurchaseQuantity;
                    model.PurchaseUnit = u.PurchaseUnit;
                    model.UnitPrice = u.UnitPrice;
                    model.TotalPrice = u.TotalPrice;
                    model.ShipmentDate = u.ShipmentDate;
                    model.Consignor = u.Consignor;
                    model.Manufacturers = u.Manufacturers;
                    model.OriginCountry = u.OriginCountry;
                    model.Books = u.Books;
                    model.BooksItem = u.BooksItem;
                    model.Waybill = u.Waybill;
                    model.BatchNo = u.BatchNo;
                    model.RecordUnit = u.RecordUnit;
                    model.RecordUnitReducedPrice = u.RecordUnitReducedPrice;
                    model.LegalUnits = u.LegalUnits;
                    model.LegalUniteReducedPrice = u.LegalUniteReducedPrice;
                    model.Qualification = u.Qualification;
                    _scheduleService.updateSchedule(model);
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
        [Route("excelimportConfirmedCustoms", Name = "excelimportConfirmedCustoms")]
        [Function("明细表导出", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsIndex")]
        public IActionResult Export2()
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var list = _scheduleService.getAll(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "明细表" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("明细表");
                //添加头
                worksheet.Cells[1, 1].Value = "采购员";
                worksheet.Cells[1, 2].Value = "订单号";
                worksheet.Cells[1, 3].Value = "索引号";
                worksheet.Cells[1, 4].Value = "物料代码";
                worksheet.Cells[1, 5].Value = "品名";
                worksheet.Cells[1, 6].Value = "牌号号";
                worksheet.Cells[1, 7].Value = "规范";
                worksheet.Cells[1, 8].Value = "规格1";
                worksheet.Cells[1, 9].Value = "规格2";
                worksheet.Cells[1, 10].Value = "规格3";
                worksheet.Cells[1, 11].Value = "采购数量";
                worksheet.Cells[1, 12].Value = "采购单位";
                worksheet.Cells[1, 13].Value = "单价";
                worksheet.Cells[1, 14].Value = "总价";
                worksheet.Cells[1, 15].Value = "发运日期";
                worksheet.Cells[1, 16].Value = "供应商名称";
                worksheet.Cells[1, 17].Value = "制造商";
                worksheet.Cells[1, 18].Value = "原产国";
                worksheet.Cells[1, 19].Value = "炉批号";
                worksheet.Cells[1, 20].Value = "运单号";
                worksheet.Cells[1, 21].Value = "账册号";
                worksheet.Cells[1, 22].Value = "账册项号";
                worksheet.Cells[1, 23].Value = "申报单位";
                worksheet.Cells[1, 24].Value = "法定单位1";
                worksheet.Cells[1, 25].Value = "法定单位2";
                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 27; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }

                //添加值
                for (int i = 0; i <= list.Count - 1; i++)
                {
                    if (list[i].Buyer.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 1].Value = list[i].Buyer.ToString();
                    }
                    if (list[i].OrderNo.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 2].Value = list[i].OrderNo.ToString();
                    }
                    if (list[i].ReferenceNo.ToString() != null)
                    { worksheet.Cells[i + 2, 3].Value = list[i].ReferenceNo.ToString(); }
                    if (list[i].MaterialCode.ToString() != null)
                    { worksheet.Cells[i + 2, 4].Value = list[i].MaterialCode.ToString(); }
                    if (list[i].Description.ToString() != null)
                    { worksheet.Cells[i + 2, 5].Value = list[i].Description.ToString(); }
                    if (list[i].Type.ToString() != null)
                    { worksheet.Cells[i + 2, 6].Value = list[i].Type.ToString(); }
                    if (list[i].Specification.ToString() != null)
                    { worksheet.Cells[i + 2, 7].Value = list[i].Specification.ToString(); }
                    if (list[i].Thickness.ToString() != null)
                    { worksheet.Cells[i + 2, 8].Value = Convert.ToDouble(list[i].Thickness); }
                    if (list[i].Length.ToString() != null)
                    { worksheet.Cells[i + 2, 9].Value = Convert.ToDouble(list[i].Length.ToString()); }
                    if (list[i].Width.ToString() != null)
                    { worksheet.Cells[i + 2, 10].Value = Convert.ToDouble(list[i].Width.ToString()); }
                    if (list[i].PurchaseQuantity.ToString() != null)
                    { worksheet.Cells[i + 2, 11].Value = list[i].PurchaseQuantity.ToString(); }
                    if (list[i].PurchaseUnit.ToString() != null)
                    { worksheet.Cells[i + 2, 12].Value = list[i].PurchaseUnit.ToString(); }
                    if (list[i].UnitPrice.ToString() != null)
                    { worksheet.Cells[i + 2, 13].Value = list[i].UnitPrice.ToString(); }
                    if (list[i].TotalPrice.ToString() != null)
                    { worksheet.Cells[i + 2, 14].Value = list[i].TotalPrice.ToString(); }
                    if (list[i].ShipmentDate != null)
                    { worksheet.Cells[i + 2, 15].Value = list[i].ShipmentDate.ToString(); }
                    if (list[i].Consignor.ToString() != null)
                    { worksheet.Cells[i + 2, 16].Value = list[i].Consignor.ToString(); }
                    if (list[i].Manufacturers.ToString() != null)
                    { worksheet.Cells[i + 2, 17].Value = list[i].Manufacturers.ToString(); }
                    if (list[i].OriginCountry.ToString() != null)
                    { worksheet.Cells[i + 2, 18].Value = list[i].OriginCountry.ToString(); }
                    if (list[i].BatchNo.ToString() != null)
                    { worksheet.Cells[i + 2, 19].Value = list[i].BatchNo.ToString(); }
                    if (list[i].Waybill.ToString() != null)
                    { worksheet.Cells[i + 2, 20].Value = list[i].Waybill.ToString(); }
                    if (list[i].Books.ToString() != null)
                    { worksheet.Cells[i + 2, 21].Value = list[i].Books.ToString(); }
                    if (list[i].BooksItem.ToString() != null)
                    { worksheet.Cells[i + 2, 22].Value = list[i].BooksItem.ToString(); }
                    if (list[i].RecordUnit.ToString() != null)
                    { worksheet.Cells[i + 2, 23].Value = list[i].RecordUnit.ToString(); }
                    if (list[i].RecordUnitReducedPrice.ToString() != null)
                    { worksheet.Cells[i + 2, 24].Value = list[i].RecordUnitReducedPrice.ToString(); }
                    if (list[i].LegalUnits.ToString() != null)
                    { worksheet.Cells[i + 2, 25].Value = list[i].LegalUnits.ToString(); }

                }
                package.Save();
            }
            return File("\\Files\\ejdfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        [Route("excelConfirmedCustoms", Name = "excelConfirmedCustoms")]
        [Function("核注清单生成", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsIndex")]
        public IActionResult Export(List<int> checkboxId)
        {
            ViewBag.Import = HttpContext.Session.GetInt32("import");
            var main = _importTrans_main_recordService.getById(ViewBag.Import);
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string f = Guid.NewGuid().ToString("N");
            string sFileName = "核注清单" + main.PoNo + "+" + main.Mbl + ".xlsx";
            string sFileNamea = "核注清单" + main.PoNo + "+" + main.Mbl +f+ ".xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\ejdfile\\", sFileNamea));
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("核注清单");
                //添加头
                worksheet.Cells[1, 1].Value = "物料代码";
                worksheet.Cells[1, 2].Value = "品名";
                worksheet.Cells[1, 3].Value = "牌号";
                worksheet.Cells[1, 4].Value = "规范";
                worksheet.Cells[1, 5].Value = "规格1";
                worksheet.Cells[1, 6].Value = "规格2";
                worksheet.Cells[1, 7].Value = "规格3";
                worksheet.Cells[1, 8].Value = "采购数量";
                worksheet.Cells[1, 9].Value = "采购单位";
                worksheet.Cells[1, 10].Value = "单价";
                worksheet.Cells[1, 11].Value = "总价";
                worksheet.Cells[1, 12].Value = "序号";
                worksheet.Cells[1, 13].Value = "商品料号";
                worksheet.Cells[1, 14].Value = "报关单商品序号";
                worksheet.Cells[1, 15].Value = "流转申报表序号";
                worksheet.Cells[1, 16].Value = "原产国（地区）";
                worksheet.Cells[1, 17].Value = "申报单价";
                worksheet.Cells[1, 18].Value = "申报总价";
                worksheet.Cells[1, 19].Value = "币制";
                worksheet.Cells[1, 20].Value = "申报数量";
                worksheet.Cells[1, 21].Value = "法定数量";
                worksheet.Cells[1, 22].Value = "第二数量";
                worksheet.Cells[1, 23].Value = "第一比例因子";
                worksheet.Cells[1, 24].Value = "第二比例因子";
                worksheet.Cells[1, 25].Value = "重量比例因子";
                worksheet.Cells[1, 26].Value = "毛重";
                worksheet.Cells[1, 27].Value = "净重";
                worksheet.Cells[1, 28].Value = "征免方式";
                worksheet.Cells[1, 29].Value = "用途";
                worksheet.Cells[1, 30].Value = "单耗版本号";
                worksheet.Cells[1, 31].Value = "备注";
                worksheet.Cells[1, 32].Value = "最终目的国（地区）";
                worksheet.Cells[1, 33].Value = "修改标志";
                worksheet.Cells[1, 34].Value = "备案序号";
                int i = 0;
                foreach (int u in checkboxId)
                {
                    var modelSch = _scheduleService.getById(u);


                    if (modelSch.MaterialCode.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 1].Value = modelSch.MaterialCode.ToString();
                    }
                    if (modelSch.Description.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 2].Value = modelSch.Description.ToString();
                    }
                    if (modelSch.Type.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 3].Value = modelSch.Type.ToString();
                    }
                    if (modelSch.Specification.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 4].Value = modelSch.Specification.ToString();
                    }
                    if (modelSch.Thickness.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 5].Value = Convert.ToDouble(modelSch.Thickness);
                    }
                    if (modelSch.Length.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 6].Value = Convert.ToDouble(modelSch.Length.ToString());
                    }
                    if (modelSch.Width.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 7].Value = Convert.ToDouble(modelSch.Width.ToString());
                    }
                    if (modelSch.PurchaseQuantity.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 8].Value = modelSch.PurchaseQuantity.ToString();
                    }
                    if (modelSch.PurchaseUnit.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 9].Value = modelSch.PurchaseUnit.ToString();
                    }
                    if (modelSch.UnitPrice.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 10].Value = modelSch.UnitPrice.ToString();
                    }
                    if (modelSch.TotalPrice.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 11].Value = modelSch.TotalPrice.ToString();
                    }
                    if (i+1>0)
                    {
                        worksheet.Cells[i + 2, 12].Value = i+1;
                    }
                    if (modelSch.OriginCountry.ToString() != null)
                    {
                        worksheet.Cells[i + 2, 16].Value = modelSch.OriginCountry.ToString();
                    }
                    i++;

                    package.Save();
                }
                return File("\\Files\\ejdfile\\" + sFileNamea, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
            }
        }
        //[HttpPost]
        //[Route("importConfirmedCustoms", Name = "importConfirmedCustoms")]
        //public ActionResult Import(IFormFile excelfile, Entities.ImportTrans_main_record model, string returnUrl = null)
        //{
        //    ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itConfirmedCustoms");
        //    string sWebRootFolder = _hostingEnvironment.WebRootPath;
        //    var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
        //    string sFileName = $"{Guid.NewGuid()}.xlsx";
        //    FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
        //    using (FileStream fs = new FileStream(file.ToString(), FileMode.Create))
        //    {
        //        excelfile.CopyTo(fs);
        //        fs.Flush();
        //    }
        //    using (ExcelPackage package = new ExcelPackage(file))
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
        //        int rowCount = worksheet.Dimension.Rows;
        //        int ColCount = worksheet.Dimension.Columns;
        //        for (int row = 2; row <= rowCount; row++)
        //        {

        //            model.Itemno = worksheet.Cells[row, 1].Value.ToString();
        //            model.Shipper = worksheet.Cells[row, 2].Value.ToString();
        //            model.PoNo = worksheet.Cells[row, 3].Value.ToString();
        //            if (model.PoNo != null)
        //            {
        //                model.Buyer = model.PoNo.Substring(1, 2);
        //            }
        //            model.Incoterms = worksheet.Cells[row, 4].Value.ToString();
        //            model.CargoType = worksheet.Cells[row, 5].Value.ToString();
        //            model.Invamou = worksheet.Cells[row, 6].Value.ToString();
        //            model.Invcurr = worksheet.Cells[row, 7].Value.ToString();
        //            model.CreationTime = DateTime.Now;
        //            model.Creator = WorkContext.CurrentUser.Id;
        //            try
        //            {
        //            }
        //            catch (Exception e)
        //            {
        //            }
        //            _importTrans_main_recordService.insertImportTransmain(model);
        //        }
        //        return Redirect(ViewBag.ReturnUrl);
        //    }
        //}
        [HttpPost]
        [Route("itConfirmedCustomsList", Name = "itConfirmedCustomsList")]
        [Function("综保报关行数据填写", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsIndex")]
        public ActionResult ITConfirmedCustomsList(string kevin)
        {
            string test = kevin;
            List<Entities.ImportTrans_main_record> jsonlist = JsonHelper.DeserializeJsonToList<Entities.ImportTrans_main_record>(test);
            try
            {
                foreach (Entities.ImportTrans_main_record u in jsonlist)
                {
                    var model = _importTrans_main_recordService.getById(u.Id);
                    model.InventoryNo = u.InventoryNo;
                    if (u.CheckAndPass != "") { model.CheckAndPass = u.CheckAndPass; }
                    _importTrans_main_recordService.updateImportTransmain(model);
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
        [HttpGet]
        [Route("edit", Name = "editITConfirmedCustoms")]
        [Function("综保报关行编辑", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsIndex")]
        public IActionResult EditITConfirmedCustoms(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itConfirmedCustoms");
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Invcurrlist"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
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
        [Route("edit")]
        public ActionResult EditITConfirmedCustoms(Entities.ImportTrans_main_record model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itConfirmedCustoms");
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
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _importTrans_main_recordService.updateImportTransmain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit2", Name = "editConfirmedCustomsSchedule")]
        [Function("综保报关行编辑明细表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITConfirmedCustomsController.ITConfirmedCustomsScheduleIndex")]
        public IActionResult EditConfirmedCustomsSchedule(int? id, string returnUrl = null)
        {//页面跳转未完成
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itConfirmedCustoms");
            if (id != null)
            {
                var model = _scheduleService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit2")]
        public ActionResult EditConfirmedCustomsSchedule(Entities.Schedule model, string returnUrl = null)
        {//页面跳转未完成
            ModelState.Remove("Id");
            int a = 0;
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itConfirmedCustoms");
            if (!ModelState.IsValid)
                return View(model);
            //if (!String.IsNullOrEmpty(model.InvoiceNo))
            //     model.InvoiceNo = model.InvoiceNo.Trim();
            // if (!String.IsNullOrEmpty(model.MaterielNo))
            //    model.MaterielNo = model.MaterielNo.Trim();
            //if (!String.IsNullOrEmpty(model.PurchasingDocuments))
            //     model.PurchasingDocuments = model.PurchasingDocuments.Trim();
            if (model.Id.Equals(0))
            {
                model.CreationTime = DateTime.Now;
                model.IsDeleted = false;
                model.Modifier = null;
                model.ModifiedTime = null;
                model.Creator = WorkContext.CurrentUser.Id;
                _scheduleService.insertSchedule(model);
            }
            else
            {
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _scheduleService.updateSchedule(model);
            }
            int mid = model.MainId;
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}