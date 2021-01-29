using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.ProcurementPlanMain
{
    public class ProcurementPlanMainService : IProcurementPlanMainService
    {

        private IRepository<Entities.ProcurementPlanMain> _sysProcurementPlanMainRepository;


        public ProcurementPlanMainService(IRepository<Entities.ProcurementPlanMain> sysProcurementPlanMainRepository)
        {
            this._sysProcurementPlanMainRepository = sysProcurementPlanMainRepository;
        }
        public Entities.ProcurementPlanMain getByAccount(string account)
        {
            return _sysProcurementPlanMainRepository.Table.FirstOrDefault(o => o.PlanItem == account && !o.IsDeleted);
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.ProcurementPlanMain> searchProcurementPlanMain(SysCustomizedListSearchArg arg, int page, int size)
        {
            
            var query = _sysProcurementPlanMainRepository.Table.Where(o=>o.IsDeleted!=true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.PlanItem.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.Project.Contains(arg.pono));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Prepare.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.invcurr))
                    query = query.Where(o => o.Creator.Contains(arg.invcurr));
            }
            query = query.OrderBy(o => o.Id);
            return new PagedList<Entities.ProcurementPlanMain>(query, page, size);
        }
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysProcurementPlanMainRepository.Table.Any(o => o.PlanItem == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.ProcurementPlanMain getById(int id)
        {
            return _sysProcurementPlanMainRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertProcurementPlanMain(Entities.ProcurementPlanMain model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysProcurementPlanMainRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateProcurementPlanMain(Entities.ProcurementPlanMain model)
        {
            var item = _sysProcurementPlanMainRepository.getById(model.Id);
            if (item == null)
                return;
            _sysProcurementPlanMainRepository.update(model);
           
        }
    }
}
