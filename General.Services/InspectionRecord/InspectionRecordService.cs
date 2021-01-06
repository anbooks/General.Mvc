using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.InspectionRecord
{
    public class InspectionRecordService : IInspectionRecordService
    {

        private IRepository<Entities.InspectionRecord> _sysInspectionRecordRepository;


        public InspectionRecordService(IRepository<Entities.InspectionRecord> sysInspectionRecordRepository)
        {
            this._sysInspectionRecordRepository = sysInspectionRecordRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.InspectionRecord> searchInspectionRecord(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysInspectionRecordRepository.Table;
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.pono))
                    query = query.Where(o => o.ContractNo.Contains(arg.pono));
            }
           // query = query.OrderBy(o => o.Cre);
            return new PagedList<Entities.InspectionRecord>(query, page, size);
        }
        public Entities.InspectionRecord getByAccount(string account)
        {
            return _sysInspectionRecordRepository.Table.FirstOrDefault(o => o.ContractNo == account);
        }

        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysInspectionRecordRepository.Table.Any(o => o.Batch == account );
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.InspectionRecord getById(int id)
        {
            return _sysInspectionRecordRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertInspectionRecord(Entities.InspectionRecord model)
        {
            //if (existAccount(model.Name))
             //   return;
            _sysInspectionRecordRepository.insert(model);
        }

        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
      public  void updateInspectionRecord(Entities.InspectionRecord model)
        {
            var item = _sysInspectionRecordRepository.getById(model.Id);
            if (item == null)
                return;
            _sysInspectionRecordRepository.update(model);
        }
    }
}
