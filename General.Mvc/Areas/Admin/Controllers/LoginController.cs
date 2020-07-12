using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework.Controllers.Admin;
using General.Framework.Security.Admin;
using General.Services.SysUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace General.Mvc.Areas.Admin.Controllers
{
    /// <summary>
    /// 后台管理登录控制器
    /// </summary>
    //[Route("admin/login")]
    [Route("admin")]
    public class LoginController : AdminAreaController
    {
        private const string R_KEY = "R_KEY";
        private ISysUserService _sysUserService;
        private IAdminAuthService _authenticationService;

        public LoginController(ISysUserService sysUserService,IAdminAuthService authenticationService)
        {
            this._sysUserService = sysUserService;
            this._authenticationService = authenticationService;
        }


        //[Route("")]
        [Route("login", Name = "adminLogin")]
        public IActionResult LoginIndex()
        {
            string r = EncryptorHelper.GetMD5(Guid.NewGuid().ToString());
            HttpContext.Session.SetString(R_KEY, r);
            LoginModel loginModel = new LoginModel() { R = r };
            return View(loginModel);

            //return View();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginIndex(LoginModel model)
        {
            string r = HttpContext.Session.GetString(R_KEY);
            r = r ?? "";

            //admin abc123
            if (!ModelState.IsValid)
            {
                AjaxData.Message = "请输入用户账号和密码";
                return Json(AjaxData);
            }

           //var result= _sysUserService.validateUser(model.Account,model.Password,"");
            var result = _sysUserService.validateUser(model.Account, model.Password, r);
            AjaxData.Status = result.Item1;
            AjaxData.Message = result.Item2;
            if (result.Item1)
            {
                //_authenticationService.signIn(result.Item3, result.Item4.Name);
                //保存登录状态  ClasimsIdentity,ClaimsPrincipal
                //HttpContext.SignInAsync()
                _authenticationService.signIn(result.Item3, result.Item4.Name);

            }
            else
            {
                //失败也这样
            }


            return Json(AjaxData);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [Route("getSalt", Name = "getSalt")]
        public IActionResult getSalt(string account)
        {
            //http://localhost:50491/admin/getsalt?account=admin
            var user = _sysUserService.getByAccount(account);
            return Content(user?.Salt);
        }


    }
}