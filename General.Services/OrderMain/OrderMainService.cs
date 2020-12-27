using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.OrderMain
{
    public class OrderMainService : IOrderMainService
    {

        private IRepository<Entities.OrderMain> _sysOrderMainRepository;


        public OrderMainService(IRepository<Entities.OrderMain> sysOrderMainRepository)
        {
            this._sysOrderMainRepository = sysOrderMainRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.OrderMain> searchOrderMain(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysOrderMainRepository.Table.Where(o=>o.IsDeleted!=true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.OrderNo.Contains(arg.pono));
            }
            query = query.OrderBy(o => o.OrderNo);
            return new PagedList<Entities.OrderMain>(query, page, size);
        }


        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysOrderMainRepository.Table.Any(o => o.OrderNo == account);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.OrderMain getById(int id)
        {
            return _sysOrderMainRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertOrderMain(Entities.OrderMain model)
        {
            //if (existAccount(model.Name))
            //   return;
            _sysOrderMainRepository.insert(model);
        }
        public Entities.OrderMain getByAccount(string account)
        {
            return _sysOrderMainRepository.Table.FirstOrDefault(o => o.OrderNo == account && !o.IsDeleted);
        }
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        public void updateOrderMain(Entities.OrderMain model)
        {
            var item = _sysOrderMainRepository.getById(model.Id);
            if (item == null)
                return;
            _sysOrderMainRepository.update(model);
        }

    }
}
