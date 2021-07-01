using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Invoice
{
    public class InvoiceService : IInvoiceService
    {

        private IRepository<Entities.Invoice> _sysInvoiceRepository;


        public InvoiceService(IRepository<Entities.Invoice> sysInvoiceRepository)
        {
            this._sysInvoiceRepository = sysInvoiceRepository;
        }
        public List<Entities.Invoice> getByDate(string account)
        {
            List<Entities.Invoice> list = null;

            if (list != null)
                return list;
            list = _sysInvoiceRepository.Table.Where(o => o.DateId == account && o.IsDeleted != true).ToList();
            return list;
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Invoice> searchInvoice(SysCustomizedListSearchArg arg, int page, int size,int id)
        {
            var query = _sysInvoiceRepository.Table.Include(p => p.InvoiceMain).Where(o=>o.UnPlaceQty!=0&& o.InvoiceMain.IsDeleted != true && o.InvoiceMain.Id ==id);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.ContractNo.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Batch.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.MaterialCode.Contains(arg.pono));
            }
           // query = query.OrderBy(o => o.Cre);
            return new PagedList<Entities.Invoice>(query, page, size);
        }
        public Entities.Invoice getByAccount(string account)
        {
            return _sysInvoiceRepository.Table.FirstOrDefault(o => o.ContractNo == account);
        }

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysInvoiceRepository.Table.Any(o => o.Batch == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Invoice getById(int id)
        {
            return _sysInvoiceRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertInvoice(Entities.Invoice model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysInvoiceRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateInvoice(Entities.Invoice model)
        {
            var item = _sysInvoiceRepository.getById(model.Id);
            if (item == null)
                return;
            _sysInvoiceRepository.update(model);
        }
    }
}
