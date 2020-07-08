using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.SysUserRole
{
    public interface ISysUserRoleService
    {
        /// <summary>
        /// 获取所有的数据
        /// </summary>
        /// <returns></returns>
        List<Entities.SysUserRole> getAll();

        /// <summary>
        /// 移除缓存
        /// </summary>
        void removeCache();
    }
}
