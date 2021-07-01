using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.Order
{
    public interface IOrderService
    {
        IPagedList<Entities.Order> searchOrder(SysCustomizedListSearchArg arg, int page, int size, int id);

        IPagedList<Entities.Order> searchOrderD(SysCustomizedListSearchArg arg, int page, int size, string orderno);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.Order getById(int id);
        Entities.Order getAccount(string account); 
             Entities.Order getOrder(string a, string b);
        List<Entities.Order> getPlan(string account);
        /// <summary>
        /// 验证账号是否已经存在getPlan
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool existAccount(string name);
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        void insertOrder(Entities.Order model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateOrder(Entities.Order model);
        
    }
}
