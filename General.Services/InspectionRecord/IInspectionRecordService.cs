using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.InspectionRecord
{
    public interface IInspectionRecordService
    {
        IPagedList<Entities.InspectionRecord> searchInspectionRecord(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.InspectionRecord> searchInspectionjy(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.InspectionRecord> searchInspectionEnd(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.InspectionRecord> searchInspectionbg(SysCustomizedListSearchArg arg, int page, int size);
        IPagedList<Entities.InspectionRecord> searchInspectionzg(SysCustomizedListSearchArg arg, int page, int size);
       
        Entities.InspectionRecord getByAccount(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.InspectionRecord getById(int id);

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
        void insertInspectionRecord(Entities.InspectionRecord model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateInspectionRecord(Entities.InspectionRecord model);
        
    }
}
