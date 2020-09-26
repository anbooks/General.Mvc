using System;
using System.Collections.Generic;
using System.Text;

namespace General.Entities
{
    public class UserRoleViewModel
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
        public List<SysUserRole> UserRoleList { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SysUser User { get; set; }
    }
}
