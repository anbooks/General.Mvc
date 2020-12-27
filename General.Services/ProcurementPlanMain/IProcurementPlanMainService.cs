using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ProcurementPlanMain
{
    public interface IProcurementPlanMainService
    {
        IPagedList<Entities.ProcurementPlanMain> searchProcurementPlanMain(SysCustomizedListSearchArg arg, int page, int size);
        Entities.ProcurementPlanMain getByAccount(string account);

        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.ProcurementPlanMain getById(int id);

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
        void insertProcurementPlanMain(Entities.ProcurementPlanMain model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateProcurementPlanMain(Entities.ProcurementPlanMain model);
        
    }
}
