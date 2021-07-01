using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.InvoiceMain
{
    public interface IInvoiceMainService
    {
        IPagedList<Entities.InvoiceMain> searchInvoiceMain(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.InvoiceMain> searchInvoiceMainjh(SysCustomizedListSearchArg arg, int page, int size);
        Entities.InvoiceMain getByAccount(string account);
        List<Entities.InvoiceMain> getByDate(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.InvoiceMain getById(int id);

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
        void insertInvoiceMain(Entities.InvoiceMain model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateInvoiceMain(Entities.InvoiceMain model);
        void deleteInvoiceMain(Entities.InvoiceMain model);
        void spInvoiceMain(Entities.InvoiceMain model);
    }
}
