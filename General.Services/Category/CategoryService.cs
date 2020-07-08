using System;
using System.Collections.Generic;
using System.Text;
using General.Entities;
using System.Linq;
using General.Core.Data;
using Microsoft.Extensions.Caching.Memory;

namespace General.Services.Category
{
    public class CategoryService : ICategoryService
    {
        private const string MODEL_KEY = "General.services.category";

        private IMemoryCache _memoryCache;
        private IRepository<Entities.Category> _categoryRepository;
        private IRepository<Entities.SysPermission> _permissionRepository;

        public CategoryService(IRepository<Entities.Category> categoryRepository,
            IMemoryCache memoryCache,
            IRepository<Entities.SysPermission> permissionRepository)
        {
            this._permissionRepository = permissionRepository;
            this._memoryCache = memoryCache;
            this._categoryRepository = categoryRepository;
        }

        /// <summary>
        /// 初始化保存方法
        /// </summary>
        /// <param name="list"></param>
        public void initCategory(List<Entities.Category> list)
        {
            var oldList = _categoryRepository.Table.ToList();
            oldList.ForEach(del =>
            {
                var item = list.FirstOrDefault(o => o.SysResource == del.SysResource);
                if (item == null)
                {
                    var permissionList = del.SysPermissions.ToList();
                    permissionList.ForEach(delPrm =>
                    {
                        _permissionRepository.Entities.Remove(delPrm);
                    });
                    _categoryRepository.Entities.Remove(del);
                }
            });
            list.ForEach(entity =>
            {
                var item = oldList.FirstOrDefault(o => o.SysResource == entity.SysResource);
                if (item == null)
                {
                    _categoryRepository.Entities.Add(entity);
                }
                else
                {
                    item.Action = entity.Action;
                    item.Controller = entity.Controller;
                    item.CssClass = entity.CssClass;
                    item.FatherResource = entity.FatherResource;
                    item.IsMenu = entity.IsMenu;
                    item.Name = entity.Name;
                    item.RouteName = entity.RouteName;
                    item.SysResource = entity.SysResource;
                    item.Sort = entity.Sort;
                    item.FatherID = entity.FatherID;
                    item.ResouceID = entity.ResouceID;
                }
            });
            if (_categoryRepository.DbContext.ChangeTracker.HasChanges())
                _categoryRepository.DbContext.SaveChanges();
        }

        /// <summary>
        /// 获取所有的菜单数据 并缓存
        /// </summary>
        /// <returns></returns>
        public List<Entities.Category> getAll()
        {
            List<Entities.Category> list = null;
            _memoryCache.TryGetValue<List<Entities.Category>>(MODEL_KEY, out list);
            if (list != null)
                return list;
            list = _categoryRepository.Table.ToList();
            _memoryCache.Set(MODEL_KEY, list, DateTimeOffset.Now.AddDays(1));
            return list;
        }
    }
}
