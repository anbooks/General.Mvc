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

        Entities.SysUserRole getById(Guid id);

        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        void insertSysUserRole(Entities.SysUserRole model);
        void updateSysUserRole(Entities.SysUserRole model);
        /// <summary>
        /// 移除缓存
        /// </summary>
        void removeCache();
    }
}
