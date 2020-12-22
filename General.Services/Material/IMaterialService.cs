using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.Material
{
    public interface IMaterialService
    {
        IPagedList<Entities.Material> searchMaterial(SysCustomizedListSearchArg arg, int page, int size);

        Entities.Material getByAccount(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.Material getById(int id);

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
        void insertMaterial(Entities.Material model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateMaterial(Entities.Material model);
        
    }
}
