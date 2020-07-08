using General.Core.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using General.Entities;

namespace General.Services.SysPermission
{
    public class SysPermissionService : ISysPermissionService
    {
        private IMemoryCache _memoryCache;

        private const string MODEL_KEY = "General.services.sysPermission";

        private IRepository<Entities.SysPermission> _sysPermissionRepository;


        public SysPermissionService(IRepository<Entities.SysPermission> sysPermissionRepository,
            IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._sysPermissionRepository = sysPermissionRepository;
        }

        public List<Entities.SysPermission> getAll()
        {
            List<Entities.SysPermission> list = null;
            _memoryCache.TryGetValue<List<Entities.SysPermission>>(MODEL_KEY, out list);
            if (list != null) return list;
            list = _sysPermissionRepository.Table.ToList();
            _memoryCache.Set(MODEL_KEY, list, DateTimeOffset.Now.AddDays(1));
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Entities.SysPermission> getByRoleId(Guid id)
        {
            var list = getAll();
            if (list == null) return null;
            return list.Where(o => o.RoleId == id).ToList();
        }

        /// <summary>
        /// 保存角色权限数据
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="categoryIds"></param>
        /// <param name="creator"></param>
        public void saveRolePermission(Guid roleId, List<int> categoryIds, Guid creator)
        {
            var list = _sysPermissionRepository.Table.Where(o => o.RoleId == roleId);
            if (categoryIds == null || !categoryIds.Any())
                foreach (var del in list)
                    _sysPermissionRepository.Entities.Remove(del);
            else
            {
                foreach (int categoryId in categoryIds)
                {
                    var item = list.FirstOrDefault(o => o.CategoryId == categoryId);
                    if (item == null)
                    {
                        _sysPermissionRepository.Entities.Add(new Entities.SysPermission()
                        {
                            Id = Guid.NewGuid(),
                            RoleId = roleId,
                            CreationTime = DateTime.Now,
                            Creator = creator,
                            CategoryId = categoryId
                        });
                    }
                }
                foreach (var del in list)
                    if (!categoryIds.Any(o => o == del.CategoryId))
                        _sysPermissionRepository.Entities.Remove(del);
            }
            _sysPermissionRepository.DbContext.SaveChanges();
            removeCache();
        }

        /// <summary>
        /// 移除缓存
        /// </summary>
        public void removeCache()
        {
            _memoryCache.Remove(MODEL_KEY);
        }
    }
}
