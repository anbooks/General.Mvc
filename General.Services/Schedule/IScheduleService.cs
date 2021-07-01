using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ScheduleService
{
    public interface IScheduleService
    {
        void saveSchedule(List<int> categoryIds);
       
        Entities.Schedule getById(int id);
        IPagedList<Entities.Schedule> searchList(SysCustomizedListSearchArg arg, int page, int size, int id);

        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        void insertSchedule(Entities.Schedule model);
        List<Entities.Schedule> getAll(int id);
        List<Entities.Schedule> getItem(string Item);
        List<Entities.Schedule> getOrder(string a, string b);
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>InventoryInput
        void updateSchedule(Entities.Schedule model);
       
    }
}
