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

namespace General.Services.SysCustomizedList
{
    public class SysCustomizedListService : ISysCustomizedListService
    {
        private IMemoryCache _memoryCache;  //不用每次去数据库里面筛选

        private const string MODEL_KEY = "General.services.sysCustomizedList";

        private IRepository<Entities.SysCustomizedList> _sysCustomizedListRepository;


        public SysCustomizedListService(IRepository<Entities.SysCustomizedList> sysCustomizedListRepository,
            IMemoryCache memoryCache)
        {
            this._memoryCache = memoryCache;
            this._sysCustomizedListRepository = sysCustomizedListRepository;
        }
        public Entities.SysCustomizedList getById(Guid id)
        {
            return _sysCustomizedListRepository.getById(id);
        }
       // public Entities.SysCustomizedList getByAccount(string account)
       // {
        //    return _sysCustomizedListRepository.Table.FirstOrDefault(o => o.CustomizedClassify== account && !o.IsDeleted);
       // }
        public List<Entities.SysCustomizedList> getByAccount(string account)
        {
            List<Entities.SysCustomizedList> list = null;
            
      
            list = _sysCustomizedListRepository.Table.Where(o=>o.CustomizedClassify==account).ToList();
            
            return list;
        }
        public void insertSysCustomized(Entities.SysCustomizedList model)
        {
            _sysCustomizedListRepository.insert(model);
            _memoryCache.Remove(MODEL_KEY);
        }
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role"></param>
        public void updateSysCustomized(Entities.SysCustomizedList model)
        {
            var item = _sysCustomizedListRepository.getById(model.Id);
            if (item == null)
                return;
            item.CustomizedClassify = model.CustomizedClassify;
            item.CustomizedValue = model.CustomizedValue;
            item.Description = model.Description;
            item.ModifiedTime = model.ModifiedTime;
            item.Modifier = model.Modifier;
            _sysCustomizedListRepository.update(item);
            _memoryCache.Remove(MODEL_KEY);
        }
        public IPagedList<Entities.SysCustomizedList> searchList(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysCustomizedListRepository.Table.Where(o => !o.IsDeleted);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.keyword))
                    query = query.Where(o => o.CustomizedClassify.Contains(arg.keyword) || o.CustomizedValue.Contains(arg.keyword) || o.Description.Contains(arg.keyword));
          
            }
            query = query.OrderBy(o => o.CustomizedClassify).ThenBy(o => o.CustomizedValue).ThenByDescending(o => o.CreationTime);
            return new PagedList<Entities.SysCustomizedList>(query, page, size);
        }
    }
}
