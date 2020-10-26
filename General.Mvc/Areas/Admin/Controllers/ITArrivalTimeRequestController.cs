using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework;
using General.Framework.Controllers.Admin;
using General.Framework.Menu;
using General.Services.ImportTrans_main_recordService;
using General.Services.test_JqGrid;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/itArrivalTimeRequest")]
    public class ITArrivalTimeRequestController : AdminPermissionController
    {
        private Itest_JqGridService _itest_JqGridService;
        private IImportTrans_main_recordService _importTrans_main_recordService;
        public ITArrivalTimeRequestController(IImportTrans_main_recordService importTrans_main_recordService, Itest_JqGridService itest_JqGridService)
        {
            this._importTrans_main_recordService = importTrans_main_recordService;
            this._itest_JqGridService = itest_JqGridService;
        }
        [Route("", Name = "itArrivalTimeRequest")]
        [Function("要求到货日期2", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.ImportTransportationController", Sort = 1)]

        public IActionResult ITArrivalTimeRequestIndex()
        {
            return View();
        }
        [Route("itArrivalTimeRequest2", Name = "itArrivalTimeRequest2")]
           public JsonResult ITArrivalTimeRequestIndex2(string sord = "asc", string sidx = "ProductName", int page = 1,
            int rows = 10, bool _search = false, string searchField = "", string searchOper = "", string searchString = "")
        {

            List<Entities.ImportTrans_main_record> test_JqGrids = _importTrans_main_recordService.getAll();
            int count = test_JqGrids.Count();
            // var objpros = new List<object>(pros);editArrivalTime
            var jsonData = JqGridModel.GridData(page, rows, count, test_JqGrids);
            return Json(jsonData);
        }
        [HttpPost]
        [Route("editArrivalTime", Name = "editArrivalTime")]
        public ActionResult Edit(ImportTrans_main_record pro, string oper, int id)
        {
            if (oper == "edit")
            {
                pro.Requester = WorkContext.CurrentUser.Id;
                pro.RequestTime = DateTime.Now;
                _importTrans_main_recordService.updateArrivalTime(pro);
            }
            return Json(pro);
        }
    }
}