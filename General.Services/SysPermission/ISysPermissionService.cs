using System;
using System.Collections.Generic;
using System.Text;
using General.Entities;

namespace General.Services.SysPermission
{
    public interface ISysPermissionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        List<Entities.SysPermission> getAll();
         
        /// <summary>
        /// 移除缓存
        /// </summary>
        void removeCache();

        /// <summary>
        /// 获取角色权限
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Entities.SysPermission> getByRoleId(Guid id);

        /// <summary>
        /// 保存角色权限数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="categoryIds"></param>
        /// <param name="creator"></param>
        void saveRolePermission(Guid roleId, List<int> categoryIds, Guid creator);
    }
}
