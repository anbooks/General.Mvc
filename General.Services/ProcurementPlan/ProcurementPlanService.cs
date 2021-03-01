using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.ProcurementPlan
{
    public class ProcurementPlanService : IProcurementPlanService
    {

        private IRepository<Entities.ProcurementPlan> _sysProcurementPlanRepository;


        public ProcurementPlanService(IRepository<Entities.ProcurementPlan> sysProcurementPlanRepository)
        {
            this._sysProcurementPlanRepository = sysProcurementPlanRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.ProcurementPlan> searchProcurementPlan(SysCustomizedListSearchArg arg, int page, int size,int id)
        {
            
            var query = _sysProcurementPlanRepository.Table.Include(p=>p.PlanMain).Where(o=>o.IsDeleted!=true&&o.MainId==id);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.Item.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Materialno.Contains(arg.shipper));
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.Name.Contains(arg.pono));
                if (arg.realreceivingdateend != null)
                    query = query.Where(o => o.CreationTime < arg.realreceivingdateend.Value.AddDays(1));
                if (arg.realreceivingdatestrat != null)
                    query = query.Where(o => o.CreationTime > arg.realreceivingdatestrat);
            }
            query = query.OrderBy(o => o.Id);
            return new PagedList<Entities.ProcurementPlan>(query, page, size);
        }


        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysProcurementPlanRepository.Table.Any(o => o.Name == account && o.IsDeleted != true);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.ProcurementPlan getById(int id)
        {
            return _sysProcurementPlanRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertProcurementPlan(Entities.ProcurementPlan model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysProcurementPlanRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateProcurementPlan(Entities.ProcurementPlan model)
        {
            var item = _sysProcurementPlanRepository.getById(model.Id);
            if (item == null)
                return;
            _sysProcurementPlanRepository.update(model);
           
        }
    }
}
