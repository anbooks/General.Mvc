using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Datatable;
using General.Framework.Menu;
using General.Services.Setting;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    [Route("admin/settings")]
    public class SettingController : AdminPermissionController
    {
        private ISettingService _settingService;

        public SettingController(ISettingService settingService)
        {
            this._settingService = settingService;
        }



        [Function("常用配置", true, "menu-icon fa fa-caret-right", FatherResource = "General.Mvc.Areas.Admin.Controllers.SystemManageController", Sort = 3)]
        [Route("", Name = "settingIndex")]
        public IActionResult SettingIndex(SettingSearchArg arg, int page = 1, int size = 20)
        {
            var pageList = _settingService.searchSetting(arg, page, size);
            ViewBag.Arg = arg;//传参数
            var dataSource = pageList.toDataSourceResult<Entities.Setting, SettingSearchArg>("settingIndex", arg);

            return View(dataSource);
        }


        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="id"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("edit", Name = "editSetting")]
        [Function("编辑系统配置", false, FatherResource = "General.Mvc.Areas.Admin.Controllers.SettingController.SettingIndex")]
        public IActionResult EditSetting(Guid? id, string returnUrl = null)
        {
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("settingIndex");
            if (id != null)
            {
                var model = _settingService.getById(id.Value);
                if (model == null)
                    return Redirect(ViewBag.ReturnUrl);
                return View(model);
            }
            return View();
        }
        [HttpPost]
        [Route("edit")]
        public ActionResult EditSetting(Entities.Setting model, string returnUrl = null)
        {
            ModelState.Remove("Id");
            ViewBag.ReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.RouteUrl("settingIndex");
            if (!ModelState.IsValid)
                return View(model);
            
            model.Name = model.Name.Trim();

            if (model.Id == Guid.Empty)
            {
                model.Id = Guid.NewGuid();
               
                _settingService.insertSetting(model);
            }
            else
            {

                _settingService.updateSetting(model);
            }
            return Redirect(ViewBag.ReturnUrl);
        }


    }
}