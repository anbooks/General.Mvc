using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.InvoiceMain
{
    public class InvoiceMainService : IInvoiceMainService
    {

        private IRepository<Entities.InvoiceMain> _sysInvoiceMainRepository;
        private IRepository<Entities.Inspection> _sysInvoiceRepository;

        public InvoiceMainService(IRepository<Entities.InvoiceMain> sysInvoiceMainRepository,IRepository<Entities.Inspection> sysInvoiceRepository)
        {
            this._sysInvoiceMainRepository = sysInvoiceMainRepository;
            this._sysInvoiceRepository = sysInvoiceRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.InvoiceMain> searchInvoiceMain(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysInvoiceMainRepository.Table.Where(o=>o.IsDeleted!=true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.InvoiceMainId.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Creator.Contains(arg.shipper));

            }
            query = query.OrderBy(o => o.CreationTime);
            return new PagedList<Entities.InvoiceMain>(query, page, size);
        }
        public IPagedList<Entities.InvoiceMain> searchInvoiceMainjh(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysInvoiceMainRepository.Table.Where(o => o.IsDeleted != true&&o.flag>0);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.InvoiceMainId.Contains(arg.itemno));

            }
            query = query.OrderBy(o => o.CreationTime);
            return new PagedList<Entities.InvoiceMain>(query, page, size);
        }

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysInvoiceMainRepository.Table.Any(o => o.InvoiceMainId == account&&o.IsDeleted!=true);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.InvoiceMain getById(int id)
        {
            return _sysInvoiceMainRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertInvoiceMain(Entities.InvoiceMain model)
        {
            //if (existAccount(model.Name))
            //   return;
            _sysInvoiceMainRepository.insert(model);
        }
        public Entities.InvoiceMain getByAccount(string account)
        {
            return _sysInvoiceMainRepository.Table.FirstOrDefault(o => o.InvoiceMainId == account && o.IsDeleted!=true);
        }
        public List<Entities.InvoiceMain> getByDate(string account)
        {
            List<Entities.InvoiceMain> list = null;

            if (list != null)
                return list; 
            list =  _sysInvoiceMainRepository.Table.Where(o => o.DateId == account && o.IsDeleted!=true).ToList();
            return list;
        }
        
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        public void updateInvoiceMain(Entities.InvoiceMain model)
        {
            var item = _sysInvoiceMainRepository.getById(model.Id);
            if (item == null)
                return;
            _sysInvoiceMainRepository.update(model);
        }
        public void deleteInvoiceMain(Entities.InvoiceMain model)
        {
            var item = _sysInvoiceMainRepository.getById(model.Id);
            if (item == null)
                return;
            List<Entities.Inspection> list = null;
            list = _sysInvoiceRepository.Table.Where(o => o.MainId == item.Id).ToList();
            foreach (Entities.Inspection u in list)
            {
                var ins = _sysInvoiceRepository.getById(u.Id);
                ins.IsDeleted = true;
                _sysInvoiceRepository.update(ins);
            }
            _sysInvoiceMainRepository.update(model);
        }
        public void spInvoiceMain(Entities.InvoiceMain model)
        {
            var item = _sysInvoiceMainRepository.getById(model.Id);
            if (item == null)
                return;
            List<Entities.Inspection> list = null;
            list = _sysInvoiceRepository.Table.Where(o => o.MainId == item.Id).ToList();
            foreach (Entities.Inspection u in list)
            {
                var ins = _sysInvoiceRepository.getById(u.Id);
                ins.flag = model.flag;
                _sysInvoiceRepository.update(ins);
            }
            _sysInvoiceMainRepository.update(model);
        }
    }
}
