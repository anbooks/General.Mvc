using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.Invoice
{
    public interface IInvoiceService
    {
        IPagedList<Entities.Invoice> searchInvoice(SysCustomizedListSearchArg arg, int page, int size,int id);

        Entities.Invoice getByAccount(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.Invoice getById(int id);
        List<Entities.Invoice> getByDate(string account);
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
        void insertInvoice(Entities.Invoice model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateInvoice(Entities.Invoice model);
        
    }
}
