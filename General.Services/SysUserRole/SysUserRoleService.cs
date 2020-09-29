using General.Core.Data;

using System;
using System.Collections.Generic;
using System.Text;
using General.Entities;
using System.Linq;
using General.Core.Librs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using General.Core;
namespace General.Services.SysUserRole
{
    public class SysUserRoleService : ISysUserRoleService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.userRole";

        private IRepository<Entities.SysUserRole> _sysUserRoleRepository;


        public SysUserRoleService(IRepository<Entities.SysUserRole> sysUserRoleRepository,
            IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._sysUserRoleRepository = sysUserRoleRepository;
        }
        //public Entities.SysUserRole getById(Guid id)
        //{
        //    return _sysUserRoleRepository.getById(id);
        //}

        public Entities.SysUserRole getById(Guid Userid)
        {
            return _sysUserRoleRepository.Table.FirstOrDefault(o => o.UserId == Userid);
          
        }


        public void insertSysUserRole(Entities.SysUserRole model)
        {
            if (existUserId(model.UserId))
                return;
            _sysUserRoleRepository.insert(model);
            removeCache();
        }

  


        public bool existUserId(Guid Userid)
            {
                return _sysUserRoleRepository.Table.Any(o => o.UserId == Userid);
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
        public void updateSysUserRole(Entities.SysUserRole model)
        {
            //_sysUserRoleRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysUserRoleRepository.DbContext.Entry(model).Property("UserId").IsModified = true;
            _sysUserRoleRepository.DbContext.Entry(model).Property("RoleId").IsModified = true;
            _sysUserRoleRepository.DbContext.SaveChanges();
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
