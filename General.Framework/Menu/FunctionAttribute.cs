using General.Core.Librs;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Framework.Menu
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class FunctionAttribute : Attribute
    {
        public FunctionAttribute()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isMenu"></param>
        public FunctionAttribute(string name, bool isMenu, int sort = 100)
        {
            this.Name = name;
            this.IsMenu = isMenu;
            this.Sort = sort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="cssClass"></param>
        public FunctionAttribute(string name, string cssClass, int sort = 100)
        {
            this.Name = name;
            this.CssClass = cssClass;
            this.Sort = sort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isMenu"></param>
        /// <param name="cssClass"></param>
        public FunctionAttribute(string name, bool isMenu, string cssClass, int sort = 100)
        {
            this.Name = name;
            this.IsMenu = isMenu;
            this.CssClass = cssClass;
            this.Sort = sort;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 菜单
        /// </summary>
        public bool IsMenu { get; set; }

        /// <summary>
        /// 统一资源定位标识符
        /// 格式：namespace.Controller.Action，或 namespace.Controller
        /// </summary>
        public string SysResource { get; set; }

        /// <summary>
        /// 统一资源定位标识符 MD5 解析
        /// </summary>
        public string ResouceID
        {
            get
            {
                if (!String.IsNullOrEmpty(SysResource))
                    return EncryptorHelper.GetMD5(SysResource);
                return "";
            }
        }

        /// <summary>
        /// 上级统一资源定位标识。
        /// SysResource 的值
        /// </summary>
        public string FatherResource { get; set; }

        /// <summary>
        /// 上级统一资源定位标识md5 解析
        /// </summary>
        public string FatherID
        {
            get
            {
                if (!String.IsNullOrEmpty(FatherResource))
                    return EncryptorHelper.GetMD5(FatherResource);
                return "";
            }
        }

        /// <summary>
        /// 控制器
        /// </summary>
        public string Controller { get; set; }

        /// <summary>
        /// 方法
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 路由名称
        /// </summary>
        public string RouteName { get; set; }

        /// <summary>
        /// css样式
        /// </summary>
        public string CssClass { get; set; }

        /// <summary>
        /// 排序序号,默认100，正序，小的排在前面
        /// </summary>
        public int Sort { get; set; }
    }
}
