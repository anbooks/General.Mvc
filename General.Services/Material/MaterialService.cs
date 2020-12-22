using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Material
{
    public class MaterialService : IMaterialService
    {

        private IRepository<Entities.Material> _sysMaterialRepository;


        public MaterialService(IRepository<Entities.Material> sysMaterialRepository)
        {
            this._sysMaterialRepository = sysMaterialRepository;
        }
        public Entities.Material getByAccount(string account)
        {
            return _sysMaterialRepository.Table.FirstOrDefault(o => o.MaterialCode == account);
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Material> searchMaterial(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysMaterialRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.MaterialCode.Contains(arg.pono));
            }
            query = query.OrderBy(o => o.MaterialCode);
            return new PagedList<Entities.Material>(query, page, size);
        }
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysMaterialRepository.Table.Any(o => o.MaterialCode == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Material getById(int id)
        {
            return _sysMaterialRepository.getById(id);
        }
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertMaterial(Entities.Material model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysMaterialRepository.insert(model);
        }
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateMaterial(Entities.Material model)
        {
            _sysMaterialRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysMaterialRepository.DbContext.Entry(model).Property("MaterialCode").IsModified = true;
            _sysMaterialRepository.DbContext.Entry(model).Property("Describe").IsModified = true;
            _sysMaterialRepository.DbContext.SaveChanges();
        }
    }
}
