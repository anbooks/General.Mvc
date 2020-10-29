using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.ImportTrans_main_recordService;
using General.Services.SysCustomizedList;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itCheckAndPass")]
    public class ITCheckAndPassController : AdminPermissionController
    {
        private IImportTrans_main_recordService _importTrans_main_recordService;
        private ISysCustomizedListService _sysCustomizedListService;
        public ITCheckAndPassController(IImportTrans_main_recordService importTrans_main_recordService, ISysCustomizedListService sysCustomizedListService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
            this._sysCustomizedListService = sysCustomizedListService;
        }
        [Route("", Name = "itCheckAndPass")]
        [Function("核放", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 11)]

        public IActionResult ITCheckAndPassIndex(List<int> sysResource, SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            RolePermissionViewModel model = new RolePermissionViewModel();
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");

            var pageList = _importTrans_main_recordService.searchListCheckAndPass(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itCheckAndPass", arg);
            return View(dataSource);//sysImport
        }

        [HttpPost]
        [Route("")]
        public ActionResult ITCheckAndPassIndex(List<int> sysResource)
        {
            //string test = "sdasdad";
            _importTrans_main_recordService.saveCheckAndPass(sysResource,WorkContext.CurrentUser.Id);
            AjaxData.Status = true;
            AjaxData.Message = "确认创建成功";
            return Json(AjaxData);
            //return View();
        }
    }
}