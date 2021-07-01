using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.ForwardChoose
{
    public interface IForwardChooseService
    {
        IPagedList<Entities.ForwardChoose> searchForwardChoose(SysCustomizedListSearchArg arg, int page, int size);
       

        Entities.ForwardChoose getByAccount(string account);
        Entities.ForwardChoose getForwarda(string a);
        Entities.ForwardChoose getForwardb(string a, string b);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.ForwardChoose getById(int id);
        List<Entities.ForwardChoose> getCount(string a, string b);
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        bool existAccount(string a, string b);
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        void insertForwardChoose(Entities.ForwardChoose model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateForwardChoose(Entities.ForwardChoose model);
        
    }
}
