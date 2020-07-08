using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Services.SysRole
{
    public interface ISysRoleService
    {
        /// <summary>
        /// 获取所有的roles数据
        /// 并缓存
        /// </summary>
        /// <returns></returns>
        List<Entities.SysRole> getAllRoles();
         
        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.SysRole getRole(Guid id);

        /// <summary>
        /// 保存新增角色
        /// </summary>
        /// <param name="role"></param>
        void inserRole(Entities.SysRole role);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        void deleteRole(Guid roleId);

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="role"></param>
        void updateRole(Entities.SysRole role);

    }
}
