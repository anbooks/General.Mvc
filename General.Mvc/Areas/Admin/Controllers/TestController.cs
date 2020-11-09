using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.SysCustomizedList;
using General.Services.test_JqGrid;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/test")]
    public class TestController : AdminPermissionController
    {
        private Itest_JqGridService _itest_JqGridService;
        private  IHostingEnvironment _hostingEnvironment;
        private ISysCustomizedListService _sysCustomizedListService;

        public TestController(Itest_JqGridService itest_JqGridService, IHostingEnvironment hostingEnvironment, ISysCustomizedListService sysCustomizedListService)
        {

            this._itest_JqGridService = itest_JqGridService;
            this._hostingEnvironment = hostingEnvironment;
            this._sysCustomizedListService = sysCustomizedListService;
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

        [Route("testFormSubmits", Name = "testFormSubmits")]
        [Function("测试一个Form多Submits", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestFormSubmits()
        {
            return View();
        }


        [HttpPost]
        [Route("", Name = "editFormSubmits")]
        public ActionResult EditFormSubmits()
        {
            //string rest = 
            //string test = "sdasdad";

            string submit = Request.Form["submit"];
          
            if (submit.Equals("1"))
            {
                AjaxData.Status = true;
                AjaxData.Message = "点击了1!";
            }
            else if (submit.Equals("2"))
            {
                AjaxData.Status = true;
                AjaxData.Message = "点击了2!";
            }
           
            return Json(AjaxData);
            //return View();
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
        [Route("Edit", Name = "Edit")]
        public ActionResult Edit(test_JqGrid pro, string oper, int id)
        {
            if (oper == "edit")
            {
                _itest_JqGridService.updatetest_JqGrid(pro);
                // _repository.Update(pro);
                //// pro.CreateTime = DateTime.Now;
            }
            if (oper == "del")
            {
                _itest_JqGridService.deletetest_JqGrid(id);
                // _repository.Update(pro);
                //// pro.CreateTime = DateTime.Now;
            }
            return Json(pro);
        }

        [Route("testUploadImage", Name = "testUploadImage")]
        [Function("测试上传图片", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestUploadImage()
        {
            return View();
        }

        //封装了
        ///// <summary>
        ///// Json帮助类
        ///// </summary>
        //public class JsonHelper
        //{
        //    /// <summary>
        //    /// 将对象序列化为JSON格式
        //    /// </summary>
        //    /// <param name="o">对象</param>
        //    /// <returns>json字符串</returns>
        //    public static string SerializeObject(object o)
        //    {
        //        string json = JsonConvert.SerializeObject(o);
        //        return json;
        //    }

        //    /// <summary>
        //    /// 解析JSON字符串生成对象实体
        //    /// </summary>
        //    /// <typeparam name="T">对象类型</typeparam>
        //    /// <param name="json">json字符串(eg.{"ID":"112","Name":"石子儿"})</param>
        //    /// <returns>对象实体</returns>
        //    public static T DeserializeJsonToObject<T>(string json) where T : class
        //    {
        //        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //        StringReader sr = new StringReader(json);
        //        object o = serializer.Deserialize(new JsonTextReader(sr), typeof(T));
        //        T t = o as T;
        //        return t;
        //    }

        //    /// <summary>
        //    /// 解析JSON数组生成对象实体集合
        //    /// </summary>
        //    /// <typeparam name="T">对象类型</typeparam>
        //    /// <param name="json">json数组字符串(eg.[{"ID":"112","Name":"石子儿"}])</param>
        //    /// <returns>对象实体集合</returns>
        //    public static List<T> DeserializeJsonToList<T>(string json) where T : class
        //    {
        //        Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();
        //        StringReader sr = new StringReader(json);
        //        object o = serializer.Deserialize(new JsonTextReader(sr), typeof(List<T>));
        //        List<T> list = o as List<T>;
        //        return list;
        //    }

        //    /// <summary>
        //    /// 反序列化JSON到给定的匿名对象.
        //    /// </summary>
        //    /// <typeparam name="T">匿名对象类型</typeparam>
        //    /// <param name="json">json字符串</param>
        //    /// <param name="anonymousTypeObject">匿名对象</param>
        //    /// <returns>匿名对象</returns>
        //    public static T DeserializeAnonymousType<T>(string json, T anonymousTypeObject)
        //    {
        //        T t = JsonConvert.DeserializeAnonymousType(json, anonymousTypeObject);
        //        return t;
        //    }
        //}


        [HttpPost]
        [Route("registerResult", Name = "registerResult")]
        public ActionResult RegisterResult([FromForm(Name = "avatar")]List<IFormFile>  files)
        {
            string show_Url = "";
             //var files = Request.Form.Files;
            //string test= Request.Form["phone"].ToString();
            files.ForEach(file =>
            {
                var fileName = file.FileName;
                string fileExtension = file.FileName.Substring(file.FileName.LastIndexOf(".") + 1);//获取文件名称后缀 
                //保存文件
                var stream = file.OpenReadStream();
                // 把 Stream 转换成 byte[] 
                byte[] bytes = new byte[stream.Length];
                stream.Read(bytes, 0, bytes.Length);
                // 设置当前流的位置为流的开始 
                stream.Seek(0, SeekOrigin.Begin);
                // 把 byte[] 写入文件 

                //需要写一个获取文件路径的方法  Kevin？
                // var path = Directory.GetCurrentDirectory();
                var currentDate = DateTime.Now;
                var webRootPath = _hostingEnvironment.WebRootPath;//>>>相当于HttpContext.Current.Server.MapPath("") 

                //F:\Code\ILPT\General.Mvc\General.Mvc\wwwroot
                var fileProfile = webRootPath + "\\Files\\profile\\";

                FileStream fs = new FileStream(fileProfile + file.FileName, FileMode.Create);
                //FileStream fs = new FileStream("D:\\" + file.FileName, FileMode.Create);
                BinaryWriter bw = new BinaryWriter(fs);
                bw.Write(bytes);
                bw.Close();
                fs.Close();
                show_Url = "http://localhost:50491/Files/profile/" + file.FileName;
                //操作model中的值给数据库赋值 Kevin?
            });
            //实现返回值的设置 Kevin？
            AjaxDataImage.Status = "OK";
            AjaxDataImage.Message = "上传成功";

            AjaxDataImage.Url = show_Url;

            return Json(AjaxDataImage);


        }



        [Route("testCheckbox", Name = "testCheckbox")]
        [Function("测试复选框如何传值", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.TestIndex")]
        public IActionResult TestCheckbox()
        {
            var customizedList = _sysCustomizedListService.getByAccount("货币类型");
            ViewData["Companys"] = new SelectList(customizedList, "CustomizedValue", "CustomizedValue");
            List<Entities.test_JqGrid> test_JqGrids = _itest_JqGridService.getAlltest_JqGrid();

            return View(test_JqGrids);
           
        }

        [HttpPost]
        [Route("submitcheckbox", Name = "submitcheckbox")]
        public ActionResult Submitcheckbox(string kevin,string returnUrl=null)
        {
            //ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("testCheckbox");

            string test = kevin;

            var Id= Request.Form["checkboxId"];
            var Value = Request.Form["checkboxValue"];   //处理checkbox的值

            List<Entities.test_JqGrid> jsonlist = JsonHelper.DeserializeJsonToList<Entities.test_JqGrid>(test);


            Entities.test_JqGrid model = new Entities.test_JqGrid();
            foreach (Entities.test_JqGrid u in jsonlist)
            {
                var  test1 = u.Name;
                var test2 = u.ShipVia;

                //u就是jsonlist里面的一个实体类
            }




            var Name = Request.Form["Name"];
            var Invcurr = Request.Form["invcurr"];
            //_itest_JqGridService.updatetest_JqGrid(model);
            //string test = "sdasdad";
            // _importTrans_main_recordService.saveShippingMode(sysResource);
            AjaxData.Status = true;
            AjaxData.Message = "OK";


            return Json(AjaxData);
            //return View();
            //return Redirect(ViewBag.ReturnUrl);
        }


    }
    }