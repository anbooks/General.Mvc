using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.OrderMain
{
    public interface IOrderMainService
    {
        IPagedList<Entities.OrderMain> searchOrderMain(SysCustomizedListSearchArg arg, int page, int size, string role, string name);

        Entities.OrderMain getByAccount(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.OrderMain getById(int id);

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool existAccount(string name);
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        void insertOrderMain(Entities.OrderMain model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateOrderMain(Entities.OrderMain model);
        
    }
}
