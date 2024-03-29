﻿using System;
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
using General.Services.Material;
using General.Services.Project;
using General.Services.OrderMain;
using General.Services.Supplier;

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
        private ISysUserService _sysUserService;
        private IOrderMainService _sysOrderMainService;
        private IMaterialService _sysMaterialService;
        private IProjectService _sysProjectService;
        private ISysCustomizedListService _sysCustomizedListService;
        private ISupplierService _sysSupplierService;
        public ITOrderImportController(ISysUserService sysUserService, ISupplierService sysSupplierService, IOrderMainService sysOrderMainService, IProjectService sysProjectService, IMaterialService sysMaterialService, ISysCustomizedListService sysCustomizedListService, IOrderService sysOrderService, IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
        {
            this._sysSupplierService = sysSupplierService;
            this._sysCustomizedListService = sysCustomizedListService;
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysRoleService = sysRoleService;
            this._sysUserService = sysUserService;
            this._sysProjectService = sysProjectService;
            this._sysMaterialService = sysMaterialService;
            this._sysOrderService = sysOrderService;
            this._sysOrderMainService = sysOrderMainService;
            this._hostingEnvironment = hostingEnvironment;
        }
        [Route("itOrderImportIndex", Name = "itOrderImportIndex")]
        [Function("采购订单详细", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public IActionResult ITOrderImportIndex(int id, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            string s;
            if (id != 0)
            {
                Response.Cookies.Append("Order", Convert.ToString(id));
                s = Convert.ToString(id);
            }
            else
            {
                Request.Cookies.TryGetValue("Order", out s);
            }
            int ida = Convert.ToInt32(s);
            RolePermissionViewModel model = new RolePermissionViewModel();
            var pageList = _sysOrderService.searchOrder(arg, page, size, ida);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Order, SysCustomizedListSearchArg>("itOrderImportIndex", arg);
            return View(dataSource);//sysImport
        }
        [Route("itOrderImportDelete", Name = "itOrderImportDelete")]
        [Function("采购订单删除", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public IActionResult ItOrderImportDelete(int id)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(null) ? null : Url.RouteUrl("itOrderImportMainIndex");
            var model = _sysOrderMainService.getById(id);
            model.IsDeleted = true;
            _sysOrderMainService.updateOrderMain(model);
            return Redirect(ViewBag.ReturnUrl);

        }
        [Route("", Name = "itOrderImportMainIndex")]
        [Function("采购订单", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.DataImportController", Sort = 1)]
        public IActionResult ITOrderImportMainIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _sysOrderMainService.searchOrderMain(arg, page, size, WorkContext.CurrentUser.RoleName, WorkContext.CurrentUser.Name);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.OrderMain, SysCustomizedListSearchArg>("itOrderImportMainIndex", arg);
            return View(dataSource);//sysImport
        }

        [HttpGet]
        [Route("edit2", Name = "editITOrderImport")]
        [Function("编辑采购订单", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public IActionResult EditITOrderImport(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("editITOrderImport");

            if (id != null)
            {
                ViewBag.fw = 1;
                var model = _sysOrderService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit2")]
        public ActionResult EditITOrderImport(Entities.Order model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itOrderImportIndex");
            if (!ModelState.IsValid)
                return View(model);
            if (model.Id.Equals(0))
            {
            }
            else
            {
                var modela = _sysOrderService.getById(model.Id);
                modela.Item = model.Item;
                modela.PlanItem = model.PlanItem;
                modela.MaterialCode = model.MaterialCode;
                modela.Name = model.Name;
                modela.PartNo = model.PartNo;
                modela.Specification = model.Specification;
                modela.Size = model.Size;
                modela.Width = model.Width;
                modela.Length = model.Length;
                modela.Package = model.Package;
                modela.Qty = model.Qty;
                modela.Unit = model.Unit;
                modela.Currency = model.Currency;
                modela.UnitPrice = model.UnitPrice;
                modela.TotalPrice = model.TotalPrice;
                modela.LeadTime = model.LeadTime;
                modela.Manufacturer = model.Manufacturer;
                modela.Origin = model.Origin;
                modela.TotalPrice = model.TotalPrice;
                modela.PlanUnit = model.PlanUnit;
                try
                {
                    modela.Reduced = Convert.ToDouble(model.Reduced);
                }
                catch (Exception e)
                {
                    Response.WriteAsync("<script>alert('折算关系必须为数字!');window.location.href ='edit2'</script>", Encoding.GetEncoding("GB2312"));
                    return Redirect(ViewBag.ReturnUrl);
                }
                modela.Modifier = WorkContext.CurrentUser.Id;
                modela.ModifiedTime = DateTime.Now;
                _sysOrderService.updateOrder(modela);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpGet]
        [Route("edit", Name = "editITOrderImportMain")]
        [Function("编辑采购订单主表", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public IActionResult EditITOrderImportMain(int? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itOrderImportMainIndex");
            var customizedList = _sysCustomizedListService.getByAccount("付款方式");
            ViewData["Payment"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            var customizedList2 = _sysUserService.getTran();
            ViewData["Transport"] = new SelectList(customizedList2, "Transport", "Transport");
            var customizedList3 = _sysCustomizedListService.getByAccount("贸易条款");
            ViewData["TradeTerms"] = new SelectList(customizedList3, "CustomizedValue", "CustomizedValue");
            var customizedList4 = _sysCustomizedListService.getByAccount("运输方式");
            ViewData["Ysfs"] = new SelectList(customizedList4, "CustomizedValue", "CustomizedValue");
            ViewBag.Person = WorkContext.CurrentUser.Name;
            ViewBag.Card = WorkContext.CurrentUser.Account;
            if (id != null)
            {
                ViewBag.Id = id;
                var model = _sysOrderMainService.getById(id.Value);
                ViewBag.tran = model.Transport;
                ViewBag.pay = model.Payment;
                ViewBag.trade = model.TradeTerms;
                ViewBag.fs = model.Transportion;
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        //[Route("excelimport", Name = "excelimport")]
        //public FileStreamResult Excel(int?id)
        //{
        //    var model = _sysOrderService.getById(id.Value);
        //    string sWebRootFolder = _hostingEnvironment.WebRootPath;
        //    var fileProfile = sWebRootFolder + "\\Files\\profile\\";
        //    string sFileName = model.Attachment;
        //    FileInfo file = new FileInfo(Path.Combine(fileProfile, sFileName));
        //    FileStream fs = new FileStream(file.ToString(), FileMode.Create);
        //    return File(fs,"application/octet-stream", sFileName);
        //}
        [Route("excelinsertorder", Name = "excelinsertorder")]
        [Function("采购订单导入模板", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public IActionResult Export3()
        {
            string sWebRootFolder = _hostingEnvironment.WebRootPath;
            string sFileName = "采购订单导入模板" + $"{DateTime.Now.ToString("yyMMdd")}.xlsx";
            FileInfo file = new FileInfo(Path.Combine(sWebRootFolder + "\\Files\\importfile\\", sFileName));
            file.Delete();
            using (ExcelPackage package = new ExcelPackage(file))
            {
                // 添加worksheet
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("sheet1");
                //添加头 

                worksheet.Cells[1, 1].Value = "订单行号";
                worksheet.Cells[1, 2].Value = "索引号";
                worksheet.Cells[1, 3].Value = "物料代码";
                worksheet.Cells[1, 4].Value = "名称";
                worksheet.Cells[1, 5].Value = "牌号";
                worksheet.Cells[1, 6].Value = "规范";
                worksheet.Cells[1, 7].Value = "规格";
                worksheet.Cells[1, 8].Value = "宽";
                worksheet.Cells[1, 9].Value = "长";
                worksheet.Cells[1, 10].Value = "包装规格";
                worksheet.Cells[1, 11].Value = "订单数量";
                worksheet.Cells[1, 12].Value = "订单单位";
                worksheet.Cells[1, 13].Value = "币种";
                worksheet.Cells[1, 14].Value = "单价";
                worksheet.Cells[1, 15].Value = "总价";
                worksheet.Cells[1, 16].Value = "交货日期";
                worksheet.Cells[1, 17].Value = "原产国";
                worksheet.Cells[1, 18].Value = "制造商";
                worksheet.Cells[1, 19].Value = "计划单位";
                worksheet.Cells[1, 20].Value = "折算关系";

                // xlSheet1.Range("A2:E2").Borders.LineStyle = 1
                for (int a = 1; a <= 22; a++)
                {
                    worksheet.Cells[1, a].Style.Font.Bold = true;
                }
                //添加值


                package.Save();
            }
            return File("\\Files\\importfile\\" + sFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", sFileName);
        }
        [HttpPost]
        [Route("editorder", Name = "editorder")]
        [Function("采购订单重传", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public ActionResult ImportOrder( IFormFile excelfile, IFormFile excelfilecr, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itOrderImportIndex");
            string s;
            Request.Cookies.TryGetValue("Order", out s);
            int ida = Convert.ToInt32(s);
            string submit = Request.Form["submit"];

          
            if (submit == "订单明细重传")
            {
                if (excelfile == null)
                {
                    Response.WriteAsync("<script>alert('未添加导入模板!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                    return Redirect(ViewBag.ReturnUrl);
                }
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                try
                {
                    var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
                    string sFileName = Guid.NewGuid().ToString() + excelfile.FileName;
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
                        int materialno = 0; int partno = 0; int technical = 0; int item = 0; int lineitem = 0;
                        int width = 0; int name = 0; int size = 0;
                        int length = 0; int orderno = 0; int orderunit = 0; int packages = 0; int currency = 0; int unitprice = 0;
                        int totamount = 0; int deliverydate = 0; int manufacturer = 0; int origin = 0; int planunit = 0; int reduced = 0;
                        for (int columns = 1; columns <= ColCount; columns++)
                        {
                            //Entities.Order model = new Entities.Order();
                            if (worksheet.Cells[1, columns].Value != null)
                            {
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单行号")) { item = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("索引号")) { lineitem = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("物料代码")) { materialno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("名称")) { name = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("牌号")) { partno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("规范")) { technical = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("规格")) { size = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("宽")) { width = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("长")) { length = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("包装规格")) { packages = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单数量")) { orderno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单单位")) { orderunit = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("币种")) { currency = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("单价")) { unitprice = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("总价")) { totamount = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("交货日期")) { deliverydate = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("制造商")) { manufacturer = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("原产国")) { origin = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("计划单位")) { planunit = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("折算关系")) { reduced = columns; }
                            }
                        }
                        var oder = _sysOrderService.getMain(ida);
                        foreach (Order u in oder)
                        {
                            var odera = _sysOrderService.getById(u.Id);
                            odera.IsDeleted = true;
                            _sysOrderService.updateOrder(odera);
                        }
                        var main = _sysOrderMainService.getById(ida);
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                if (lineitem > 0)
                                {
                                    if (worksheet.Cells[row, lineitem].Value == null)
                                    {

                                        Response.WriteAsync("<script>alert('excel第'" + row + "'行数据，索引号不能为空!');window.location.href ='editITOrderImportMain'</script>", Encoding.GetEncoding("GB2312"));
                                    }

                                }
                                int a = 0;
                                Entities.Order model = new Entities.Order();
                                model.MainId = main.Id;
                                model.OrderNo = main.OrderNo;
                                model.OrderConfirmDate = main.OrderConfirmDate;
                                model.OrderSigner = main.OrderSigner;
                                model.SignerCard = main.SignerCard;
                                model.SupplierCode = main.SupplierCode;
                                model.SupplierName = main.SupplierName;
                                model.TradeTerms = main.TradeTerms;
                                model.Transport = main.Transport;
                                //var modelproject = _sysProjectService.getByAccount(modelplan.OrderNo.Substring(0, 1));
                                model.Project = main.Project;
                                //var modelMaterial = _sysMaterialService.getByAccount(modelplan.OrderNo.Substring(3, 1));
                                model.MaterialCategory = main.MaterialCategory;
                                if (item > 0 && worksheet.Cells[row, item].Value != null)
                                {
                                    model.Item = worksheet.Cells[row, item].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (materialno > 0 && worksheet.Cells[row, materialno].Value != null)
                                {
                                    model.MaterialCode = worksheet.Cells[row, materialno].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (name > 0 && worksheet.Cells[row, name].Value != null)
                                {
                                    model.Name = worksheet.Cells[row, name].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (lineitem > 0 && worksheet.Cells[row, lineitem].Value != null)
                                {
                                    model.PlanItem = worksheet.Cells[row, lineitem].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (technical > 0 && worksheet.Cells[row, technical].Value != null)
                                {
                                    model.Specification = worksheet.Cells[row, technical].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (width > 0 && worksheet.Cells[row, width].Value != null)
                                {
                                    model.Width = worksheet.Cells[row, width].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (size > 0 && worksheet.Cells[row, size].Value != null)
                                {
                                    model.Size = worksheet.Cells[row, size].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (length > 0 && worksheet.Cells[row, length].Value != null)
                                {
                                    model.Length = worksheet.Cells[row, length].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (packages > 0 && worksheet.Cells[row, packages].Value != null)
                                {
                                    model.Package = worksheet.Cells[row, packages].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    int number = Convert.ToInt32(worksheet.Cells[row, orderno].Value);
                                    if (orderno > 0 && worksheet.Cells[row, orderno].Value != null)
                                    {
                                        model.Qty = worksheet.Cells[row, orderno].Value.ToString();
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {

                                    Response.WriteAsync("<script>alert('采购数量填写错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }

                                if (orderunit > 0 && worksheet.Cells[row, orderunit].Value != null)
                                {
                                    model.Unit = worksheet.Cells[row, orderunit].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (unitprice > 0 && worksheet.Cells[row, unitprice].Value != null)
                                {
                                    model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (totamount > 0 && worksheet.Cells[row, totamount].Value != null)
                                {
                                    model.TotalPrice = worksheet.Cells[row, totamount].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (currency > 0 && worksheet.Cells[row, currency].Value != null)
                                {
                                    model.Currency = worksheet.Cells[row, currency].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    if (deliverydate > 0 && worksheet.Cells[row, deliverydate].Value != null)
                                    {
                                        model.LeadTime = Convert.ToDateTime(worksheet.Cells[row, deliverydate].Value.ToString());
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {

                                    Response.WriteAsync("<script>alert('日期格式错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }

                                if (manufacturer > 0 && worksheet.Cells[row, manufacturer].Value != null)
                                {
                                    model.Manufacturer = worksheet.Cells[row, manufacturer].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (origin > 0 && worksheet.Cells[row, origin].Value != null)
                                {
                                    model.Origin = worksheet.Cells[row, origin].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (partno > 0 && worksheet.Cells[row, partno].Value != null)
                                {
                                    model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (planunit > 0 && worksheet.Cells[row, planunit].Value != null)
                                {
                                    model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    if (reduced > 0 && worksheet.Cells[row, reduced].Value != null)
                                    {
                                        model.Reduced = Convert.ToDouble(worksheet.Cells[row, reduced].Value);
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {

                                    Response.WriteAsync("<script>alert('折算关系填写错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }

                                model.CreationTime = DateTime.Now;
                                model.Creator = WorkContext.CurrentUser.Id;
                                if (a != 20)
                                {
                                    _sysOrderService.insertOrder(model);
                                }

                            }
                            catch (Exception e)
                            {
                                if (e.Message == "String was not recognized as a valid DateTime.")
                                {
                                    Response.WriteAsync("<script>alert('日期格式错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                                if (e.Message == "Input string was not in a correct format.")
                                {
                                    Response.WriteAsync("<script>alert('计划单位或折算关系错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Response.WriteAsync("<script>alert('请将文件名中的空格或特殊字符去掉!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                }
            }
            else if (submit == "订单明细插入")
            {
                if (excelfile == null)
                {
                    Response.WriteAsync("<script>alert('未添加导入模板!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                    return Redirect(ViewBag.ReturnUrl);
                }
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                try
                {
                    var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
                    string sFileName = Guid.NewGuid().ToString() + excelfile.FileName;
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
                        int materialno = 0; int partno = 0; int technical = 0; int item = 0; int lineitem = 0;
                        int width = 0; int name = 0; int size = 0;
                        int length = 0; int orderno = 0; int orderunit = 0; int packages = 0; int currency = 0; int unitprice = 0;
                        int totamount = 0; int deliverydate = 0; int manufacturer = 0; int origin = 0; int planunit = 0; int reduced = 0;
                        for (int columns = 1; columns <= ColCount; columns++)
                        {
                            //Entities.Order model = new Entities.Order();
                            if (worksheet.Cells[1, columns].Value != null)
                            {
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单行号")) { item = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("索引号")) { lineitem = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("物料代码")) { materialno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("名称")) { name = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("牌号")) { partno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("规范")) { technical = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("规格")) { size = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("宽")) { width = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("长")) { length = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("包装规格")) { packages = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单数量")) { orderno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单单位")) { orderunit = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("币种")) { currency = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("单价")) { unitprice = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("总价")) { totamount = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("交货日期")) { deliverydate = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("制造商")) { manufacturer = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("原产国")) { origin = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("计划单位")) { planunit = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("折算关系")) { reduced = columns; }
                            }
                        }
                       
                        var main = _sysOrderMainService.getById(ida);
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                if (lineitem > 0)
                                {
                                    if (worksheet.Cells[row, lineitem].Value == null)
                                    {

                                        Response.WriteAsync("<script>alert('excel第'" + row + "'行数据，索引号不能为空!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                    }

                                }
                                int a = 0;
                                Entities.Order model = new Entities.Order();
                                model.MainId = main.Id;
                                model.OrderNo = main.OrderNo;
                                model.OrderConfirmDate = main.OrderConfirmDate;
                                model.OrderSigner = main.OrderSigner;
                                model.SignerCard = main.SignerCard;
                                model.SupplierCode = main.SupplierCode;
                                model.SupplierName = main.SupplierName;
                                model.TradeTerms = main.TradeTerms;
                                model.Transport = main.Transport;
                                //var modelproject = _sysProjectService.getByAccount(modelplan.OrderNo.Substring(0, 1));
                                model.Project = main.Project;
                                //var modelMaterial = _sysMaterialService.getByAccount(modelplan.OrderNo.Substring(3, 1));
                                model.MaterialCategory = main.MaterialCategory;
                                if (item > 0 && worksheet.Cells[row, item].Value != null)
                                {
                                    model.Item = worksheet.Cells[row, item].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (materialno > 0 && worksheet.Cells[row, materialno].Value != null)
                                {
                                    model.MaterialCode = worksheet.Cells[row, materialno].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (name > 0 && worksheet.Cells[row, name].Value != null)
                                {
                                    model.Name = worksheet.Cells[row, name].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (lineitem > 0 && worksheet.Cells[row, lineitem].Value != null)
                                {
                                    model.PlanItem = worksheet.Cells[row, lineitem].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (technical > 0 && worksheet.Cells[row, technical].Value != null)
                                {
                                    model.Specification = worksheet.Cells[row, technical].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (width > 0 && worksheet.Cells[row, width].Value != null)
                                {
                                    model.Width = worksheet.Cells[row, width].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (size > 0 && worksheet.Cells[row, size].Value != null)
                                {
                                    model.Size = worksheet.Cells[row, size].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (length > 0 && worksheet.Cells[row, length].Value != null)
                                {
                                    model.Length = worksheet.Cells[row, length].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (packages > 0 && worksheet.Cells[row, packages].Value != null)
                                {
                                    model.Package = worksheet.Cells[row, packages].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    int number = Convert.ToInt32(worksheet.Cells[row, orderno].Value);
                                    if (orderno > 0 && worksheet.Cells[row, orderno].Value != null)
                                    {
                                        model.Qty = worksheet.Cells[row, orderno].Value.ToString();
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {

                                    Response.WriteAsync("<script>alert('采购数量填写错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }

                                if (orderunit > 0 && worksheet.Cells[row, orderunit].Value != null)
                                {
                                    model.Unit = worksheet.Cells[row, orderunit].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (unitprice > 0 && worksheet.Cells[row, unitprice].Value != null)
                                {
                                    model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (totamount > 0 && worksheet.Cells[row, totamount].Value != null)
                                {
                                    model.TotalPrice = worksheet.Cells[row, totamount].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (currency > 0 && worksheet.Cells[row, currency].Value != null)
                                {
                                    model.Currency = worksheet.Cells[row, currency].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    if (deliverydate > 0 && worksheet.Cells[row, deliverydate].Value != null)
                                    {
                                        model.LeadTime = Convert.ToDateTime(worksheet.Cells[row, deliverydate].Value.ToString());
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {

                                    Response.WriteAsync("<script>alert('日期格式错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }

                                if (manufacturer > 0 && worksheet.Cells[row, manufacturer].Value != null)
                                {
                                    model.Manufacturer = worksheet.Cells[row, manufacturer].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (origin > 0 && worksheet.Cells[row, origin].Value != null)
                                {
                                    model.Origin = worksheet.Cells[row, origin].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (partno > 0 && worksheet.Cells[row, partno].Value != null)
                                {
                                    model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (planunit > 0 && worksheet.Cells[row, planunit].Value != null)
                                {
                                    model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    if (reduced > 0 && worksheet.Cells[row, reduced].Value != null)
                                    {
                                        model.Reduced = Convert.ToDouble(worksheet.Cells[row, reduced].Value);
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {

                                    Response.WriteAsync("<script>alert('折算关系填写错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }

                                model.CreationTime = DateTime.Now;
                                model.Creator = WorkContext.CurrentUser.Id;
                                if (a != 20)
                                {
                                    _sysOrderService.insertOrder(model);
                                }

                            }
                            catch (Exception e)
                            {
                                if (e.Message == "String was not recognized as a valid DateTime.")
                                {
                                    Response.WriteAsync("<script>alert('日期格式错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                                if (e.Message == "Input string was not in a correct format.")
                                {
                                    Response.WriteAsync("<script>alert('计划单位或折算关系错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Response.WriteAsync("<script>alert('请将文件名中的空格或特殊字符去掉!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                }
            }
               
            
           
            return Redirect(ViewBag.ReturnUrl);
        }
        [HttpPost]
        [Route("edit", Name = "importexcelorder")]
        [Function("采购订单修改、批量导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public ActionResult Import(Entities.OrderMain modelplan, IFormFile excelfile, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itOrderImportMainIndex");
            if (modelplan.Id.Equals(0))
            {
                if (excelfile == null)
                {
                    Response.WriteAsync("<script>alert('未添加导入模板!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                    return Redirect(ViewBag.ReturnUrl);
                }
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                try
                {
                    var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
                    string sFileName = Guid.NewGuid().ToString() + excelfile.FileName;
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
                        int materialno = 0; int partno = 0; int technical = 0; int item = 0; int lineitem = 0;
                        int width = 0; int name = 0; int size = 0;
                        int length = 0; int orderno = 0; int orderunit = 0; int packages = 0; int currency = 0; int unitprice = 0;
                        int totamount = 0; int deliverydate = 0; int manufacturer = 0; int origin = 0; int planunit = 0; int reduced = 0;
                        for (int columns = 1; columns <= ColCount; columns++)
                        {
                            //Entities.Order model = new Entities.Order();
                            if (worksheet.Cells[1, columns].Value != null)
                            {
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单行号")) { item = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("索引号") ) { lineitem = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("物料代码") ) { materialno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("名称") ) { name = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("牌号")) { partno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("规范") ) { technical = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("规格") ) { size = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("宽") ) { width = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("长"))  { length = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("包装规格")) { packages = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单数量")) { orderno = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("订单单位")) { orderunit = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("币种")) { currency = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("单价")) { unitprice = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("总价")) { totamount = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("交货日期")) { deliverydate = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("制造商")) { manufacturer = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("原产国")) { origin = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("计划单位")) { planunit = columns; }
                                if (worksheet.Cells[1, columns].Value.ToString().Contains("折算关系")) { reduced = columns; }
                            }
                        }
                        Entities.OrderMain modelmain = new Entities.OrderMain();
                        var listmain = _sysOrderMainService.existAccount(modelplan.OrderNo);
                        if (listmain == true)
                        {
                            Response.WriteAsync("<script>alert('采购订单号重复!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                            return Redirect(ViewBag.ReturnUrl);
                        }
                        modelmain.OrderNo = modelplan.OrderNo;
                        modelmain.OrderConfirmDate = modelplan.OrderConfirmDate;
                        modelmain.OrderSigner = modelplan.OrderSigner;
                        modelmain.Transportion = modelplan.Transportion;
                        var sc = _sysUserService.getByName(modelplan.OrderSigner);
                        modelmain.SignerCard = sc.Account;
                        modelmain.SupplierCode = modelplan.SupplierCode;
                        modelmain.Payment = modelplan.Payment;
                        modelmain.CodeNo = modelplan.CodeNo;
                        modelmain.LongDealNo = modelplan.LongDealNo;
                        modelmain.TradeTerms = modelplan.TradeTerms;
                        modelmain.Transport = modelplan.Transport;
                        var modelproject = _sysProjectService.getByAccount(modelplan.OrderNo.Substring(0, 1));
                        if (modelproject != null) { modelmain.Project = modelproject.Describe; }
                        var modelBuyer = _sysUserService.getByBuyer(modelplan.OrderNo.Substring(1, 2));
                        if (modelBuyer != null) { modelmain.Buyer = modelBuyer.Name; }
                        var modelMaterial = _sysMaterialService.getByAccount(modelplan.OrderNo.Substring(3, 1));
                        if (modelMaterial != null) { modelmain.MaterialCategory = modelMaterial.Describe; }
                        var modelSupplier = _sysSupplierService.getByAccount(modelplan.SupplierCode);
                        if (modelSupplier != null) { modelmain.SupplierName = modelSupplier.Describe; }
                        modelmain.CreationTime = DateTime.Now;
                        modelmain.Creator = WorkContext.CurrentUser.Id;
                        _sysOrderMainService.insertOrderMain(modelmain);
                        var main = _sysOrderMainService.getByAccount(modelplan.OrderNo);
                        for (int row = 2; row <= rowCount; row++)
                        {
                            try
                            {
                                if (lineitem > 0 )
                                {
                                    if (worksheet.Cells[row, lineitem].Value == null)
                                    {
                                        main.IsDeleted = true;
                                        _sysOrderMainService.updateOrderMain(main);
                                        string num = Convert.ToString(row);
                                        //Response.WriteAsync("<script>alert('excel第'" + num + "'行数据，索引号不能为空!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                        Response.WriteAsync("<script>alert('索引号不能为空!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                    }

                                }
                                int a = 0;
                                Entities.Order model = new Entities.Order();
                                model.MainId = main.Id;
                                model.OrderNo = main.OrderNo;
                                model.OrderConfirmDate = main.OrderConfirmDate;
                                model.OrderSigner = main.OrderSigner;
                                model.SignerCard = main.SignerCard;
                                model.SupplierCode = main.SupplierCode;
                                model.SupplierName = main.SupplierName;
                                model.TradeTerms = main.TradeTerms;
                                model.Transport = main.Transport;
                                //var modelproject = _sysProjectService.getByAccount(modelplan.OrderNo.Substring(0, 1));
                                model.Project = main.Project;
                                //var modelMaterial = _sysMaterialService.getByAccount(modelplan.OrderNo.Substring(3, 1));
                                model.MaterialCategory = main.MaterialCategory;
                                if (item > 0 && worksheet.Cells[row, item].Value != null)
                                {
                                    model.Item = worksheet.Cells[row, item].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (materialno > 0 && worksheet.Cells[row, materialno].Value != null)
                                {
                                    model.MaterialCode = worksheet.Cells[row, materialno].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (name > 0 && worksheet.Cells[row, name].Value != null)
                                {
                                    model.Name = worksheet.Cells[row, name].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (lineitem > 0 && worksheet.Cells[row, lineitem].Value != null)
                                {
                                    model.PlanItem = worksheet.Cells[row, lineitem].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (technical > 0 && worksheet.Cells[row, technical].Value != null)
                                {
                                    model.Specification = worksheet.Cells[row, technical].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (width > 0 && worksheet.Cells[row, width].Value != null)
                                {
                                    model.Width = worksheet.Cells[row, width].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (size > 0 && worksheet.Cells[row, size].Value != null)
                                {
                                    model.Size = worksheet.Cells[row, size].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (length > 0 && worksheet.Cells[row, length].Value != null)
                                {
                                    model.Length = worksheet.Cells[row, length].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (packages > 0 && worksheet.Cells[row, packages].Value != null)
                                {
                                    model.Package = worksheet.Cells[row, packages].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    int number=  Convert.ToInt32(worksheet.Cells[row, orderno].Value);
                                    if (orderno > 0 && worksheet.Cells[row, orderno].Value != null)
                                    {
                                        model.Qty = worksheet.Cells[row, orderno].Value.ToString();
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {
                                    main.IsDeleted = true;
                                    _sysOrderMainService.updateOrderMain(main);
                                    Response.WriteAsync("<script>alert('采购数量填写错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                               
                                if (orderunit > 0 && worksheet.Cells[row, orderunit].Value != null)
                                {
                                    model.Unit = worksheet.Cells[row, orderunit].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (unitprice > 0 && worksheet.Cells[row, unitprice].Value != null)
                                {
                                    model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (totamount > 0 && worksheet.Cells[row, totamount].Value != null)
                                {
                                    model.TotalPrice = worksheet.Cells[row, totamount].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (currency > 0 && worksheet.Cells[row, currency].Value != null)
                                {
                                    model.Currency = worksheet.Cells[row, currency].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    if (deliverydate > 0 && worksheet.Cells[row, deliverydate].Value != null)
                                    {
                                        model.LeadTime = Convert.ToDateTime(worksheet.Cells[row, deliverydate].Value.ToString());
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {
                                    main.IsDeleted = true;
                                    _sysOrderMainService.updateOrderMain(main);
                                    Response.WriteAsync("<script>alert('日期格式错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                                
                                if (manufacturer > 0 && worksheet.Cells[row, manufacturer].Value != null)
                                {
                                    model.Manufacturer = worksheet.Cells[row, manufacturer].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (origin > 0 && worksheet.Cells[row, origin].Value != null)
                                {
                                    model.Origin = worksheet.Cells[row, origin].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (partno > 0 && worksheet.Cells[row, partno].Value != null)
                                {
                                    model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                if (planunit > 0 && worksheet.Cells[row, planunit].Value != null)
                                {
                                    model.PlanUnit = worksheet.Cells[row, planunit].Value.ToString();
                                }
                                else
                                {
                                    a = a + 1;
                                }
                                try
                                {
                                    if (reduced > 0 && worksheet.Cells[row, reduced].Value != null)
                                    {
                                        model.Reduced = Convert.ToDouble(worksheet.Cells[row, reduced].Value);
                                    }
                                    else
                                    {
                                        a = a + 1;
                                    }
                                }
                                catch
                                {
                                    main.IsDeleted = true;
                                    _sysOrderMainService.updateOrderMain(main);
                                    Response.WriteAsync("<script>alert('折算关系错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                                model.IsDeleted = false;
                                model.CreationTime = DateTime.Now;
                                model.Creator = WorkContext.CurrentUser.Id;
                                if (a!=20)
                                {
                                    _sysOrderService.insertOrder(model);
                                }
                               
                            }
                            catch (Exception e)
                            {

                                main.IsDeleted = true;
                                _sysOrderMainService.updateOrderMain(main);
                                if (e.Message == "String was not recognized as a valid DateTime.")
                                {
                                    Response.WriteAsync("<script>alert('日期格式错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                                if (e.Message == "Input string was not in a correct format.")
                                {
                                    Response.WriteAsync("<script>alert('计划单位或折算关系错误!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Response.WriteAsync("<script>alert('请将文件名中的空格或特殊字符去掉!');window.location.href ='edit'</script>", Encoding.GetEncoding("GB2312"));
                }
            }
            else
            {
                var model = _sysOrderMainService.getById(modelplan.Id);
                model.OrderNo = modelplan.OrderNo;
                model.Transportion = modelplan.Transportion;
                model.OrderConfirmDate = modelplan.OrderConfirmDate;
                model.OrderSigner = modelplan.OrderSigner;
                var sc = _sysUserService.getByName(modelplan.OrderSigner);
                model.SignerCard = sc.Account;
                model.SupplierCode = modelplan.SupplierCode;
                var modelSupplier = _sysSupplierService.getByAccount(modelplan.SupplierCode);
                if (modelSupplier != null) { model.SupplierName = modelSupplier.Describe; }
                model.SupplierName = modelplan.SupplierName;
                model.TradeTerms = modelplan.TradeTerms;
                model.Payment = modelplan.Payment;
                model.LongDealNo = modelplan.LongDealNo;
                model.CodeNo = modelplan.CodeNo;
                model.Transport = modelplan.Transport;
                model.Project = modelplan.Project;
                model.MaterialCategory = modelplan.MaterialCategory;
                model.Modifier = WorkContext.CurrentUser.Id;
                model.ModifiedTime = DateTime.Now;
                _sysOrderMainService.updateOrderMain(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }
    }
}