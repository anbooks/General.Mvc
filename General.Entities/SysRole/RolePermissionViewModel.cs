using System;
using System.Collections.Generic;
using System.Text;

namespace General.Entities
{
    public class RolePermissionViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SysRole Role { get; set; }

        /// <summary>
        /// 角色select下拉菜单
        /// </summary>
        public List<SysRole> RoleList { get; set; }

        /// <summary>
        /// 角色的权限数据
        /// </summary>
        public List<SysPermission> Permissions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<Category> CategoryList { get; set; }
    }
}
