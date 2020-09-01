using General.Core;
using General.Framework.Security.Ordinary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class OrdinaryAuthFilter : Attribute, IResourceFilter
    {


        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //throw new NotImplementedException();
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            var _ordinaryAuthService = EnginContext.Current.Resolve<IOrdinaryAuthService>();
            var user = _ordinaryAuthService.getCurrentUser();
            if (user == null || !user.Enabled)
                context.Result = new RedirectToRouteResult("adminLogin", new { returnUrl = context.HttpContext.Request.Path });
            //跳转到这里adminLogin

            //throw new NotImplementedException();
            //防止不登录就使用系统
            // context.Result = new RedirectToRouteResult("adminLogin",new { resultUrl =context.HttpContext.Request.Path});
        }
    }
}
