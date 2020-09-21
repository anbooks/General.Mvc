using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Setting
{
    public class SettingService: ISettingService
    {

        private IRepository<Entities.Setting> _sysSettingRepository;


        public SettingService(IRepository<Entities.Setting> sysSettingRepository)
        {
            this._sysSettingRepository = sysSettingRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Setting> searchSetting(SettingSearchArg arg, int page, int size)
        {
            var query = _sysSettingRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.q))
                    query = query.Where(o => o.Name.Contains(arg.q));
         
            }
            query = query.OrderBy(o => o.Name);
            return new PagedList<Entities.Setting>(query, page, size);
        }


        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysSettingRepository.Table.Any(o => o.Name == account );
        }


        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Setting getById(Guid id)
        {
            return _sysSettingRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertSetting(Entities.Setting model)
        {
            if (existAccount(model.Name))
                return;
            _sysSettingRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateSetting(Entities.Setting model)
        {
            _sysSettingRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysSettingRepository.DbContext.Entry(model).Property("Name").IsModified = true;
            _sysSettingRepository.DbContext.Entry(model).Property("Value").IsModified = true;
            _sysSettingRepository.DbContext.SaveChanges();
        }
    }
}
