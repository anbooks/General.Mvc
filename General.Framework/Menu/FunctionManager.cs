using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace General.Framework.Menu
{
    /// <summary>
    /// 
    /// </summary>
    public class FunctionManager
    {
        /// <summary>
        /// 获取 action 特性
        /// </summary>
        /// <returns></returns>
        public static List<FunctionAttribute> getFunctionLists()
        {
            var assembly = AssemblyLoadContext.Default.LoadFromAssemblyName(new AssemblyName("General.Mvc"));
            List<FunctionAttribute> result = new List<FunctionAttribute>();
            var types = assembly.GetTypes();
            if (types != null)
            {
                foreach (var type in types)
                {
                    string typeName = type.FullName.ToLower();
                    if (typeName.EndsWith("controller"))
                    {
                        var funAttList = type.GetCustomAttributes<FunctionAttribute>(false);
                        FunctionAttribute father = null;
                        if (funAttList != null && funAttList.Any())
                        {
                            foreach (var fun in funAttList)
                            {
                                if (String.IsNullOrEmpty(fun.SysResource))
                                    fun.SysResource = type.FullName;
                                father = fun;
                                result.Add(fun);
                                break;
                            }
                        }
                        //获取action 方法
                        var members = type.FindMembers(MemberTypes.Method, BindingFlags.Public
                            | BindingFlags.Instance ,Type.FilterName, "*");
                        if (members != null && members.Any())
                        {
                            foreach (var m in members)
                            {
                                var funs = m.GetCustomAttributes<FunctionAttribute>(false);
                                foreach (var fun in funs)
                                {
                                    if (String.IsNullOrEmpty(fun.SysResource))
                                        fun.SysResource = type.FullName + "." + m.Name;
                                    fun.Controller = type.Name.Replace("Controller", "");
                                    fun.Action = m.Name;
                                    //如果父级未指定
                                    if (String.IsNullOrEmpty(fun.FatherResource))
                                        if (father != null)
                                            fun.FatherResource = father.SysResource;
                                    object[] routes = m.GetCustomAttributes(typeof(RouteAttribute), false);
                                    if (routes != null && routes.Any())
                                    {
                                        var route = routes.First() as RouteAttribute;
                                        fun.RouteName = route.Name;
                                    }
                                    result.Add(fun);
                                    break;
                                }
                            }
                        }

                    }
                }
            }
            return result;
        }

    }
}
