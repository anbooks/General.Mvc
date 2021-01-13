using General.Core;
using General.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace General.Services.Attachment
{
    public interface IAttachmentService
    {
        IPagedList<Entities.Attachment> searchAttachment(SysCustomizedListSearchArg arg, int page, int size,int id);
        IPagedList<Entities.Attachment> searchPorkAttachment(SysCustomizedListSearchArg arg, int page, int size, int id);

        Entities.Attachment getByAccount(string account);
        /// <summary>
        /// 获取详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Entities.Attachment getById(int id);

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
        void insertAttachment(Entities.Attachment model);

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        void updateAttachment(Entities.Attachment model);
        
    }
}
