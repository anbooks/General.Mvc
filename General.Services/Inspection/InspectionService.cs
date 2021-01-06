using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Inspection
{
    public class InspectionService : IInspectionService
    {

        private IRepository<Entities.Inspection> _sysInspectionRepository;


        public InspectionService(IRepository<Entities.Inspection> sysInspectionRepository)
        {
            this._sysInspectionRepository = sysInspectionRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Inspection> searchInspection(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysInspectionRepository.Table.Where(o=>o.ReceivedQty==0|| o.ReceivedQty == null);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.ContractNo.Contains(arg.pono));
            }
           // query = query.OrderBy(o => o.Cre);
            return new PagedList<Entities.Inspection>(query, page, size);
        }
        public Entities.Inspection getByAccount(string account)
        {
            return _sysInspectionRepository.Table.FirstOrDefault(o => o.ContractNo == account);
        }

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysInspectionRepository.Table.Any(o => o.Batch == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Inspection getById(int id)
        {
            return _sysInspectionRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertInspection(Entities.Inspection model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysInspectionRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateInspection(Entities.Inspection model)
        {
            var item = _sysInspectionRepository.getById(model.Id);
            if (item == null)
                return;
            _sysInspectionRepository.update(model);
        }
    }
}
