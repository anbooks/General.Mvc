using General.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using General.Services.SysUser;

namespace General.Framework.Security.Admin
{
    public class AdminAuthService:IAdminAuthService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ISysUserService _sysUserService;

        public AdminAuthService(IHttpContextAccessor httpContextAccessor, ISysUserService sysUserService)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._sysUserService = sysUserService;
        }

        public SysUser getCurrentUser()
        {
            //之前保存到cookie中了，现在从cookie中获取
            var result = _httpContextAccessor.HttpContext.AuthenticateAsync(CookieAdminAuthInfo.AuthenticationScheme).Result;
            if (result.Principal == null)
                return null;
            //var token = result.Principal.FindFirstValue(ClaimTypes.Sid);
            var token = result.Principal.FindFirst(ClaimTypes.Sid).Value;
            // return _sysUserService.getLogged(token ?? "");
            //http://localhost:50491/admin/main

            var user = _sysUserService.getLogged(token);

            return _sysUserService.getLogged(token);
           // return new SysUser { Id = Guid.NewGuid(), Name = "李四" };
        }

        public void signIn(string token, string name)
        {
            //throw new NotImplementedException();
            ClaimsIdentity claimsIdentity = new ClaimsIdentity("Forms");
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Sid, token));
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, name));
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
           // _httpContextAccessor.HttpContext.SignInAsync("General", claimsPrincipal);
            _httpContextAccessor.HttpContext.SignInAsync(CookieAdminAuthInfo.AuthenticationScheme, claimsPrincipal);

        }
    }
}
