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
        public ITOrderImportController(ISysUserService sysUserService,ISupplierService sysSupplierService, IOrderMainService sysOrderMainService,IProjectService sysProjectService,IMaterialService sysMaterialService,ISysCustomizedListService sysCustomizedListService, IOrderService sysOrderService,IImportTrans_main_recordService importTrans_main_recordService, IHostingEnvironment hostingEnvironment, ISysRoleService sysRoleService)
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
            var pageList = _sysOrderService.searchOrder(arg, page, size,ida);
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

            var pageList = _sysOrderMainService.searchOrderMain(arg, page, size);
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
            var customizedList2 = _sysUserService.getTran();
            ViewData["Transport"] = new SelectList(customizedList2, "Transport", "Transport");
            ViewBag.Person = WorkContext.CurrentUser.Name;
            ViewBag.Card = WorkContext.CurrentUser.Account;
            if (id != null)
            {
                ViewBag.Id = id;
                var model = _sysOrderMainService.getById(id.Value);
                ViewBag.tran = model.Transport;
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
        [HttpPost]
        [Route("importexcelorder", Name = "importexcelorder")]
        [Function("采购订单修改、批量导入", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.ITOrderImportController.ITOrderImportMainIndex")]
        public ActionResult Import(Entities.OrderMain modelplan, IFormFile excelfile, string excelbh, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("itOrderImportMainIndex");
            if (excelfile!=null) {
                string sWebRootFolder = _hostingEnvironment.WebRootPath;
                var fileProfile = sWebRootFolder + "\\Files\\importfile\\";
                string sFileName = excelfile.FileName;
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
                    int length = 0; int planno = 0; int planunit = 0; int packages = 0; int currency = 0; int unitprice = 0;
                    int totamount = 0; int deliverydate = 0; int manufacturer = 0; int origin = 0;
                    for (int columns = 1; columns <= ColCount; columns++)
                    {
                        //Entities.Order model = new Entities.Order();
                        if (worksheet.Cells[1, columns].Value.ToString() == "订单行号") { item = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "索引号") { lineitem = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "物料代码") { materialno = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "名称") { name = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "牌号") { partno = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "规范") { technical = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "规格") { size = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "宽") { width = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "长") { length = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "包装规格") { packages = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "订单数量") { planno = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "订单单位") { planunit = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "币种") { currency = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "单价") { unitprice = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "总价") { totamount = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "交货日期") { deliverydate = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "制造商") { manufacturer = columns; }
                        if (worksheet.Cells[1, columns].Value.ToString() == "原产国") { origin = columns; }
                    }
                    Entities.OrderMain modelmain = new Entities.OrderMain();
                    var listmain = _sysOrderMainService.existAccount(modelplan.OrderNo);
                    if (listmain == true) { return Redirect(ViewBag.ReturnUrl); }
                    modelmain.OrderNo = modelplan.OrderNo;
                    modelmain.OrderConfirmDate = modelplan.OrderConfirmDate;
                    modelmain.OrderSigner = modelplan.OrderSigner;
                    modelmain.SignerCard = modelplan.SignerCard;
                    modelmain.SupplierCode = modelplan.SupplierCode;
                   // modelmain.SupplierName = modelplan.SupplierName;
                    modelmain.TradeTerms = modelplan.TradeTerms;
                    modelmain.Transport = modelplan.Transport;
                    var modelproject = _sysProjectService.getByAccount(modelplan.OrderNo.Substring(0, 1));
                    modelmain.Project = modelproject.Describe;
                    var modelBuyer = _sysUserService.getByBuyer(modelplan.OrderNo.Substring(1, 2));
                    modelmain.Buyer = modelBuyer.Account;
                    var modelMaterial = _sysMaterialService.getByAccount(modelplan.OrderNo.Substring(3, 1));
                    modelmain.MaterialCategory = modelMaterial.Describe;
                    var modelSupplier = _sysSupplierService.getByAccount(modelplan.SupplierCode);
                    modelmain.SupplierName = modelSupplier.Describe;
                    modelmain.CreationTime = DateTime.Now;
                    modelmain.Creator = WorkContext.CurrentUser.Id;
                    _sysOrderMainService.insertOrderMain(modelmain);
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            Entities.Order model = new Entities.Order();
                            var main = _sysOrderMainService.getByAccount(modelplan.OrderNo);
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
                            if (worksheet.Cells[row, item].Value != null)
                            {
                                model.Item = worksheet.Cells[row, item].Value.ToString();
                            }
                            if (worksheet.Cells[row, materialno].Value != null)
                            {
                                model.MaterialCode = worksheet.Cells[row, materialno].Value.ToString();
                            }
                            if (worksheet.Cells[row, name].Value != null)
                            {
                                model.Name = worksheet.Cells[row, name].Value.ToString();
                            }
                           
                                model.PlanItem = worksheet.Cells[row, lineitem].Value.ToString();
                            
                            if (worksheet.Cells[row, technical].Value != null)
                            {
                                model.Specification = worksheet.Cells[row, technical].Value.ToString();
                            }
                            if (worksheet.Cells[row, width].Value != null)
                            {
                                model.Width = worksheet.Cells[row, width].Value.ToString();
                            }
                            if (worksheet.Cells[row, size].Value != null)
                            {
                                model.Size = worksheet.Cells[row, size].Value.ToString();
                            }
                            if (worksheet.Cells[row, length].Value != null)
                            {
                                model.Length = worksheet.Cells[row, length].Value.ToString();
                            }
                            if (worksheet.Cells[row, packages].Value != null)
                            {
                                model.Package = worksheet.Cells[row, packages].Value.ToString();
                            }
                            if (worksheet.Cells[row, planno].Value != null)
                            {
                                model.Qty = worksheet.Cells[row, planno].Value.ToString();
                            }
                            if (worksheet.Cells[row, planunit].Value != null)
                            {
                                model.Unit = worksheet.Cells[row, planunit].Value.ToString();
                            }
                            if (worksheet.Cells[row, unitprice].Value != null)
                            {
                                model.UnitPrice = worksheet.Cells[row, unitprice].Value.ToString();
                            }
                            if (worksheet.Cells[row, totamount].Value != null)
                            {
                                model.TotalPrice = worksheet.Cells[row, totamount].Value.ToString();
                            }
                            if (worksheet.Cells[row, currency].Value != null)
                            {
                                model.Currency = worksheet.Cells[row, currency].Value.ToString();
                            }
                            if (worksheet.Cells[row, deliverydate].Value != null)
                            {
                                model.LeadTime = Convert.ToDateTime(worksheet.Cells[row, deliverydate].Value.ToString());
                            }
                            if (worksheet.Cells[row, manufacturer].Value != null)
                            {
                                model.Manufacturer = worksheet.Cells[row, manufacturer].Value.ToString();
                            }
                            if (worksheet.Cells[row, origin].Value != null)
                            {
                                model.Origin = worksheet.Cells[row, origin].Value.ToString();
                            }
                            if (worksheet.Cells[row, partno].Value != null)
                            {
                                model.PartNo = worksheet.Cells[row, partno].Value.ToString();
                            }
                            model.CreationTime = DateTime.Now;
                            model.Creator = WorkContext.CurrentUser.Id;
                            _sysOrderService.insertOrder(model);
                        }
                        catch (Exception e)
                        {
                            ViewData["IsShowAlert"] = "True";
                        }
                    }
                   
                }
            } else {
                if (modelplan.Id.Equals(0)) { }
                var model = _sysOrderMainService.getById(modelplan.Id);
                model.OrderNo = modelplan.OrderNo;
                model.OrderConfirmDate = modelplan.OrderConfirmDate;
                model.OrderSigner = modelplan.OrderSigner;
                model.SignerCard = modelplan.SignerCard;
                model.SupplierCode = modelplan.SupplierCode;
                model.SupplierName = modelplan.SupplierName;
                model.TradeTerms = modelplan.TradeTerms;
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