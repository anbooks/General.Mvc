using General.Core;
using General.Core.Data;
using General.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace General.Services.InspecationMain
{
    public class InspecationMainService : IInspecationMainService
    {

        private IRepository<Entities.InspecationMain> _sysInspecationMainRepository;
        private IRepository<Entities.Inspection> _sysInspecationRepository;

        public InspecationMainService(IRepository<Entities.InspecationMain> sysInspecationMainRepository,IRepository<Entities.Inspection> sysInspecationRepository)
        {
            this._sysInspecationMainRepository = sysInspecationMainRepository;
            this._sysInspecationRepository = sysInspecationRepository;
        }

        /// <summary>
        /// 搜索数据
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IPagedList<Entities.InspecationMain> searchInspecationMain(SysCustomizedListSearchArg arg, int page, int size, string name)
        {
            var query = _sysInspecationMainRepository.Table.Where(o=>o.IsDeleted!=true && o.Creator == name);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.InspecationMainId.Contains(arg.itemno));
                if (!String.IsNullOrEmpty(arg.shipper))
                    query = query.Where(o => o.Creator.Contains(arg.shipper));

            }
            query = query.OrderByDescending(o => o.CreationTime);
            return new PagedList<Entities.InspecationMain>(query, page, size);
        }
        public IPagedList<Entities.InspecationMain> searchInspecationMainjh(SysCustomizedListSearchArg arg, int page, int size, string name)
        {
            var query = _sysInspecationMainRepository.Table.Where(o => o.IsDeleted != true&&o.flag>1 && o.JhyName==name);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.InspecationMainId.Contains(arg.itemno));

            }
            query = query.OrderBy(o => o.CreationTime);
            return new PagedList<Entities.InspecationMain>(query, page, size);
        }
        public IPagedList<Entities.InspecationMain> searchInspecationMainjy(SysCustomizedListSearchArg arg, int page, int size)
        {
            var query = _sysInspecationMainRepository.Table.Where(o => o.IsDeleted != true && o.flag > 3);
            if (arg != null)
            {
                if (!String.IsNullOrEmpty(arg.itemno))
                    query = query.Where(o => o.InspecationMainId.Contains(arg.itemno));

            }
            query = query.OrderBy(o => o.CreationTime);
            return new PagedList<Entities.InspecationMain>(query, page, size);
        }
        /// <summary>
        /// 验证账号是否已经存在
        /// </summary>
        /// <param name="id"></param>
        /// <param name="account"></param>
        /// <returns></returns>
        public bool existAccount(string account)
        {
            return _sysInspecationMainRepository.Table.Any(o => o.InspecationMainId == account&&o.IsDeleted!=true);
        }
        /// <summary>
        /// 获取用户详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entities.InspecationMain getById(int id)
        {
            return _sysInspecationMainRepository.getById(id);
        }


        /// <summary>
        /// 新增，插入
        /// </summary>
        /// <param name="model"></param>
        public void insertInspecationMain(Entities.InspecationMain model)
        {
            //if (existAccount(model.Name))
            //   return;
            _sysInspecationMainRepository.insert(model);
        }
        public Entities.InspecationMain getByAccount(string account)
        {
            return _sysInspecationMainRepository.Table.FirstOrDefault(o => o.InspecationMainId == account && o.IsDeleted!=true);
        }
        public List<Entities.InspecationMain> getByDate(string account)
        {
            List<Entities.InspecationMain> list = null;

            if (list != null)
                return list; 
            list =  _sysInspecationMainRepository.Table.Where(o => o.DateId == account).ToList();
            return list;
        }
        
        /// <summary>
        /// 更新修改
        /// </summary>
        /// <param name="model"></param>
        public void updateInspecationMain(Entities.InspecationMain model)
        {
            var item = _sysInspecationMainRepository.getById(model.Id);
            if (item == null)
                return;
            _sysInspecationMainRepository.update(model);
        }
        public void deleteInspecationMain(Entities.InspecationMain model)
        {
            var item = _sysInspecationMainRepository.getById(model.Id);
            if (item == null)
                return;
            List<Entities.Inspection> list = null;
            list = _sysInspecationRepository.Table.Where(o => o.MainId == item.Id).ToList();
            foreach (Entities.Inspection u in list)
            {
                var ins = _sysInspecationRepository.getById(u.Id);
                ins.IsDeleted = true;
                _sysInspecationRepository.update(ins);
            }
            _sysInspecationMainRepository.update(model);
        }
        public void spInspecationMain(Entities.InspecationMain model)
        {
            var item = _sysInspecationMainRepository.getById(model.Id);
            if (item == null)
                return;
            List<Entities.Inspection> list = null;
            list = _sysInspecationRepository.Table.Where(o => o.MainId == item.Id).ToList();
            foreach (Entities.Inspection u in list)
            {
                var ins = _sysInspecationRepository.getById(u.Id);
                ins.flag = model.flag;
                _sysInspecationRepository.update(ins);
            }
            _sysInspecationMainRepository.update(model);
        }
    }
}
