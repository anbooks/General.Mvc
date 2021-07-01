using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.InspecationMain
{
    public interface IInspecationMainService
    {
        IPagedList<Entities.InspecationMain> searchInspecationMain(SysCustomizedListSearchArg arg, int page, int size, string name);
        IPagedList<Entities.InspecationMain> searchInspecationMainjh(SysCustomizedListSearchArg arg, int page, int size, string name);
        IPagedList<Entities.InspecationMain> searchInspecationMainjy(SysCustomizedListSearchArg arg, int page, int size);
        Entities.InspecationMain getByAccount(string account);
        List<Entities.InspecationMain> getByDate(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.InspecationMain getById(int id);

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
        void insertInspecationMain(Entities.InspecationMain model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateInspecationMain(Entities.InspecationMain model);
        void deleteInspecationMain(Entities.InspecationMain model);
        void spInspecationMain(Entities.InspecationMain model);
    }
}
