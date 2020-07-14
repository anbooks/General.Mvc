using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Security.Admin
{
   public  interface IAdminAuthService
    {

        void signIn(string token, string name);


       
        void signOut();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        Entities.SysUser getCurrentUser();


        /// <summary>
        /// 获取我的权限数据
        /// </summary>
        /// <returns></returns>
        List<Entities.Category> getMyCategories();

        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="routeName"></param> 
        /// <returns></returns>
        bool authorize(string routeName);


        /// <summary>
        /// 权限验证
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        bool authorize(ActionExecutingContext context);
    }
}
