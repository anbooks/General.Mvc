using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.InspectionAttachment
{
    public class InspectionAttachmentService : IInspectionAttachmentService
    {

        private IRepository<Entities.InspectionAttachment> _sysInspectionAttachmentRepository;


        public InspectionAttachmentService(IRepository<Entities.InspectionAttachment> sysInspectionAttachmentRepository)
        {
            this._sysInspectionAttachmentRepository = sysInspectionAttachmentRepository;
        }
        public Entities.InspectionAttachment getByAccount(string account)
        {
            return _sysInspectionAttachmentRepository.Table.FirstOrDefault(o => o.AttachmentLoad == account);
        }
        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.InspectionAttachment> searchInspectionAttachment(SysCustomizedListSearchArg arg, int page, int size,int id)
        {
            var query = _sysInspectionAttachmentRepository.Table.Where(o=>o.ImportId==id && o.Type == "质保单" && o.IsDelet != true);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.ImportId==Convert.ToInt32(arg.pono));
            }
            query = query.OrderBy(o => o.Type);
            return new PagedList<Entities.InspectionAttachment>(query, page, size);
        }
        
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysInspectionAttachmentRepository.Table.Any(o => o.AttachmentLoad == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.InspectionAttachment getById(int id)
        {
            return _sysInspectionAttachmentRepository.getById(id);
        }
        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertInspectionAttachment(Entities.InspectionAttachment model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysInspectionAttachmentRepository.insert(model);
        }
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateInspectionAttachment(Entities.InspectionAttachment model)
        {
            var item = _sysInspectionAttachmentRepository.getById(model.Id);
            if (item == null)
                return;
            _sysInspectionAttachmentRepository.update(model);
        }
    }
}
