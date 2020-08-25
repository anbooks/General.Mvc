using General.Framework.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;
using General.Entities;
using General.Framework.Security.Admin;
using System.Linq;
using General.Framework.Menu;

namespace General.Framework
{
    public class WorkContext : IWorkContext
    {
        private IAdminAuthService _authenticationService;

        public WorkContext(IAdminAuthService authenticationService)   //引入安全类
        {
            this._authenticationService = authenticationService;
        }




        /// <summary>
        /// 当前登录用户
        /// </summary>
        public SysUser CurrentUser
        {
            get { return _authenticationService.getCurrentUser(); }
            //get { return null; }
        }

        /// <summary>
        /// 当前登录用户菜单
        /// </summary>
        public List<Category> Categories
        {
            get
            {
                //return _authenticationService.getMyCategories();

                return _authenticationService.getMyCategories();
                //return FunctionManager.getFunctionLists().Select(item => new Category {
                //    Name = item.Name,
                //    Action=item.Action,
                //    Controller=item.Controller,
                //    CssClass=item.CssClass,
                //    FatherID=item.FatherID,
                //    FatherResource=item.FatherResource,
                //    IsMenu=item.IsMenu,
                //    ResouceID=item.ResouceID,
                //    RouteName=item.RouteName,
                //    Sort=item.Sort,
                //    SysResource=item.SysResource
                //}).ToList();
            }
        }
    }
}
