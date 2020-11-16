using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Order
{
    public class OrderService : IOrderService
    {

        private IRepository<Entities.Order> _sysOrderRepository;


        public OrderService(IRepository<Entities.Order> sysOrderRepository)
        {
            this._sysOrderRepository = sysOrderRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Order> searchOrder(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysOrderRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.Name.Contains(arg.pono));
         
            }
            query = query.OrderBy(o => o.Name);
            return new PagedList<Entities.Order>(query, page, size);
        }


        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysOrderRepository.Table.Any(o => o.Name == account );
        }


        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Order getById(int id)
        {
            return _sysOrderRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertOrder(Entities.Order model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysOrderRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateOrder(Entities.Order model)
        {
            _sysOrderRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysOrderRepository.DbContext.Entry(model).Property("Name").IsModified = true;
            _sysOrderRepository.DbContext.Entry(model).Property("Item").IsModified = true;
            _sysOrderRepository.DbContext.SaveChanges();
        }
    }
}
