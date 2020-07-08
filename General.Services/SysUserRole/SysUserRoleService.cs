using General.Core.Data;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace General.Services.SysUserRole
{
    public class SysUserRoleService : ISysUserRoleService
    {
        private IMemoryCache _memoryCache;

        private const string MODEL_KEY = "General.services.userRole";

        private IRepository<Entities.SysUserRole> _sysUserRoleRepository;


        public SysUserRoleService(IRepository<Entities.SysUserRole> sysUserRoleRepository,
            IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._sysUserRoleRepository = sysUserRoleRepository;
        }

        /// <summary>
        /// 获取所有的数据
        /// </summary>
        /// <returns></returns>
        public List<Entities.SysUserRole> getAll()
        {
            List<Entities.SysUserRole> list = null;
            _memoryCache.TryGetValue<List<Entities.SysUserRole>>(MODEL_KEY, out list);
            if (list != null) return list;
            list = _sysUserRoleRepository.Table.ToList();
            _memoryCache.Set(MODEL_KEY,list, DateTimeOffset.Now.AddDays(1));
            return list;
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
