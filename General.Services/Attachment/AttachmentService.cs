using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {

        private IRepository<Entities.Attachment> _sysAttachmentRepository;


        public AttachmentService(IRepository<Entities.Attachment> sysAttachmentRepository)
        {
            this._sysAttachmentRepository = sysAttachmentRepository;
        }
        public Entities.Attachment getByAccount(string account)
        {
            return _sysAttachmentRepository.Table.FirstOrDefault(o => o.AttachmentLoad == account);
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.Attachment> searchAttachment(SysCustomizedListSearchArg arg, int page, int size,int id)
        {
            var query = _sysAttachmentRepository.Table.Where(o=>o.ImportId==id && o.Type != "破损记录");
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.ImportId==Convert.ToInt32(arg.pono));
            }
            query = query.OrderBy(o => o.Type);
            return new PagedList<Entities.Attachment>(query, page, size);
        }
        public IPagedList<Entities.Attachment> searchPorkAttachment(SysCustomizedListSearchArg arg, int page, int size, int id)
        {
            var query = _sysAttachmentRepository.Table.Where(o => o.ImportId == id&&o.Type=="破损记录");
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.ImportId == Convert.ToInt32(arg.pono));
            }
            query = query.OrderBy(o => o.ImportId);
            return new PagedList<Entities.Attachment>(query, page, size);
        }
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysAttachmentRepository.Table.Any(o => o.AttachmentLoad == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.Attachment getById(int id)
        {
            return _sysAttachmentRepository.getById(id);
        }
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertAttachment(Entities.Attachment model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysAttachmentRepository.insert(model);
        }
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateAttachment(Entities.Attachment model)
        {
            _sysAttachmentRepository.DbContext.Entry(model).State = EntityState.Unchanged;
            _sysAttachmentRepository.DbContext.Entry(model).Property("AttachmentCode").IsModified = true;
            _sysAttachmentRepository.DbContext.Entry(model).Property("Describe").IsModified = true;
            _sysAttachmentRepository.DbContext.SaveChanges();
        }
    }
}
