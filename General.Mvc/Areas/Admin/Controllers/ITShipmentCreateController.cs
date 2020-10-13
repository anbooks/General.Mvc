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

namespace General.Mvc.Areas.Admin.Controllers
{

    [Route("admin/itShipmentCreate")]
    public class ITShipmentCreateController : AdminPermissionController
    {    
        private IImportTrans_main_recordService _importTrans_main_recordService;

        
        public ITShipmentCreateController(IImportTrans_main_recordService importTrans_main_recordService)
        {

            this._importTrans_main_recordService = importTrans_main_recordService;
        }
        [Route("", Name = "itShipmentCreate")]
        [Function("创建发运条目", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITShipmentCreateIndex(SysCustomizedListSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _importTrans_main_recordService.searchList(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.ImportTrans_main_record, SysCustomizedListSearchArg>("itShipmentCreate", arg);

            return View(dataSource);
        }
    }
}