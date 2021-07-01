using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Supplier
{
    public class SupplierService : ISupplierService
    {

        private IRepository<Entities.Supplier> _sysSupplierRepository;


        public SupplierService(IRepository<Entities.Supplier> sysSupplierRepository)
        {
            this._sysSupplierRepository = sysSupplierRepository;
        }
        public Entities.Supplier getByAccount(string account)
        {
            return _sysSupplierRepository.Table.FirstOrDefault(o => o.SupplierCode == account&&o.SupplierCode!=null);
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Supplier> searchSupplier(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysSupplierRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.keyword))
                    query = query.Where(o => o.SupplierCode.Contains(arg.keyword));
            }
            query = query.OrderBy(o => o.SupplierCode);
            return new PagedList<Entities.Supplier>(query, page, size);
        }
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysSupplierRepository.Table.Any(o => o.SupplierCode == account );
        }
        public List<Entities.Supplier> getCount(string co,int id)
        {
            List<Entities.Supplier> list = null;

            if (list != null)
                return list;
           
                list = _sysSupplierRepository.Table.Where(o => o.SupplierCode == co&&o.Id!=id).ToList();
            

            return list;
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Supplier getById(int id)
        {
            return _sysSupplierRepository.getById(id);
        }
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertSupplier(Entities.Supplier model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysSupplierRepository.insert(model);
        }
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateSupplier(Entities.Supplier model)
        {
            _sysSupplierRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysSupplierRepository.DbContext.Entry(model).Property("SupplierCode").IsModified = true;
            _sysSupplierRepository.DbContext.Entry(model).Property("Describe").IsModified = true;
            _sysSupplierRepository.DbContext.SaveChanges();
        }
    }
}
