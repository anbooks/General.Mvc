﻿using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ProcurementPlan
{
    public interface IProcurementPlanService
    {
        IPagedList<Entities.ProcurementPlan> searchProcurementPlan(SysCustomizedListSearchArg arg, int page, int size,int id);

       
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.ProcurementPlan getById(int id);

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
        void insertProcurementPlan(Entities.ProcurementPlan model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateProcurementPlan(Entities.ProcurementPlan model);
        
    }
}
