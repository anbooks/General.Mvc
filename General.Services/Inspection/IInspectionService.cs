using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.Inspection
{
    public interface IInspectionService
    {
        IPagedList<Entities.Inspection> searchInspection(SysCustomizedListSearchArg arg, int page, int size,int id);

        Entities.Inspection getByAccount(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.Inspection getById(int id);
        List<Entities.Inspection> getByDate(string account);
        List<Entities.Inspection> getByMain(int id);
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
        void insertInspection(Entities.Inspection model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateInspection(Entities.Inspection model);
        
    }
}
