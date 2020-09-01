using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using General.Core.Librs;
using General.Entities;
using General.Framework.Controllers.Ordinary;
using General.Framework.Security.Ordinary;
using General.Services.SysUser;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace General.Mvc.Areas.Ordinary.Controllers
{/// <summary>
 /// 后台管理登录控制器
 /// </summary>
    [Area("Ordinary")]
    //[Route("admin/login")]
    [Route("ordinary")]
    public class LoginController : OrdinaryAreaController    //这里引用的就是上面framework中的AdminAreaController
    {
        private const string R_KEY = "R_KEY";
        private ISysUserService _sysUserService;
        private IOrdinaryAuthService _authenticationService;

        private IMemoryCache _memoryCache;

        public LoginController(ISysUserService sysUserService, IOrdinaryAuthService authenticationService, IMemoryCache memoryCache)
        {
            this._sysUserService = sysUserService;
            this._authenticationService = authenticationService;
            this._memoryCache = memoryCache;
        }


        //[Route("")]
        [Route("login", Name = "ordinaryLogin")]
        public IActionResult LoginIndex()
        {
            
            string r = EncryptorHelper.GetMD5(Guid.NewGuid().ToString());
            HttpContext.Session.SetString(R_KEY, r);
            LoginModel loginModel = new LoginModel() { R = r };

            //为啥不找自己的视图呢？
            return View(loginModel);

            //return View();
        }

        [HttpPost]
        [Route("login")]
        public IActionResult LoginIndex(LoginModel model)    //loginModel处理登录的事
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
            var result = _sysUserService.validateUser(model.Account, model.Password, r);  //这里成功后就写入了
            AjaxData.Status = result.Item1;
            AjaxData.Message = result.Item2;
            if (result.Item1)
            {
                //_authenticationService.signIn(result.Item3, result.Item4.Name);
                //保存登录状态  ClasimsIdentity,ClaimsPrincipal
                //HttpContext.SignInAsync()

                //登录成功后需要写入token表中
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
        [Route("o_getSalt", Name = "o_getSalt")]
        public IActionResult o_getSalt(string account)
        {
            //http://localhost:50491/admin/getsalt?account=admin
            var user = _sysUserService.getByAccount(account);
            return Content(user?.Salt);
        }


    }
}