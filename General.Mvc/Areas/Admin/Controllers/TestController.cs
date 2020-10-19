using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Entities;
using General.Framework;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.test_JqGrid;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/test")]
    public class TestController : AdminPermissionController
    {
        private Itest_JqGridService _itest_JqGridService;


        public TestController(Itest_JqGridService itest_JqGridService)
        {

            this._itest_JqGridService = itest_JqGridService;
        }





        [Route("", Name = "testIndex")]
        [Function("测试入口", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.CodingLibraryController", Sort = 2)]
        public IActionResult TestIndex()
        {
            return View();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("testFr", Name = "testFr")]  
        [Function("测试Finereport", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestFr()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("testWebservice", Name = "testWebservice")]
        [Function("测试Webservice", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestWebservice()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("testTableJqGrid", Name = "testTableJqGrid")]
        [Function("测试JqGrid", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestTableJqGrid()
        {
            return View();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Route("nonparam", Name = "nonparamReport")]
       // [Function("新增、修改角色", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.RoleController.RoleIndex")]
        public IActionResult NonparamReport()
        {
            // http://localhost:8075/WebReport/ReportServer?reportlet=SysUser.cpt

         //  return Redirect("http://192.168.14.107:8075/WebReport/ReportServer?reportlet=SysUser.cpt");  //临时重定向
            return Content("来了老弟");
          
        }

        [Route("jqGridIndex", Name = "jqGridIndex")]
        //public IActionResult JqGridIndex(test_JqGridSearchArg arg, int page = 1, int size = 20)
        //{
        //    var pageList = _itest_JqGridService.searchList(arg, page, size);
        //    ViewBag.Arg = arg;//传参数
        //    var dataSource = pageList.toDataSourceResult<Entities.test_JqGrid, test_JqGridSearchArg>("jqGridIndex", arg);

        //    return View(dataSource);
        //}
        public IActionResult JqGridIndex()
        {
            //test_JqGrid model = new test_JqGrid();
            //model.Id = 1;
            //model.Name = "test";

            List<Entities.test_JqGrid> test_JqGrids = _itest_JqGridService.getAlltest_JqGrid();

            return View(test_JqGrids);
        }


        [Route("jqGridIndex2", Name = "jqGridIndex2")]
        public JsonResult JqGridIndex2(string sord = "asc", string sidx = "ProductName", int page = 1,
            int rows = 10, bool _search = false, string searchField = "", string searchOper = "", string searchString = "")
        {

            List<Entities.test_JqGrid> test_JqGrids = _itest_JqGridService.getAlltest_JqGrid();
            int count = test_JqGrids.Count();
           // var objpros = new List<object>(pros);
            var jsonData = JqGridModel.GridData(page, rows, count, test_JqGrids);
            return Json(jsonData);
        }

        [HttpPost]
        public ActionResult Edit(test_JqGrid pro, string oper, int id)
        {
            if (oper == "edit")
            {
               // _repository.Update(pro);
               //// pro.CreateTime = DateTime.Now;
            }
            return Json(pro);
        }


    }
}