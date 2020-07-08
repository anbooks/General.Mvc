using General.Core.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Entities;
using General.Services.SysPermission;
using General.Services.SysUserRole;

namespace General.Services.SysRole
{
    public class SysRoleService : ISysRoleService
    {
        private const string MODEL_KEY = "General.services.role_all";
        private IMemoryCache _memoryCache;
        private IRepository<Entities.SysRole> _sysRoleRepository;
        private IRepository<Entities.SysUserRole> _sysUserRoleRepository;
        private IRepository<Entities.SysPermission> _sysPermissionRepository;
        private ISysPermissionService _sysPermissionServices;
        private ISysUserRoleService _sysUserRoleService;

        public SysRoleService(IMemoryCache memoryCache,
             ISysUserRoleService sysUserRoleService,
             IRepository<Entities.SysRole> sysRoleRepository,
             IRepository<Entities.SysUserRole> sysUserRoleRepository,
             ISysPermissionService sysPermissionServices,
             IRepository<Entities.SysPermission> sysPermissionRepository)
        {
            this._sysUserRoleService = sysUserRoleService;
            this._sysPermissionServices = sysPermissionServices;
            this._memoryCache = memoryCache;
            this._sysRoleRepository = sysRoleRepository;
            this._sysUserRoleRepository = sysUserRoleRepository;
            this._sysPermissionRepository = sysPermissionRepository;
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="roleId"></param>
        public void deleteRole(Guid roleId)
        {
            var item = _sysRoleRepository.getById(roleId);
            if (item == null)
                return;
            foreach (var del in item.SysPermission.ToList())
                _sysPermissionRepository.Entities.Remove(del);
            foreach (var del in _sysUserRoleRepository.Table.Where(o => o.RoleId == roleId).ToList())
                _sysUserRoleRepository.Entities.Remove(del);
            _sysRoleRepository.delete(item);
            _memoryCache.Remove(MODEL_KEY);
            _sysPermissionServices.removeCache();
            _sysUserRoleService.removeCache();
        }

        /// <summary>
        /// 获取所有的roles数据
        /// 并缓存
        /// </summary>
        /// <returns></returns>
        public List<Entities.SysRole> getAllRoles()
        {
            List<Entities.SysRole> list = null;
            _memoryCache.TryGetValue<List<Entities.SysRole>>(MODEL_KEY, out list);
            if (list != null)
                return list;
             list = _sysRoleRepository.Table.ToList();
            _memoryCache.Set(MODEL_KEY, list, DateTimeOffset.Now.AddDays(1));
            return list;
        }


        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="role"></param>
        public void inserRole(Entities.SysRole role)
        {
            _sysRoleRepository.insert(role);
            _memoryCache.Remove(MODEL_KEY);
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        public void updateRole(Entities.SysRole role)
        {
            var item = _sysRoleRepository.getById(role.Id);
            if (item == null)
                return;
            item.Name = role.Name;
            item.ModifiedTime = DateTime.Now;
            item.Modifier = role.Modifier;
            _sysRoleRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }

        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.SysRole getRole(Guid id)
        {
            return _sysRoleRepository.getById(id);
        }
         

    }
}
