﻿using General.Core;
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
        public IPagedList<Entities.Order> searchOrder(SysCustomizedListSearchArg arg, int page, int size, int id)
        {
            var query = _sysOrderRepository.Table.Include(p=>p.Main).Where(o=>o.Main.IsDeleted != true && o.MainId == id&&o.IsDeleted != true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.OrderNo.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.SupplierCode.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.Main.Buyer.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Project.Contains(arg.invcurr));
            }
            query = query.OrderBy(o => o.Name);
            return new PagedList<Entities.Order>(query, page, size);
        }
        public IPagedList<Entities.Order> searchOrderD(SysCustomizedListSearchArg arg, int page, int size, string orderno)
        {
            var query = _sysOrderRepository.Table.Include(p => p.Main).Where(o => o.Main.IsDeleted != true && o.Main.OrderNo == orderno && o.IsDeleted != true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Item.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.PartNo.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.Specification.Contains(arg.pono));
               
            }
            query = query.OrderBy(o => o.Id);
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
            return _sysOrderRepository.Table.Any(o => o.Name == account && o.IsDeleted != true && o.Main.IsDeleted != true);
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

        public Entities.Order getAccount(string account)
        {
            return _sysOrderRepository.Table.Include(p => p.Main).Where(o => o.Main.IsDeleted != true && o.IsDeleted != true).FirstOrDefault(o => o.PlanItem == account && o.IsDeleted != true);
        }
        public Entities.Order getOrder(string a,string b)
        {
            return _sysOrderRepository.Table.Include(p => p.Main).Where(o => o.Main.IsDeleted != true && o.IsDeleted != true).FirstOrDefault(o => o.OrderNo == a && o.Item == b && o.IsDeleted != true);
        }
        public List<Entities.Order> getPlan(string account)
        {
            List<Entities.Order> list = null;

            if (list != null)
                return list;
            list = _sysOrderRepository.Table.Include(p => p.Main).Where(o => o.PlanItem == account && o.IsDeleted != true && o.Main.IsDeleted != true).ToList();
            return list;

        }
        public List<Entities.Order> getMain(int  account)
        {
            List<Entities.Order> list = null;

            if (list != null)
                return list;
            list = _sysOrderRepository.Table.Include(p => p.Main).Where(o => o.MainId == account && o.IsDeleted != true && o.Main.IsDeleted != true).ToList();
            return list;

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
        public void updateOrder(Entities.Order model)
        {
            var item = _sysOrderRepository.getById(model.Id);
            if (item == null)
                return;
            _sysOrderRepository.update(model);
        }

    }
}
