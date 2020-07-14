using General.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using General.Services.SysUser;
using General.Services.Category;
using General.Services.SysUserRole;
using General.Services.SysPermission;

namespace General.Framework.Security.Admin
{
    public class AdminAuthService:IAdminAuthService
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ISysUserService _sysUserService;
        private ICategoryService _categoryService;
        private ISysUserRoleService _sysUserRoleService;
        private ISysPermissionService _sysPermissionServices;

        public AdminAuthService(IHttpContextAccessor httpContextAccessor, ISysUserService sysUserService, ICategoryService categoryService, ISysUserRoleService sysUserRoleService, ISysPermissionService sysPermissionServices)
        {
            this._httpContextAccessor = httpContextAccessor;
            this._sysUserService = sysUserService;
            this._categoryService = categoryService;
            this._sysUserRoleService = sysUserRoleService;
            this._sysPermissionServices = sysPermissionServices;
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

            //return _sysUserService.getLogged(token);
            return _sysUserService.getLogged(token ?? "");
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

        public void signOut()
        {

            _httpContextAccessor.HttpContext.SignOutAsync(CookieAdminAuthInfo.AuthenticationScheme);

        }

        /// <summary>
        /// 获取我的权限数据
        /// </summary>
        /// <returns></returns>
        public List<Entities.Category> getMyCategories()
        {
            //var user = getCurrentUser();
            // return getMyCategories(user);
            //return _categoryService.getAll();
            var list = _categoryService.getAll();
            var user = getCurrentUser();
            if (user == null) return null;
            if (user.IsAdmin) return list;



            //获取权限数据

           var userRoles= _sysUserRoleService.getAll();
            if (userRoles == null) return null;
            if (userRoles == null || !userRoles.Any()) return null;
            var roleIds = userRoles.Where(o => o.UserId == user.Id).Select(x => x.RoleId).Distinct().ToList();

            var permissionList = _sysPermissionServices.getAll();
            if (permissionList == null || !permissionList.Any()) return null;

            var categoryIds = permissionList.Where(o => roleIds.Contains(o.RoleId)).Select(x => x.CategoryId).Distinct().ToList();
            if (!categoryIds.Any())
                return null;
            list = list.Where(o => categoryIds.Contains(o.Id)).ToList();
            return list;





        }


    }
}
